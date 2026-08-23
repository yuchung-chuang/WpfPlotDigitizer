# /// script
# requires-python = ">=3.11"
# dependencies = [
#   "numpy>=2.0",
#   "opencv-python-headless>=4.10",
#   "scipy>=1.14",
#   "scikit-image>=0.24",
#   "scikit-learn>=1.5",
#   "pillow>=10.4",
# ]
# ///
"""Run the deterministic baseline pipeline over every figure with checked-in ground truth."""

from __future__ import annotations

import argparse
import hashlib
import json
import re
import shutil
import subprocess
import sys
from pathlib import Path
from typing import Any

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

from pdkit.truth import load_truth

VERSION = "1"
REPO_ROOT = Path(__file__).resolve().parents[3]
DEFAULT_READINGS = Path(__file__).with_name("corpus-axis-readings.json")
DEFAULT_WORK_ROOT = REPO_ROOT / ".digitize" / "scoreboard"
DEFAULT_OUTPUT = REPO_ROOT / ".scratch" / "plot-digitizer-agent" / "corpus-scoreboard.md"
PIPELINE = (
    ("analyse-chart", "inspect_chart.py"),
    ("analyse-axis", "locate_plot_area.py"),
    ("analyse-axis", "read_axis_scale.py"),
    ("filter-noise", "clear_border.py"),
    ("segment-data", "extract_point_series.py"),
    ("export-data", "export_data.py"),
    ("review-extraction", "score_extraction.py"),
)


def main() -> int:
    parser = argparse.ArgumentParser(description="Score every ground-truthed corpus figure.")
    parser.add_argument(
        "--image-root",
        type=Path,
        default=REPO_ROOT / "images",
        help="directory containing images and adjacent truth CSV files",
    )
    parser.add_argument(
        "--readings",
        type=Path,
        default=DEFAULT_READINGS,
        help="checked-in axis-reading registry",
    )
    parser.add_argument(
        "--work-root",
        type=Path,
        default=DEFAULT_WORK_ROOT,
        help="dedicated generated workspace for the run",
    )
    parser.add_argument(
        "--output",
        type=Path,
        default=DEFAULT_OUTPUT,
        help="committed Markdown report",
    )
    parser.add_argument(
        "--tolerance",
        type=float,
        default=0.005,
        help="passing median error as a fraction of axis range (default %(default)s)",
    )
    args = parser.parse_args()

    image_root = _absolute(args.image_root)
    readings_path = _absolute(args.readings)
    work_root = _absolute(args.work_root)
    output = _absolute(args.output)

    try:
        readings = json.loads(readings_path.read_text(encoding="utf-8"))
        _validate_readings(readings)
    except (OSError, json.JSONDecodeError, ValueError) as error:
        print(f"score-corpus  failed to load axis readings: {error}")
        return 1

    truth_files = sorted(image_root.glob("*.csv"), key=lambda path: path.name.casefold())
    if not truth_files:
        print(f"score-corpus  no ground-truth CSV files found in {image_root}")
        return 1

    image_by_stem = {
        image.stem.casefold(): image
        for image in image_root.iterdir()
        if image.is_file() and image.suffix.casefold() in {".png", ".jpg", ".jpeg", ".bmp", ".tif", ".tiff"}
    }
    work_root.mkdir(parents=True, exist_ok=True)
    rows = []
    for truth_file in truth_files:
        image = image_by_stem.get(truth_file.stem.casefold())
        rows.append(_score_figure(image, truth_file, readings, work_root, args.tolerance))

    report = _render_report(rows, args.tolerance, readings_path)
    output.parent.mkdir(parents=True, exist_ok=True)
    temporary = output.with_name(output.name + ".tmp")
    temporary.write_text(report, encoding="utf-8", newline="\n")
    temporary.replace(output)

    passed = sum(row["passed"] for row in rows)
    expected = sum(row["expected"] for row in rows)
    failures = sum(row["status"] == "FAIL" for row in rows)
    print(f"score-corpus  {passed} of {expected} series passed; {failures} figure(s) failed")
    print(f"  report      {output}")
    return 1 if failures else 0


