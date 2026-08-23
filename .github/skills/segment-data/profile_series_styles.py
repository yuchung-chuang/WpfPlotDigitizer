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
"""Turn legend swatches into concrete colour, marker, and line-style profiles."""

from __future__ import annotations

import ast
import json
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace
from pdkit.ink import ink_mask

STAGE = "profile-series-styles"
MARKERS = ("circle", "square", "triangle", "diamond", "cross", "star")
MIN_MARKER_AREA = 5


def main() -> int:
    parser = base_parser("Profile each legend swatch into a series style signature.")
    parser.add_argument(
        "--override",
        action="append",
        default=[],
        metavar="INDEX.FIELD=VALUE",
        help="correct a profile field, for example --override 0.marker=triangle",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    entries = (extraction.data.get("legend") or {}).get("entries") or []
    if not entries:
        result.confidence = 0.2
        result.note("no legend entries to profile; clustering can still run without a legend")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["styles      none - no legend entries"])

    overrides = _overrides(args.override, result)
    profiles = []
    overlay = Overlay(image)
    for index, entry in enumerate(entries):
        swatch = entry.get("swatch") or {}
        profile = _profile(image, swatch)
        corrections = overrides.get(index, {})
        if corrections:
            profile.update(corrections)
            profile["style_corrections"] = corrections
            result.note(f"style {index} corrected by explicit override: {', '.join(sorted(corrections))}")
        entry["style"] = profile
        profiles.append(profile)
        _annotate(overlay, swatch, index, profile)

    extraction.data["style_profiles"] = profiles
    result.confidence = round(float(np.mean([profile["confidence"] for profile in profiles])), 3)
    overlay_path = overlay.save(workdir.overlay_path(STAGE))
    summary = [
        f"styles      {len(profiles)} legend entries profiled",
        "signatures  LAB/RGB colour, marker, fill, line style and width",
        f"corrections {sum(bool(overrides.get(index)) for index in range(len(entries)))} explicit",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay_path)


def _profile(image: np.ndarray, box: dict) -> dict:
    crop = _crop(image, box)
    if crop.size == 0:
        return _empty_profile()
    mask = ink_mask(crop)
    colour_lab, colour_rgb = _colour(crop, mask)
    line_mask = _line_mask(mask)
    marker_mask = _marker_region(mask, line_mask)
    marker, filled, marker_size, marker_confidence = _marker(marker_mask)
    line_style, line_width, line_confidence = _line(line_mask, mask)
    has_marker = marker != "none"
    has_line = line_style != "none"
    kind = "both" if has_marker and has_line else "point" if has_marker else "line" if has_line else "unknown"
    return {
        "colour_lab": [round(float(value), 2) for value in colour_lab],
        "colour_rgb": [int(value) for value in colour_rgb],
        "marker": marker,
        "filled": filled,
        "marker_size": marker_size,
        "line_style": line_style,
        "line_width": line_width,
        "kind": kind,
        "confidence": round(float(np.mean([marker_confidence, line_confidence])), 3),
    }


def _colour(crop: np.ndarray, mask: np.ndarray) -> tuple[np.ndarray, np.ndarray]:
    pixels = crop[mask]
    if pixels.size == 0:
        return np.zeros(3), np.zeros(3, dtype=int)
    bgr = np.median(pixels, axis=0)
    lab = cv2.cvtColor(np.uint8([[bgr]]), cv2.COLOR_BGR2LAB)[0, 0]
    return lab, bgr[::-1].astype(int)


def _line_mask(mask: np.ndarray) -> np.ndarray:
    height, width = mask.shape
    kernel_width = max(5, width // 2)
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (kernel_width, 1))
    return cv2.morphologyEx(mask.astype(np.uint8), cv2.MORPH_OPEN, kernel) > 0


def _marker_region(mask: np.ndarray, line_mask: np.ndarray) -> np.ndarray:
    without_line = mask & ~line_mask
    if without_line.any():
        return without_line
    return mask


