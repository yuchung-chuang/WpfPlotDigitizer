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
"""Claim in-plot text and nearby arrow leaders as an annotation mask layer."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.masks import load_mask, rebuild_combined

STAGE = "remove-annotations"
LAYER = "annotations"
DEFAULT_JOIN = 5
DEFAULT_MIN_WIDTH = 12
ARROW_REACH = 24


def main() -> int:
    parser = base_parser("Remove text and arrow annotations drawn inside the plot area.")
    parser.add_argument("--join", type=int, default=DEFAULT_JOIN, help="horizontal text closing width")
    parser.add_argument("--min-width", type=int, default=DEFAULT_MIN_WIDTH, help="smallest text box width")
    parser.add_argument("--drop", action="store_true", help="drop this layer and rebuild the combined mask")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    if args.drop:
        return _drop(workdir, extraction, image, result)
    area = extraction.data.get("plot_area") or {}
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["annotations none"])

    claim, boxes = _text_claim(image, area, args.join, args.min_width)
    arrows = _arrow_claim(image, area, boxes)
    claim |= arrows
    claim = _exclude_existing(workdir, extraction, claim)
    claim = _spare(claim, extraction)
    path = workdir.masks / f"{LAYER}.png"
    pixels = save_mask(path, claim)
    extraction.add_mask_layer(LAYER, "noise", workdir.relative(path), pixels)
    extraction.data["annotations"] = {
        "regions": boxes,
        "arrows": int(np.count_nonzero(arrows)),
    }
    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    result.confidence = 0.7 if boxes or arrows.any() else 0.3
    if boxes:
        result.note(f"claimed {len(boxes)} text region(s)")
    if arrows.any():
        result.note(f"claimed {int(np.count_nonzero(arrows))} arrow-leader pixels")
    if not boxes and not arrows.any():
        result.note("no in-plot text or arrow annotations found; the layer claims nothing")
    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"text        {len(boxes)} region(s)",
        f"arrows      {int(np.count_nonzero(arrows))} px",
        f"layer       {pixels} px   {workdir.relative(path)}",
        f"combined    {combined} px over {len(extraction.mask_layers('noise'))} noise layers",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _text_claim(image, area, join, min_width):
    height, width = image.shape[:2]
    x, y = max(0, int(round(area["x"]))), max(0, int(round(area["y"])))
    right, bottom = min(width, int(round(area["x"] + area["width"]))), min(height, int(round(area["y"] + area["height"])))
    crop = image[y:bottom, x:right]
    grey = cv2.cvtColor(crop, cv2.COLOR_BGR2GRAY)
    gradient = cv2.morphologyEx(grey, cv2.MORPH_GRADIENT, np.ones((3, 3), np.uint8))
    _, binary = cv2.threshold(gradient, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)
    closed = cv2.morphologyEx(binary, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_RECT, (max(3, join), 1)))
    contours, _ = cv2.findContours(closed, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    claim = np.zeros((height, width), dtype=bool)
    boxes = []
    for contour in contours:
        box_x, box_y, box_width, box_height = cv2.boundingRect(contour)
        if box_width < min_width or box_height < 5:
            continue
        if box_height > 45:
            continue
        if box_width < 35 and box_height < 35:
            continue
        if box_width > 0.3 * (right - x) and box_height > 0.1 * (bottom - y):
            continue
        if box_width > 0.8 * (right - x) and box_height < 0.1 * (bottom - y):
            continue
        if box_height > 0.35 * (bottom - y) and box_width < 0.08 * (right - x):
            continue
        region = {"x": box_x + x, "y": box_y + y, "width": box_width, "height": box_height}
        boxes.append(region)
        claim[region["y"] : region["y"] + region["height"], region["x"] : region["x"] + region["width"]] |= closed[box_y : box_y + box_height, box_x : box_x + box_width] > 0
    return claim, boxes


def _arrow_claim(image, area, boxes):
    height, width = image.shape[:2]
    x, y = max(0, int(round(area["x"]))), max(0, int(round(area["y"])))
    right, bottom = min(width, int(round(area["x"] + area["width"]))), min(height, int(round(area["y"] + area["height"])))
    crop = image[y:bottom, x:right]
    edges = cv2.Canny(cv2.cvtColor(crop, cv2.COLOR_BGR2GRAY), 40, 120)
    lines = cv2.HoughLinesP(edges, 1, np.pi / 180, threshold=12, minLineLength=12, maxLineGap=4)
    claim = np.zeros((height, width), dtype=bool)
    segments = [np.asarray(line).reshape(-1)[:4] for line in lines] if lines is not None else []
    for index, (x0, y0, x1, y1) in enumerate(segments):
        length = float(np.hypot(x1 - x0, y1 - y0))
        if length < 18 or length > 0.7 * max(right - x, bottom - y):
            continue
        if min(abs(x1 - x0), abs(y1 - y0)) > 0.25 * max(abs(x1 - x0), abs(y1 - y0), 1):
            continue
        points = ((x0 + x, y0 + y), (x1 + x, y1 + y))
        if _has_arrowhead(index, segments) and any(
            _near_box(point, box, ARROW_REACH) for point in points for box in boxes
        ):
            cv2.line(claim, points[0], points[1], True, 2)
    return claim


def _has_arrowhead(index, segments):
    main = segments[index]
    main_start = np.array(main[:2], dtype=float)
    main_end = np.array(main[2:], dtype=float)
    main_vector = main_end - main_start
    main_length = np.linalg.norm(main_vector)
    if not main_length:
        return False
    for endpoint_index, endpoint in enumerate((main_start, main_end)):
        branches = []
        for other_index, other in enumerate(segments):
            if other_index == index:
                continue
            other_start = np.array(other[:2], dtype=float)
            other_end = np.array(other[2:], dtype=float)
            nearest = other_start if np.linalg.norm(endpoint - other_start) <= np.linalg.norm(endpoint - other_end) else other_end
            if np.linalg.norm(endpoint - nearest) > 7:
                continue
            vector = (other_end - other_start) if np.array_equal(nearest, other_start) else (other_start - other_end)
            other_length = np.linalg.norm(vector)
            if not 4 <= other_length <= 16:
                continue
            cosine = np.clip(np.dot(main_vector, vector) / (main_length * other_length), -1, 1)
            angle = np.degrees(np.arccos(cosine))
            cross = main_vector[0] * vector[1] - main_vector[1] * vector[0]
            if 25 <= angle <= 70:
                branches.append(np.sign(cross))
        if len(branches) >= 2 and len(set(branches)) > 1:
            return True
    return False


def _near_box(point, box, reach):
    return box["x"] - reach <= point[0] <= box["x"] + box["width"] + reach and box["y"] - reach <= point[1] <= box["y"] + box["height"] + reach


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
    extraction.data.pop("annotations", None)
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