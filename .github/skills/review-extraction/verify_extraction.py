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
"""Project exported points back onto the figure and report their distance to visible ink."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import numpy as np
from scipy.spatial import cKDTree

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour
from pdkit.ink import ink_mask
from pdkit.project import axes_ready, to_pixels

STAGE = "verify-extraction"
WARN_DISTANCE = 12.0
ERROR_DISTANCE = 15.0


def main() -> int:
    parser = base_parser("Reproject exported series onto the original figure and check their alignment.")
    parser.add_argument(
        "--warn-distance",
        type=float,
        default=WARN_DISTANCE,
        help="mean nearest-ink distance that raises a warning, in pixels (default %(default)s)",
    )
    parser.add_argument(
        "--error-distance",
        type=float,
        default=ERROR_DISTANCE,
        help="mean nearest-ink distance that fails verification, in pixels (default %(default)s)",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    missing = axes_ready(extraction)
    if missing:
        result.fail(f"axis {' and '.join(missing)} has no fit; verification needs data units")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["verification skipped"])

    entries = extraction.data.get("series") or []
    if not entries:
        result.fail("no series to verify; extract and export a series first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["verification skipped"])

    area = extraction.plot_area
    visible = ink_mask(image)
    visible &= _region_mask(visible.shape, area)
    tree = cKDTree(np.column_stack(np.nonzero(visible))[:, ::-1]) if visible.any() else None
    overlay = Overlay(image)
    reports = []
    for index, entry in enumerate(entries):
        values = _read_export(workdir, entry, result)
        if values is None:
            continue
        pixels = np.column_stack(
            [to_pixels(values[:, 0], extraction.data["axes"]["x"]), to_pixels(values[:, 1], extraction.data["axes"]["y"])]
        )
        pixels = pixels[np.isfinite(pixels).all(axis=1)]
        distances = tree.query(pixels)[0] if tree is not None and len(pixels) else np.full(len(pixels), np.inf)
        mean_distance = float(np.mean(distances)) if len(distances) else float("inf")
        reports.append((entry, pixels, mean_distance))
        overlay.points(pixels, palette_colour(index), size=7, label=entry["id"])
        if mean_distance > args.error_distance:
            result.fail(
                f"series {entry['id']} reprojects {mean_distance:.1f} px from ink; "
                "check the axis fit or exported coordinates"
            )
        elif mean_distance > args.warn_distance:
            result.warn(
                f"series {entry['id']} reprojects {mean_distance:.1f} px from ink; "
                "check the extraction overlay"
            )

    if not reports:
        result.fail("no exported series could be verified")
    result.confidence = _confidence(reports, args.warn_distance, args.error_distance)
    overlay_path = overlay.save(workdir.overlay_path(STAGE))
    summary = [
        f"series      {len(reports)} verified",
        *[
            f"{entry['id']}          {len(pixels)} points, mean nearest ink {distance:.2f} px"
            for entry, pixels, distance in reports
        ],
        f"overlay     {workdir.display(overlay_path)}   <-- view this before declaring success",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay_path)


def _read_export(workdir, entry: dict, result: Result) -> np.ndarray | None:
    reference = entry.get("points_data")
    if not reference:
        result.warn(f"series {entry['id']} has no exported data; run export_data.py first")
        return None
    path = workdir.path / reference
    if not path.exists():
        result.warn(f"series {entry['id']} export is missing: {reference}")
        return None
    values = np.genfromtxt(path, delimiter=",", skip_header=1, usecols=(0, 1))
    values = np.atleast_2d(values)
    return values if values.size else None


def _region_mask(shape: tuple[int, int], area: dict) -> np.ndarray:
    mask = np.zeros(shape, dtype=bool)
    if not all(key in area for key in ("x", "y", "width", "height")):
        return mask
    height, width = shape
    x = max(0, int(round(area["x"])))
    y = max(0, int(round(area["y"])))
    right = min(width, int(round(area["x"] + area["width"])))
    bottom = min(height, int(round(area["y"] + area["height"])))
    mask[y:bottom, x:right] = True
    return mask


def _confidence(reports: list, warning: float, error: float) -> float:
    if not reports:
        return 0.0
    distances = [distance for _, _, distance in reports]
    return round(max(0.0, min(1.0, 1.0 - np.mean(distances) / max(error, warning, 1.0))), 3)


if __name__ == "__main__":
    raise SystemExit(main())