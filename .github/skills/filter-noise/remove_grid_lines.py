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
"""Claim regularly spaced grid lines as a non-destructive noise mask layer."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import background_colour
from pdkit.masks import load_mask, rebuild_combined

STAGE = "remove-grid-lines"
LAYER = "grid-lines"
CONTRAST = 8
MIN_LINE_COVERAGE = 0.45
MIN_LINES = 3
SPACING_TOLERANCE = 0.18
MAX_THICKNESS = 4


def main() -> int:
    parser = base_parser("Remove regularly spaced horizontal and vertical grid lines.")
    parser.add_argument("--contrast", type=int, default=CONTRAST, help="minimum page contrast for grid pixels")
    parser.add_argument(
        "--spacing-tolerance",
        type=float,
        default=SPACING_TOLERANCE,
        help="relative spacing variation accepted as a grid (default %(default)s)",
    )
    parser.add_argument("--drop", action="store_true", help="drop this layer and rebuild the combined mask")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    if args.drop:
        return _drop(workdir, extraction, image, result)

    area = extraction.data.get("plot_area") or {}
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["grid lines   none"])

    claim, horizontal, vertical = _detect(workdir, image, extraction, area, args.contrast, args.spacing_tolerance)
    pixels = int(claim.sum())
    if horizontal < MIN_LINES and vertical < MIN_LINES:
        result.note("no regularly spaced grid lines found; the layer claims nothing")
    else:
        result.note(f"detected {horizontal} horizontal and {vertical} vertical grid lines")
    claim = _spare(claim, extraction)
    path = workdir.masks / f"{LAYER}.png"
    save_mask(path, claim)
    extraction.add_mask_layer(LAYER, "noise", workdir.relative(path), int(claim.sum()))
    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    result.confidence = _confidence(horizontal, vertical, pixels)
    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"lines       {horizontal} horizontal, {vertical} vertical",
        f"layer       {int(claim.sum())} px   {workdir.relative(path)}",
        f"combined    {combined} px over {len(extraction.mask_layers('noise'))} noise layers",
        f"contrast    {args.contrast} levels, spacing tolerance {args.spacing_tolerance:g}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _detect(workdir, image, extraction, area, contrast, tolerance):
    height, width = image.shape[:2]
    x, y = max(0, int(round(area["x"]))), max(0, int(round(area["y"])))
    right = min(width, int(round(area["x"] + area["width"])))
    bottom = min(height, int(round(area["y"] + area["height"])))
    crop = image[y:bottom, x:right]
    background = background_colour(image).astype(np.int16)
    difference = np.abs(crop.astype(np.int16) - background).max(axis=2)
    field = difference > contrast
    grey = cv2.cvtColor(crop, cv2.COLOR_BGR2GRAY)
    gradient = cv2.morphologyEx(grey, cv2.MORPH_GRADIENT, np.ones((3, 3), np.uint8))
    field |= gradient > max(contrast // 2, 3)
    horizontal_mask = _line_candidates(field, axis=1, coverage=MIN_LINE_COVERAGE, thickness=MAX_THICKNESS)
    vertical_mask = _line_candidates(field, axis=0, coverage=MIN_LINE_COVERAGE, thickness=MAX_THICKNESS)
    horizontal_positions = _positions(horizontal_mask, axis=0)
    vertical_positions = _positions(vertical_mask, axis=1)
    horizontal_positions = _regular(horizontal_positions, tolerance)
    vertical_positions = _regular(vertical_positions, tolerance)
    claim = np.zeros((height, width), dtype=bool)
    claim[y:bottom, x:right] = _keep_positions(horizontal_mask, horizontal_positions, axis=0) | _keep_positions(vertical_mask, vertical_positions, axis=1)
    for layer in extraction.mask_layers(kind="noise"):
        if layer.get("name") == LAYER:
            continue
        path = workdir.path / layer["file"]
        if path.exists():
            claim &= ~load_mask(path)
    return claim, len(horizontal_positions), len(vertical_positions)


def _line_candidates(field, axis, coverage, thickness):
    length = field.shape[1] if axis == 1 else field.shape[0]
    counts = field.sum(axis=axis) / max(length, 1)
    occupied = counts >= coverage
    runs = _runs(np.flatnonzero(occupied))
    selected = np.zeros_like(field, dtype=bool)
    for run in runs:
        if run.size > thickness:
            continue
        if axis == 1:
            selected[run, :] = field[run, :]
        else:
            selected[:, run] = field[:, run]
    return selected


def _positions(mask, axis):
    counts = mask.sum(axis=1 if axis == 0 else 0)
    occupied = np.flatnonzero(counts > 0)
    return [float(run.mean()) for run in _runs(occupied)]


def _regular(positions, tolerance):
    if len(positions) < MIN_LINES:
        return []
    positions = sorted(positions)
    gaps = np.diff(positions)
    positive = gaps[gaps > 1]
    if positive.size == 0:
        return []
    typical = float(np.median(positive))
    if typical and float(np.median(np.abs(positive - typical)) / typical) <= tolerance:
        return positions
    return positions if _repeated_spacing(positions, tolerance) else []


def _repeated_spacing(positions, tolerance):
    """Accept logarithmic grids whose minor-line pattern repeats once per decade."""
    if len(positions) < 8:
        return False
    differences = np.asarray(
        [right - left for index, left in enumerate(positions) for right in positions[index + 1 :]],
        dtype=float,
    )
    candidates = differences[differences > 20]
    if not candidates.size:
        return False
    for period in sorted(candidates, key=lambda value: abs(value - np.median(candidates))):
        tolerance_pixels = max(3.0, period * tolerance * 0.08)
        repeats = int(np.count_nonzero(np.abs(differences - period) <= tolerance_pixels))
        if repeats >= max(3, len(positions) // 3):
            return True
    return False


def _keep_positions(mask, positions, axis):
    if not positions:
        return np.zeros_like(mask, dtype=bool)
    selected = np.zeros_like(mask, dtype=bool)
    for position in positions:
        centre = int(round(position))
        if axis == 0:
            selected[max(0, centre - 1) : min(mask.shape[0], centre + 2), :] |= mask[max(0, centre - 1) : min(mask.shape[0], centre + 2), :]
        else:
            selected[:, max(0, centre - 1) : min(mask.shape[1], centre + 2)] |= mask[:, max(0, centre - 1) : min(mask.shape[1], centre + 2)]
    return selected


def _spare(claim, extraction):
    for region in extraction.data.get("protected_regions", []):
        _clear(claim, region)
    return claim


def _drop(workdir, extraction, image, result):
    path = workdir.masks / f"{LAYER}.png"
    path.unlink(missing_ok=True)
    extraction.data["mask_layers"] = [layer for layer in extraction.data["mask_layers"] if layer.get("name") != LAYER]
    result.confidence = 1.0
    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, [f"dropped     {LAYER}", f"combined    {combined} px"], overlay)


def _draw(image, workdir, extraction):
    overlay = Overlay(image)
    for index, layer in enumerate(extraction.mask_layers(kind="noise")):
        path = workdir.path / layer["file"]
        if path.exists():
            overlay.tint(load_mask(path), palette_colour(index), label=layer["name"])
    for region in extraction.data.get("protected_regions", []):
        overlay.rectangle(region, palette_colour(6), label=f"protected: {region.get('name', '?')}")
    return overlay


def _confidence(horizontal, vertical, pixels):
    if horizontal < MIN_LINES and vertical < MIN_LINES:
        return 0.3
    return round(min(0.9, 0.5 + 0.05 * min(horizontal + vertical, 8)), 3)


def _runs(indices):
    if len(indices) == 0:
        return []
    return np.split(indices, np.flatnonzero(np.diff(indices) > 1) + 1)


def _clear(mask, box):
    x, y = max(0, int(round(box.get("x", 0)))), max(0, int(round(box.get("y", 0))))
    right = min(mask.shape[1], int(round(box.get("x", 0) + box.get("width", 0))))
    bottom = min(mask.shape[0], int(round(box.get("y", 0) + box.get("height", 0))))
    mask[y:bottom, x:right] = False


if __name__ == "__main__":
    raise SystemExit(main())