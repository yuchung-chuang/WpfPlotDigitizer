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
"""Project every extracted series into data units and write the exports.

Produces a tidy table with a series column, one file per series, and a copy of the extraction with
its confidences and diagnostics.
"""

from __future__ import annotations

import csv
import json
import shutil
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import numpy as np

from pdkit import Result, base_parser, finish, open_workspace
from pdkit.project import axes_ready, to_data

STAGE = "export-data"


def main() -> int:
    parser = base_parser("Project extracted series into data units and write the exports.")
    parser.add_argument(
        "--decimals", type=int, default=6, help="digits kept in the exports (default %(default)s)"
    )
    args = parser.parse_args()

    workdir, extraction, _ = open_workspace(args)
    result = Result(stage=STAGE)

    missing = axes_ready(extraction)
    if missing:
        result.fail(
            f"axis {' and '.join(missing)} has no fit; run read_axis_scale.py before exporting"
        )
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["export skipped"])

    series = extraction.data["series"]
    if not series:
        result.fail("no series to export; extract one first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["export skipped"])

    axes = extraction.data["axes"]
    export = workdir.path / "export"
    export.mkdir(parents=True, exist_ok=True)

    rows: list[tuple] = []
    exported = 0
    for entry in series:
        pixels = _read_pixels(workdir, entry, result)
        if pixels is None:
            continue
        x = to_data(pixels[:, 0], axes["x"])
        y = to_data(pixels[:, 1], axes["y"])
        label = entry.get("label") or entry["id"]

        per_series = export / f"{entry['id']}-data.csv"
        _write_points(per_series, axes, x, y, args.decimals)
        entry["points_data"] = f"export/{per_series.name}"
        support_pixels = _read_support_pixels(workdir, entry, result)
        if support_pixels is not None:
            support_x = to_data(support_pixels[:, 0], axes["x"])
            support_y = to_data(support_pixels[:, 1], axes["y"])
            support_file = export / f"{entry['id']}-support-data.csv"
            _write_points(support_file, axes, support_x, support_y, args.decimals)
            entry["support_points_data"] = f"export/{support_file.name}"
        rows.extend((label, round(a, args.decimals), round(b, args.decimals)) for a, b in zip(x, y))
        exported += 1

    tidy = export / "data.csv"
    _write_tidy(tidy, axes, rows)
    shutil.copyfile(workdir.extraction_path, export / "extraction.json")

    result.confidence = min(
        [float(entry.get("confidence") or 0) for entry in series] + [_axis_confidence(axes)]
    )
    lines = [
        f"series      {exported} of {len(series)} exported, {len(rows)} points",
        f"x           {_describe(axes['x'])}",
        f"y           {_describe(axes['y'])}",
        f"tidy        {workdir.display(tidy)}",
        f"document    {workdir.display(export / 'extraction.json')}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines)


def _read_pixels(workdir, entry: dict, result: Result) -> np.ndarray | None:
    reference = entry.get("points_pixel")
    if not reference:
        result.warn(f"series {entry['id']} has no pixel coordinates")
        return None
    path = workdir.path / reference
    if not path.exists():
        result.warn(f"series {entry['id']} points file is missing: {reference}")
        return None
    points = np.genfromtxt(path, delimiter=",", skip_header=1, usecols=(0, 1))
    points = np.atleast_2d(points)
    if points.size == 0:
        result.warn(f"series {entry['id']} has no points")
        return None
    return points


def _read_support_pixels(workdir, entry: dict, result: Result) -> np.ndarray | None:
    reference = entry.get("support_points_pixel")
    if not reference:
        return None
    path = workdir.path / reference
    if not path.exists():
        result.warn(f"series {entry['id']} support points file is missing: {reference}")
        return None
    points = np.genfromtxt(path, delimiter=",", skip_header=1, usecols=(0, 1))
    points = np.atleast_2d(points)
    return points if points.size else None


def _write_points(path: Path, axes: dict, x, y, decimals: int) -> None:
    with path.open("w", newline="", encoding="utf-8") as handle:
        writer = csv.writer(handle)
        writer.writerow([_column(axes["x"], "x"), _column(axes["y"], "y")])
        writer.writerows(zip(np.round(x, decimals), np.round(y, decimals)))


def _write_tidy(path: Path, axes: dict, rows: list[tuple]) -> None:
    with path.open("w", newline="", encoding="utf-8") as handle:
        writer = csv.writer(handle)
        writer.writerow(["series", _column(axes["x"], "x"), _column(axes["y"], "y")])
        writer.writerows(rows)


def _column(axis: dict, fallback: str) -> str:
    name = axis.get("title") or fallback
    unit = axis.get("unit")
    return f"{name} ({unit})" if unit else name


def _describe(axis: dict) -> str:
    scale = axis.get("scale", "linear")
    base = f" base {axis['log_base']}" if scale == "log" and axis.get("log_base") else ""
    reversed_note = ", reversed" if axis.get("reversed") else ""
    return f"{axis['min']:.6g} .. {axis['max']:.6g}   {scale}{base}{reversed_note}"


def _axis_confidence(axes: dict) -> float:
    return min(float(axes[name].get("confidence") or 0) for name in ("x", "y"))


if __name__ == "__main__":
    raise SystemExit(main())
