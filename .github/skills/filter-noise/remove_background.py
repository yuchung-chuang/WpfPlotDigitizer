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
"""Claim smooth, large area fills as background without claiming chart ink."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.masks import load_mask, rebuild_combined

STAGE = "remove-background"
LAYER = "background"
DEFAULT_TOLERANCE = 12
MIN_COMPONENT_FRACTION = 0.002
EDGE_RESIDUAL = 5


def main() -> int:
    parser = base_parser("Remove smooth coloured fills and shaded background areas.")
    parser.add_argument(
        "--tolerance",
        type=int,
        default=DEFAULT_TOLERANCE,
        help="minimum distance from the page colour for a fill (default %(default)s)",
    )
    parser.add_argument(
        "--edge-residual",
        type=int,
        default=EDGE_RESIDUAL,
        help="maximum local residual for a smooth fill pixel (default %(default)s)",
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
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["background   none"])

    claim, colour = _detect(image, area, args.tolerance, args.edge_residual)
    claim = _exclude_existing(workdir, extraction, claim)
    claim = _spare(claim, extraction)
    pixels = int(claim.sum())
    path = workdir.masks / f"{LAYER}.png"
    save_mask(path, claim)
    extraction.add_mask_layer(LAYER, "noise", workdir.relative(path), pixels)
    extraction.data["background"] = {
        "colour_rgb": [int(value) for value in colour[::-1]],
        "tolerance": args.tolerance,
        "edge_residual": args.edge_residual,
    }
    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    result.confidence = 0.75 if pixels else 0.35
    if not pixels:
        result.note("no large smooth area fill found; the layer claims nothing")
    else:
        result.note(f"claimed {pixels} smooth background pixels while preserving high-residual ink")
    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"background  {pixels} px claimed",
        f"colour      RGB {tuple(int(value) for value in colour[::-1])}",
        f"combined    {combined} px over {len(extraction.mask_layers('noise'))} noise layers",
        f"tolerance   {args.tolerance}, local residual <= {args.edge_residual}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _detect(image, area, tolerance, edge_residual):
    height, width = image.shape[:2]
    x = max(0, int(round(area["x"])))
    y = max(0, int(round(area["y"])))
    right = min(width, int(round(area["x"] + area["width"])))
    bottom = min(height, int(round(area["y"] + area["height"])))
    crop = image[y:bottom, x:right]
    colour = _page_colour(image).astype(np.int16)
    smooth = cv2.GaussianBlur(crop, (0, 0), sigmaX=5, sigmaY=5)
    distance = np.abs(smooth.astype(np.int16) - colour).max(axis=2)
    residual = np.abs(crop.astype(np.int16) - smooth.astype(np.int16)).max(axis=2)
    candidate = (distance > tolerance) & (residual <= edge_residual)
    count, labels, stats, _ = cv2.connectedComponentsWithStats(candidate.astype(np.uint8), 8)
    minimum = max(100, int((right - x) * (bottom - y) * MIN_COMPONENT_FRACTION))
    keep = np.zeros_like(candidate)
    for label in range(1, count):
        if stats[label, cv2.CC_STAT_AREA] >= minimum:
            keep[labels == label] = True
    claim = np.zeros((height, width), dtype=bool)
    claim[y:bottom, x:right] = keep
    return claim, colour


def _page_colour(image):
    height, width = image.shape[:2]
    reach = max(2, int(round(0.02 * min(height, width))))
    border = np.concatenate(
        [
            image[:reach].reshape(-1, 3),
            image[-reach:].reshape(-1, 3),
            image[:, :reach].reshape(-1, 3),
            image[:, -reach:].reshape(-1, 3),
        ]
    )
    return np.median(border, axis=0)


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
    extraction.data.pop("background", None)
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