def _marker(mask: np.ndarray) -> tuple[str, bool | None, int, float]:
    if not mask.any():
        return "none", None, 0, 0.25
    count, labels, stats, _ = cv2.connectedComponentsWithStats(mask.astype(np.uint8), connectivity=8)
    if count <= 1:
        return "none", None, 0, 0.25
    label = 1 + int(np.argmax(stats[1:, cv2.CC_STAT_AREA]))
    x, y, box_width, box_height, _ = stats[label]
    if stats[label, cv2.CC_STAT_AREA] < MIN_MARKER_AREA:
        return "none", None, 0, 0.5
    if x > 0.7 * mask.shape[1] and x + box_width >= mask.shape[1]:
        return "none", None, 0, 0.5
    clipped = labels[y : y + box_height, x : x + box_width] == label
    filled_mask = nd_fill(clipped)
    fill_ratio = float(clipped.mean())
    filled_ratio = float(clipped.sum() / max(filled_mask.sum(), 1))
    filled = filled_ratio > 0.62 or fill_ratio > 0.48
    if _is_cross(clipped):
        marker = "cross"
    else:
        contours, _ = cv2.findContours(clipped.astype(np.uint8), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
        contour = max(contours, key=cv2.contourArea, default=None)
        vertices = 0
        if contour is not None and len(contour) >= 3:
            perimeter = cv2.arcLength(contour, True)
            vertices = len(cv2.approxPolyDP(contour, 0.08 * perimeter, True)) if perimeter else 0
        if vertices == 3:
            marker = "triangle"
        elif vertices == 4:
            marker = "diamond" if _rotated_quad(contour) else "square"
        elif vertices >= 8 and _star_like(clipped):
            marker = "star"
        else:
            marker = "circle"
    size = int(round((box_width + box_height) / 2))
    return marker, bool(filled), size, 0.75 if marker in MARKERS else 0.4


def _line(line_mask: np.ndarray, mask: np.ndarray) -> tuple[str, int, float]:
    if not line_mask.any():
        return "none", 0, 0.7
    rows = line_mask.sum(axis=1)
    row = int(np.argmax(rows))
    columns = line_mask[max(0, row - 1) : min(line_mask.shape[0], row + 2)].any(axis=0)
    occupied = np.flatnonzero(columns)
    if occupied.size < max(4, line_mask.shape[1] // 4):
        return "none", 0, 0.5
    runs = _runs(occupied)
    gaps = np.diff([run[-1] for run in runs]) - 1 if len(runs) > 1 else np.array([])
    coverage = occupied.size / max(line_mask.shape[1], 1)
    if coverage > 0.78 and not gaps.size:
        style = "solid"
    elif len(runs) >= 4 and np.median(gaps) <= 2:
        style = "dotted"
    elif len(runs) >= 2 and np.median(gaps) >= 5:
        style = "dash-dot" if len(runs) % 2 else "dashed"
    else:
        style = "dashed"
    thickness = int(round(np.median(np.flatnonzero(line_mask[:, occupied].any(axis=1)).size)))
    return style, max(1, thickness), 0.7


def _is_cross(mask: np.ndarray) -> bool:
    height, width = mask.shape
    horizontal = mask.sum(axis=1).max() >= 0.65 * width
    vertical = mask.sum(axis=0).max() >= 0.65 * height
    return horizontal and vertical and float(mask.mean()) < 0.45


def _rotated_quad(contour: np.ndarray | None) -> bool:
    if contour is None:
        return False
    rectangle = cv2.minAreaRect(contour)
    angle = abs(float(rectangle[2]))
    return 20 < angle < 70


def _star_like(mask: np.ndarray) -> bool:
    centre = np.array(np.nonzero(mask)).mean(axis=1)
    points = np.column_stack(np.nonzero(mask))
    distances = np.linalg.norm(points - centre, axis=1)
    return distances.size > 6 and float(distances.std() / max(distances.mean(), 1)) > 0.35


def nd_fill(mask: np.ndarray) -> np.ndarray:
    flooded = cv2.floodFill((~mask).astype(np.uint8), None, (0, 0), 2)[1]
    return flooded != 2


def _overrides(values: list[str], result: Result) -> dict[int, dict[str, object]]:
    corrections: dict[int, dict[str, object]] = {}
    for value in values:
        if "=" not in value or "." not in value.split("=", 1)[0]:
            result.warn(f"ignored malformed style override {value!r}; expected INDEX.FIELD=VALUE")
            continue
        target, raw = value.split("=", 1)
        index_text, field = target.split(".", 1)
        try:
            index = int(index_text)
            corrections.setdefault(index, {})[field] = _literal(raw)
        except ValueError:
            result.warn(f"ignored malformed style override {value!r}; index must be an integer")
    return corrections


def _literal(value: str) -> object:
    try:
        return ast.literal_eval(value)
    except (SyntaxError, ValueError):
        return value


def _empty_profile() -> dict:
    return {
        "colour_lab": [0.0, 0.0, 0.0],
        "colour_rgb": [0, 0, 0],
        "marker": "none",
        "filled": None,
        "marker_size": 0,
        "line_style": "none",
        "line_width": 0,
        "kind": "unknown",
        "confidence": 0.0,
    }


def _runs(indices: np.ndarray) -> list[np.ndarray]:
    if indices.size == 0:
        return []
    return np.split(indices, np.flatnonzero(np.diff(indices) > 1) + 1)


def _crop(image: np.ndarray, box: dict) -> np.ndarray:
    height, width = image.shape[:2]
    x = max(0, int(round(box.get("x", 0))))
    y = max(0, int(round(box.get("y", 0))))
    right = min(width, int(round(box.get("x", 0) + box.get("width", 0))))
    bottom = min(height, int(round(box.get("y", 0) + box.get("height", 0))))
    return image[y:bottom, x:right]


def _annotate(overlay: Overlay, box: dict, index: int, profile: dict) -> None:
    overlay.rectangle(box, (40, 180, 40), thickness=1)
    text = f"{index}: {profile['marker']} / {profile['line_style']}"
    x = int(round(box["x"] + box["width"] + 4))
    y = max(12, int(round(box["y"] + box["height"] - 2)))
    cv2.putText(overlay.canvas, text, (x, y), cv2.FONT_HERSHEY_SIMPLEX, 0.35, (40, 120, 40), 1)


if __name__ == "__main__":
    raise SystemExit(main())