def _score_figure(
    image: Path | None,
    truth_file: Path,
    readings: dict[str, Any],
    work_root: Path,
    tolerance: float,
) -> dict[str, Any]:
    entry = readings.get("figures", {}).get(image.name) if image else None
    expected = _expected_series(truth_file, entry)
    row = {
        "image": image.name if image else truth_file.stem,
        "expected": expected,
        "found": 0,
        "passed": 0,
        "median": None,
        "p95": None,
        "coverage": None,
        "status": "FAIL",
        "reason": "",
        "workdir": None,
    }
    if image is None:
        row["reason"] = "image beside truth file is missing"
        return row
    if entry is None:
        row["reason"] = "no axis readings are recorded"
        return row

    workdir = work_root / _slug(image.stem)
    row["workdir"] = workdir.relative_to(REPO_ROOT).as_posix()
    if workdir.exists():
        shutil.rmtree(workdir)
    workdir.mkdir(parents=True, exist_ok=True)
    log: list[str] = []

    try:
        for group, filename in PIPELINE:
            command = _stage_command(group, filename, image, truth_file, entry, workdir, tolerance)
            completed = subprocess.run(
                command,
                cwd=REPO_ROOT,
                capture_output=True,
                text=True,
                encoding="utf-8",
                errors="replace",
            )
            log.append(_log_command(command, completed))
            if completed.returncode:
                row["reason"] = f"{group}/{filename} exited {completed.returncode}: {_last_output(completed)}"
                return _write_row_log(row, workdir, log)

        extraction_path = workdir / "extraction.json"
        extraction = json.loads(extraction_path.read_text(encoding="utf-8"))
        score = extraction.get("score")
        if not score:
            row["reason"] = "score stage produced no score"
            return _write_row_log(row, workdir, log)

        reports = score.get("series", [])
        row["expected"] = int(score.get("series_expected", expected))
        row["found"] = int(score.get("series_found", 0))
        row["passed"] = int(score.get("passed", 0))
        row["median"] = _worst(reports, "median")
        row["p95"] = _worst(reports, "p95")
        row["coverage"] = _mean(reports, "coverage")
        row["status"] = "PASS" if row["passed"] == row["expected"] and row["expected"] > 0 else "FAIL"
        if row["status"] == "FAIL":
            row["reason"] = f"{row['passed']} of {row['expected']} series passed"
        return _write_row_log(row, workdir, log)
    except (OSError, json.JSONDecodeError, subprocess.SubprocessError, ValueError) as error:
        row["reason"] = f"pipeline crashed: {error}"
        return _write_row_log(row, workdir, log)


def _stage_command(
    group: str,
    filename: str,
    image: Path,
    truth_file: Path,
    entry: dict[str, Any],
    workdir: Path,
    tolerance: float,
) -> list[str]:
    script = REPO_ROOT / ".github" / "skills" / group / filename
    command = [sys.executable, str(script), "--image", str(image), "--workdir", str(workdir)]
    if filename == "inspect_chart.py":
        command.extend(["--type", entry.get("type", "scatter"), "--declare", "supported"])
    elif filename == "read_axis_scale.py":
        for name in ("x", "y"):
            axis = entry[name]
            command.extend(
                [
                    f"--{name}",
                    _pairs(axis["pairs"]),
                    f"--{name}-scale",
                    axis.get("scale", "linear"),
                    f"--{name}-title",
                    axis["title"],
                ]
            )
            if axis.get("unit"):
                command.extend([f"--{name}-unit", axis["unit"]])
            if axis.get("log_base") is not None:
                command.extend([f"--{name}-log-base", _number(axis["log_base"])])
    elif filename == "extract_point_series.py":
        command.extend(["--series-id", "baseline"])
    elif filename == "score_extraction.py":
        command.extend(["--truth", str(truth_file), "--tolerance", _number(tolerance)])
    return command


def _expected_series(truth_file: Path, entry: dict[str, Any] | None) -> int:
    if entry is None:
        return 0
    try:
        return len(load_truth(truth_file, entry["x"]["title"], entry["y"]["title"]).series)
    except (OSError, ValueError, KeyError):
        return 0


