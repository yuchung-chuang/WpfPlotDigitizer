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
"""Claim thin axis-aligned error bars while sparing the marker at each bar's centre."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import ink_mask
from pdkit.masks import load_mask, rebuild_combined

STAGE = "remove-error-bars"
LAYER = "error-bars"
MIN_LENGTH = 10
MAX_LENGTH = 100
MAX_THICKNESS = 4
DEFAULT_MARKER_RADIUS = 4


def main() -> int:
    parser = base_parser("Remove capped and uncapped horizontal or vertical error bars.")
    parser.add_argument("--min-length", type=int, default=MIN_LENGTH, help="shortest whisker in pixels")
    parser.add_argument("--max-length", type=int, default=MAX_LENGTH, help="longest whisker in pixels")
    parser.add_argument("--marker-radius", type=int, default=DEFAULT_MARKER_RADIUS, help="radius spared at each bar centre")
    parser.add_argument("--drop", action="store_true", help="drop this layer and rebuild the combined mask")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    if args.drop:
        return _drop(workdir, extraction, image, result)
    area = extraction.data.get("plot_area") or {}
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["error bars  none"])

    claim, extents = _detect(workdir, image, extraction, area, args.min_length, args.max_length, args.marker_radius)
    path = workdir.masks / f"{LAYER}.png"
    pixels = save_mask(path, claim)
    extraction.add_mask_layer(LAYER, "noise", workdir.relative(path), pixels)
    extraction.data["error_bars"] = {"extents": extents, "marker_radius": args.marker_radius}
    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    result.confidence = 0.7 if extents else 0.3
    if extents:
        result.note(f"detected {len(extents)} thin error-bar extent(s); marker centres were spared")
    else:
        result.note("no thin axis-aligned error bars found; the layer claims nothing")
    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"bars        {len(extents)} detected",
        f"layer       {pixels} px   {workdir.relative(path)}",
        f"combined    {combined} px over {len(extraction.mask_layers('noise'))} noise layers",
        f"length      {args.min_length}-{args.max_length} px, marker radius {args.marker_radius} px spared",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _detect(workdir, image, extraction, area, minimum, maximum, marker_radius):
    height, width = image.shape[:2]
    x, y = max(0, int(round(area["x"]))), max(0, int(round(area["y"])))
    right, bottom = min(width, int(round(area["x"] + area["width"]))), min(height, int(round(area["y"] + area["height"])))
    crop = ink_mask(image)[y:bottom, x:right]
    claim = np.zeros((height, width), dtype=bool)
    extents = []
    for orientation, kernel in (("vertical", (minimum, 1)), ("horizontal", (1, minimum))):
        line_mask = cv2.morphologyEx(crop.astype(np.uint8), cv2.MORPH_OPEN, np.ones(kernel, np.uint8)) > 0
        count, labels, stats, _ = cv2.connectedComponentsWithStats(line_mask.astype(np.uint8), 8)
        for label in range(1, count):
            box_x, box_y, box_width, box_height, pixels = stats[label]
            length = box_height if orientation == "vertical" else box_width
            thickness = box_width if orientation == "vertical" else box_height
            if not minimum <= length <= maximum or thickness > MAX_THICKNESS:
                continue
            if box_x <= 1 or box_y <= 1 or box_x + box_width >= right - x - 1 or box_y + box_height >= bottom - y - 1:
                continue
            component = labels == label
            centre_x = box_x + box_width / 2 + x
            centre_y = box_y + box_height / 2 + y
            full_component = np.zeros((height, width), dtype=np.uint8)
            full_component[y:bottom, x:right] = component
            cv2.circle(full_component, (int(round(centre_x)), int(round(centre_y))), marker_radius, 0, -1)
            claim |= full_component > 0
            extents.append(
                {
                    "orientation": orientation,
                    "x": round(float(box_x + x), 2),
                    "y": round(float(box_y + y), 2),
                    "width": int(box_width),
                    "height": int(box_height),
                    "centre": [round(float(centre_x), 2), round(float(centre_y), 2)],
                    "pixels": int(pixels),
                }
            )
    claim = _exclude_existing(workdir, extraction, claim)
    claim = _spare(claim, extraction)
    return claim, extents


def _exclude_existing(workdir, extraction, claim):
    for layer in extraction.mask_layers(kind="noise"):
        if layer.get("name") == LAYER:
            continue
        path = workdir.path / layer["file"]
        if path.exists():
            claim &= ~load_mask(path)
    return claim


def _spare(claim, extraction):
    for region in extraction.data.get("protected_regions", []):
        x, y = max(0, int(round(region.get("x", 0)))), max(0, int(round(region.get("y", 0))))
        right = min(claim.shape[1], int(round(region.get("x", 0) + region.get("width", 0))))
        bottom = min(claim.shape[0], int(round(region.get("y", 0) + region.get("height", 0))))
        claim[y:bottom, x:right] = False
    return claim


def _drop(workdir, extraction, image, result):
    path = workdir.masks / f"{LAYER}.png"
    path.unlink(missing_ok=True)
    extraction.data["mask_layers"] = [layer for layer in extraction.data["mask_layers"] if layer.get("name") != LAYER]
    extraction.data.pop("error_bars", None)
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


if __name__ == "__main__":
    raise SystemExit(main())