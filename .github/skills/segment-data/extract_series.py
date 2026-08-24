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
"""Extract every segmented series and preserve one overlay for the complete figure."""

from __future__ import annotations

import subprocess
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

from pdkit import Extraction, Overlay, Result, base_parser, finish, open_workspace, palette_colour

STAGE = "extract-series"


def main() -> int:
    parser = base_parser("Extract all segmented point and line series in one pass.")
    parser.add_argument("--radius", type=int, default=1, help="point-opening radius")
    parser.add_argument("--z-threshold", type=float, default=3.5, help="point area outlier threshold")
    parser.add_argument("--step", type=float, default=2.0, help="line sampling distance in pixels")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    entries = extraction.data.get("series") or []
    if not entries:
        result.fail("no segmented series recorded; run segment_series.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["extraction skipped"])

    point_script = Path(__file__).with_name("extract_point_series.py")
    line_script = Path(__file__).with_name("extract_line_series.py")
    completed = 0
    for entry in entries:
        script = line_script if _line_only(entry) else point_script
        command = [
            sys.executable,
            str(script),
            "--image",
            str(args.image),
            "--workdir",
            str(workdir.path),
            "--series-id",
            entry["id"],
        ]
        if script == line_script:
            command.extend(["--step", str(args.step)])
        else:
            command.extend(["--radius", str(args.radius), "--z-threshold", str(args.z_threshold)])
        child = subprocess.run(command, capture_output=True, text=True, encoding="utf-8", errors="replace")
        if child.stdout:
            print(child.stdout, end="")
        if child.stderr:
            print(child.stderr, end="", file=sys.stderr)
        if child.returncode:
            result.warn(f"series {entry['id']} extraction exited {child.returncode}")
            continue
        completed += 1

    extraction = Extraction.load(workdir.extraction_path)
    overlay = _overlay(workdir, extraction, image)
    result.confidence = completed / len(entries)
    if completed < len(entries):
        result.warn(f"only {completed} of {len(entries)} series extracted")
    summary = [
        f"series      {completed} of {len(entries)} extracted",
        f"overlay     {workdir.display(overlay)}   <-- view every series together",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _line_only(entry: dict) -> bool:
    style = entry.get("style") or {}
    return style.get("marker") == "none" or style.get("kind") in {"line", "both"} or style.get("line_style", "none") != "none"


def _overlay(workdir, extraction, image):
    overlay = Overlay(image)
    for index, entry in enumerate(extraction.data.get("series", [])):
        reference = entry.get("points_pixel")
        if not reference:
            continue
        path = workdir.path / reference
        if not path.exists():
            continue
        import numpy as np

        points = np.genfromtxt(path, delimiter=",", skip_header=1, usecols=(0, 1))
        points = np.atleast_2d(points)
        if points.size:
            colour = palette_colour(index)
            if entry.get("kind") == "line":
                overlay.points(points, colour, size=3, label=entry["id"])
            else:
                overlay.points(points, colour, size=5, label=entry["id"])
    return overlay.save(workdir.overlay_path(STAGE))


if __name__ == "__main__":
    raise SystemExit(main())