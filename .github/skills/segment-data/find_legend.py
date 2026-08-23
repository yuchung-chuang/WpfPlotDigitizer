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
"""Find a legend, crop each entry's swatch and label, and protect the legend region."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, rect
from pdkit.imageio import write_image
from pdkit.ink import ink_mask

STAGE = "find-legend"
MIN_WIDTH = 60
MIN_HEIGHT = 18
MIN_BORDER_COVERAGE = 0.55
MAX_PLOT_WIDTH_FRACTION = 0.8
MAX_PLOT_HEIGHT_FRACTION = 0.8
UPSCALE = 3.0
ENTRY_PADDING = 3
VERTICAL_SWATCH_WIDTH = 31
HORIZONTAL_SWATCH_WIDTH = 22
LABEL_GAP = 2


def main() -> int:
    parser = base_parser("Find a legend and crop its swatches and labels for inspection.")
    parser.add_argument(
        "--upscale",
        type=float,
        default=UPSCALE,
        help="how much to enlarge entry crops (default %(default)s)",
    )
    parser.add_argument(
        "--label",
        action="append",
        default=[],
        metavar="INDEX=TEXT",
        help="record the label read from an entry crop, repeatable",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    box = _find_box(image, extraction.plot_area)
    overlay = Overlay(image)
    if box is None:
        extraction.data["legend"] = {}
        result.confidence = 0.2
        result.note("no boxed legend found; continuing because legends are optional")
        overlay_path = overlay.save(workdir.overlay_path(STAGE))
        return finish(
            workdir,
            extraction,
            STAGE,
            Path(__file__).name,
            result,
            ["legend      none found; the figure may have no boxed legend"],
            overlay_path,
        )

    entries = _entries(image, box)
    if not entries:
        result.warn("legend box found, but no entry rows could be separated")
    crops = _write_crops(workdir, image, entries, args.upscale)
    labels = _labels(args.label, result)
    recorded = []
    for index, (swatch, label) in enumerate(entries):
        recorded.append(
            {
                "index": index,
                "swatch": swatch,
                "label_box": label,
                "label": labels.get(index),
            }
        )
        overlay.rectangle(swatch, palette_colour(index), thickness=1)
        overlay.rectangle(label, palette_colour(index), thickness=1)

    extraction.data["legend"] = {
        "box": box,
        "entries": recorded,
        "confidence": round(_confidence(box, entries, extraction.plot_area), 3),
    }
    extraction.protect("legend", box)
    result.confidence = extraction.data["legend"]["confidence"]
    overlay.rectangle(box, palette_colour(6), label="legend", thickness=2)
    overlay_path = overlay.save(workdir.overlay_path(STAGE))
    summary = [
        f"legend      {_describe(box)}",
        f"entries     {len(entries)}",
        f"crops       {workdir.relative(workdir.crops)}  (swatch and label per entry, {args.upscale:g}x)",
        "protected   legend region spared by noise filters",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay_path)


def _find_box(image: np.ndarray, plot_area: dict) -> dict | None:
    mask = ink_mask(image).astype(np.uint8)
    contours, _ = cv2.findContours(mask, cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE)
    height, width = mask.shape
    candidates = []
    for contour in contours:
        x, y, box_width, box_height = cv2.boundingRect(contour)
        if box_width < MIN_WIDTH or box_height < MIN_HEIGHT:
            continue
        if box_width >= 0.95 * width and box_height >= 0.8 * height:
            continue
        if _looks_like_plot(box_width, box_height, plot_area, mask.shape):
            continue
        box = rect(x, y, box_width, box_height)
        border = _border_coverage(mask, box)
        if min(border) < MIN_BORDER_COVERAGE:
            continue
        interior = mask[y + 3 : y + box_height - 3, x + 3 : x + box_width - 3]
        density = float(interior.mean()) if interior.size else 0.0
        candidates.append((min(border), density, box_width * box_height, box))

    candidates.sort(key=lambda candidate: (candidate[0], candidate[1], candidate[2]), reverse=True)
    if candidates:
        return candidates[0][3]
    return None


def _looks_like_plot(width: int, height: int, plot_area: dict, shape: tuple[int, int]) -> bool:
    if not all(key in plot_area for key in ("width", "height")):
        return width >= 0.8 * shape[1] and height >= 0.8 * shape[0]
    return width >= MAX_PLOT_WIDTH_FRACTION * plot_area["width"] and height >= MAX_PLOT_HEIGHT_FRACTION * plot_area["height"]


def _border_coverage(mask: np.ndarray, box: dict) -> tuple[float, float, float, float]:
    x, y = int(box["x"]), int(box["y"])
    width, height = int(box["width"]), int(box["height"])
    top_values = []
    bottom_values = []
    left_values = []
    right_values = []
    for offset in range(-2, 3):
        top = y + offset
        bottom = y + height - 1 + offset
        left = x + offset
        right = x + width - 1 + offset
        top_values.append(_line_coverage(mask, top, x, x + width, 0))
        bottom_values.append(_line_coverage(mask, bottom, x, x + width, 0))
        left_values.append(_line_coverage(mask, left, y, y + height, 1))
        right_values.append(_line_coverage(mask, right, y, y + height, 1))
    return max(top_values), max(bottom_values), max(left_values), max(right_values)


def _line_coverage(mask: np.ndarray, fixed: int, start: int, stop: int, axis: int) -> float:
    if axis == 0:
        if not 0 <= fixed < mask.shape[0]:
            return 0.0
        values = mask[fixed, max(0, start) : min(mask.shape[1], stop)]
    else:
        if not 0 <= fixed < mask.shape[1]:
            return 0.0
        values = mask[max(0, start) : min(mask.shape[0], stop), fixed]
    return float(values.mean()) if values.size else 0.0


def _entries(image: np.ndarray, box: dict) -> list[tuple[dict, dict]]:
    mask = ink_mask(image)
    x, y, width, height = (int(round(box[key])) for key in ("x", "y", "width", "height"))
    inner = mask[y + ENTRY_PADDING : y + height - ENTRY_PADDING, x + ENTRY_PADDING : x + width - ENTRY_PADDING]
    if inner.size == 0:
        return []
    joined = cv2.morphologyEx(
        inner.astype(np.uint8),
        cv2.MORPH_CLOSE,
        cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3)),
    )
    rows = _runs(joined.any(axis=1), gap=2)
    rows = [run for run in rows if run.size >= 6]
    if len(rows) == 1 and width / max(height, 1) >= 4:
        columns = cv2.morphologyEx(
            joined,
            cv2.MORPH_CLOSE,
            cv2.getStructuringElement(cv2.MORPH_RECT, (max(5, int(round(width * 0.015))), 1)),
        ).any(axis=0)
        columns = _runs(columns, gap=max(8, int(round(width * 0.025))))
        if len(columns) > 1:
            return [_entry_boxes(box, row, column) for column in columns for row in rows]
    return [_entry_boxes(box, row, None) for row in rows]


def _entry_boxes(box: dict, row: np.ndarray, column: np.ndarray | None) -> tuple[dict, dict]:
    x, y, width, height = (int(round(box[key])) for key in ("x", "y", "width", "height"))
    top, bottom = int(row[0]), int(row[-1]) + 1
    left = int(column[0]) if column is not None else 0
    right = int(column[-1]) + 1 if column is not None else width - 2 * ENTRY_PADDING
    entry_x = x + ENTRY_PADDING + left
    entry_width = max(1, right - left)
    entry_y = y + ENTRY_PADDING + top
    entry_height = max(1, bottom - top)
    swatch_width = min(
        VERTICAL_SWATCH_WIDTH if column is None else HORIZONTAL_SWATCH_WIDTH,
        max(1, entry_width // 2),
    )
    swatch = rect(entry_x, entry_y, swatch_width, entry_height)
    label_x = min(entry_x + swatch_width + LABEL_GAP, x + width - 1)
    label = rect(label_x, entry_y, max(1, entry_x + entry_width - label_x), entry_height)
    return swatch, label


def _write_crops(workdir, image: np.ndarray, entries: list[tuple[dict, dict]], upscale: float) -> list[Path]:
    paths = []
    for index, (swatch, label) in enumerate(entries):
        for kind, region in (("swatch", swatch), ("label", label)):
            crop = _crop(image, region)
            if upscale != 1 and crop.size:
                crop = cv2.resize(crop, None, fx=upscale, fy=upscale, interpolation=cv2.INTER_CUBIC)
            path = workdir.crops / f"legend-entry-{index}-{kind}.png"
            write_image(path, crop)
            paths.append(path)
    return paths


def _labels(values: list[str], result: Result) -> dict[int, str]:
    labels = {}
    for value in values:
        if "=" not in value:
            result.warn(f"ignored malformed legend label {value!r}; expected INDEX=TEXT")
            continue
        index_text, label = value.split("=", 1)
        try:
            labels[int(index_text)] = label.strip()
        except ValueError:
            result.warn(f"ignored malformed legend label {value!r}; index must be an integer")
    return labels


def _crop(image: np.ndarray, box: dict) -> np.ndarray:
    height, width = image.shape[:2]
    x = max(0, int(round(box["x"])))
    y = max(0, int(round(box["y"])))
    right = min(width, int(round(box["x"] + box["width"])))
    bottom = min(height, int(round(box["y"] + box["height"])))
    return image[y:bottom, x:right]


def _runs(values: np.ndarray, gap: int) -> list[np.ndarray]:
    indices = np.flatnonzero(values)
    if indices.size == 0:
        return []
    return [run for run in np.split(indices, np.flatnonzero(np.diff(indices) > gap) + 1) if run.size]


def _iou(a: dict, b: dict) -> float:
    left = max(a["x"], b["x"])
    top = max(a["y"], b["y"])
    right = min(a["x"] + a["width"], b["x"] + b["width"])
    bottom = min(a["y"] + a["height"], b["y"] + b["height"])
    overlap = max(0.0, right - left) * max(0.0, bottom - top)
    union = a["width"] * a["height"] + b["width"] * b["height"] - overlap
    return overlap / union if union else 0.0


def _confidence(box: dict, entries: list, plot_area: dict) -> float:
    score = 0.55 + min(len(entries), 8) * 0.04
    if plot_area and not _contains(plot_area, box):
        score += 0.08
    return min(score, 0.95)


def _contains(outer: dict, inner: dict) -> bool:
    return (
        outer["x"] <= inner["x"]
        and outer["y"] <= inner["y"]
        and outer["x"] + outer["width"] >= inner["x"] + inner["width"]
        and outer["y"] + outer["height"] >= inner["y"] + inner["height"]
    )


def _describe(box: dict) -> str:
    return f"x={box['x']:.0f} y={box['y']:.0f} w={box['width']:.0f} h={box['height']:.0f}"


if __name__ == "__main__":
    raise SystemExit(main())