def _validate_readings(readings: dict[str, Any]) -> None:
    if readings.get("schema_version") != 1:
        raise ValueError("schema_version must be 1")
    figures = readings.get("figures")
    if not isinstance(figures, dict):
        raise ValueError("figures must be an object")
    for name, figure in figures.items():
        if not isinstance(figure, dict):
            raise ValueError(f"{name}: figure must be an object")
        for axis_name in ("x", "y"):
            axis = figure.get(axis_name)
            if not isinstance(axis, dict) or not axis.get("title") or len(axis.get("pairs", [])) < 2:
                raise ValueError(f"{name}: {axis_name} needs a title and two axis pairs")
            if axis.get("scale", "linear") not in {"linear", "log"}:
                raise ValueError(f"{name}: {axis_name} scale is invalid")


def _render_report(rows: list[dict[str, Any]], tolerance: float, readings_path: Path) -> str:
    lines = [
        "# Corpus scoreboard",
        "",
        f"Baseline pipeline version: `{VERSION}`",
        f"Tolerance: `{tolerance * 100:.2f}%` median error, per series",
        f"Axis readings: `{readings_path.relative_to(REPO_ROOT).as_posix()}`",
        "",
        "Skill source versions (SHA-256 prefixes):",
    ]
    for group, filename in PIPELINE:
        path = REPO_ROOT / ".github" / "skills" / group / filename
        lines.append(f"- `{group}/{filename}`: `{_sha256(path)[:12]}`")
    lines.extend(
        [
            "",
            "Median and p95 are the worst matched-series values for each figure; coverage is the mean.",
            "",
            "| Figure | Series found | Median | P95 | Coverage | Result |",
            "| --- | ---: | ---: | ---: | ---: | --- |",
        ]
    )
    for row in rows:
        lines.append(
            f"| {_escape(row['image'])} | {row['found']}/{row['expected']} | "
            f"{_percent(row['median'])} | {_percent(row['p95'])} | {_percent(row['coverage'])} | "
            f"{row['status']}{(': ' + _escape(row['reason'])) if row['reason'] else ''} |"
        )
    passed = sum(row["passed"] for row in rows)
    expected = sum(row["expected"] for row in rows)
    lines.extend(["", f"**Total:** {passed} of {expected} series passed.", ""])
    return "\n".join(lines)


def _write_row_log(row: dict[str, Any], workdir: Path, log: list[str]) -> dict[str, Any]:
    (workdir / "scoreboard.log").write_text("\n\n".join(log) + "\n", encoding="utf-8")
    return row


def _log_command(command: list[str], completed: subprocess.CompletedProcess[str]) -> str:
    output = completed.stdout.strip()
    error = completed.stderr.strip()
    return "\n".join(
        [
            "$ " + " ".join(command),
            f"exit {completed.returncode}",
            output,
            error,
        ]
    ).strip()


def _last_output(completed: subprocess.CompletedProcess[str]) -> str:
    text = (completed.stdout + "\n" + completed.stderr).strip()
    return next((line.strip() for line in reversed(text.splitlines()) if line.strip()), "no diagnostic")


def _worst(reports: list[dict[str, Any]], key: str) -> float | None:
    values = [float(report[key]) for report in reports if report.get(key) is not None]
    return max(values) if values else None


def _mean(reports: list[dict[str, Any]], key: str) -> float | None:
    values = [float(report[key]) for report in reports if report.get(key) is not None]
    return sum(values) / len(values) if values else None


def _percent(value: float | None) -> str:
    return "n/a" if value is None else f"{value * 100:.2f}%"


def _pairs(pairs: list[list[float]]) -> str:
    return ",".join(f"{_number(pixel)}={_number(value)}" for pixel, value in pairs)


def _number(value: float | int) -> str:
    return f"{float(value):.15g}"


def _escape(value: str) -> str:
    return str(value).replace("|", "\\|")


def _absolute(path: Path) -> Path:
    return path if path.is_absolute() else (Path.cwd() / path).resolve()


def _slug(name: str) -> str:
    return re.sub(r"[^A-Za-z0-9._-]+", "-", name).strip("-").lower() or "figure"


def _sha256(path: Path) -> str:
    digest = hashlib.sha256()
    digest.update(path.read_bytes())
    return digest.hexdigest()


if __name__ == "__main__":
    raise SystemExit(main())