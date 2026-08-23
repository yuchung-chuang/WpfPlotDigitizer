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
"""Take the first look at a figure: how many panels, and is it in scope.

Writes the chart section of the extraction, a downscaled preview to read the figure from, and an
overlay marking the panels it found.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, rect
from pdkit.imageio import write_image
from pdkit.ink import has_tick_marks, ink_mask, line_clusters, long_runs, looks_like_grid

STAGE = "inspect-chart"
PREVIEW_MAX_EDGE = 768
MIN_PANEL_AREA_FRACTION = 0.06
FIGURE_BORDER_FRACTION = 0.9
FRAME_RUN_FRACTION = 0.06
AXIS_LINE_FRACTION = 0.5


def main() -> int:
    parser = base_parser("Classify a figure, count its panels, and decide whether it is in scope.")
    parser.add_argument(
        "--type",
        default=None,
        help="record the chart type you can see in the preview, e.g. scatter, line, bar",
    )
    parser.add_argument(
        "--declare",
        choices=("supported", "unsupported"),
        default=None,
        help="record your verdict after reading the preview",
    )
    parser.add_argument("--reason", default=None, help="why you declared it unsupported")
    parser.add_argument(
        "--min-panel-area",
        type=float,
        default=MIN_PANEL_AREA_FRACTION,
        help="smallest fraction of the figure a panel may occupy (default %(default)s)",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    height, width = image.shape[:2]

    ink = ink_mask(image)
    panels = _find_panels(ink, args.min_panel_area)
    if not panels:
        result.warn("no panel frame found; treating the whole figure as one panel")
        panels = [rect(0, 0, width, height)]

    axis_lines = _axis_lines(ink, panels[0], (height, width))
    supported, reason = _verdict(args, panels, result)
    result.confidence = 0.85 if supported is not None else 0.5

    extraction.data["chart"] = {
        "type": args.type,
        "panel_count": len(panels),
        "panels": panels,
        "supported": supported,
        "reason": reason,
        "axis_lines": axis_lines,
        "confidence": round(result.confidence, 3),
    }

    preview = _write_preview(workdir, image)
    overlay = _draw(image, panels).save(workdir.overlay_path(STAGE))

    verdict = {True: "yes", False: "no", None: "unknown - re-run with --declare"}[supported]
    lines = [
        f"panels      {len(panels)}",
        f"axis lines  {len(axis_lines['vertical'])} vertical, "
        f"{len(axis_lines['horizontal'])} horizontal (tick-bearing, of "
        f"{len(axis_lines['vertical_candidates'])} and "
        f"{len(axis_lines['horizontal_candidates'])} long lines)",
        f"supported   {verdict}" + (f" - {reason}" if reason else ""),
        f"type        {args.type or 'unset - pass --type once you have read the preview'}",
        f"preview     {workdir.display(preview)}   <-- read the figure here",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines, overlay)


def _find_panels(ink: np.ndarray, min_area_fraction: float) -> list[dict]:
    """A panel is a connected frame of long lines big enough to hold data."""
    height, width = ink.shape
    run = max(12, int(FRAME_RUN_FRACTION * min(height, width)))
    frame = long_runs(ink, axis=1, min_length=run) | long_runs(ink, axis=0, min_length=run)

    count, labels, stats, _ = cv2.connectedComponentsWithStats(
        frame.astype(np.uint8), connectivity=8
    )
    figure_area = height * width
    boxes = []
    for index in range(1, count):
        x, y, w, h, _ = stats[index]
        area = w * h
        if area < min_area_fraction * figure_area:
            continue
        if area > FIGURE_BORDER_FRACTION * figure_area:
            continue  # the figure's own border, not a panel
        boxes.append(rect(x, y, w, h))

    return _drop_nested(sorted(boxes, key=lambda b: -b["width"] * b["height"]))


def _drop_nested(boxes: list[dict]) -> list[dict]:
    """A frame inside another frame is a legend or an inset, not a panel."""
    kept: list[dict] = []
    for box in boxes:
        if not any(_contains(outer, box) for outer in kept):
            kept.append(box)
    return kept


def _contains(outer: dict, inner: dict) -> bool:
    return (
        outer["x"] <= inner["x"]
        and outer["y"] <= inner["y"]
        and outer["x"] + outer["width"] >= inner["x"] + inner["width"]
        and outer["y"] + outer["height"] >= inner["y"] + inner["height"]
    )


def _axis_lines(ink: np.ndarray, panel: dict, shape: tuple[int, int]) -> dict:
    """Long lines around the primary panel, and which of them carry tick marks.

    Tick-bearing lines are the ones that are really axes, but tick detection misses thin or
    inward-drawn ticks, so both counts are reported and the agent judges.
    """
    height, width = shape
    margin = 0.2
    left = max(0, int(panel["x"] - margin * panel["width"]))
    top = max(0, int(panel["y"] - margin * panel["height"]))
    right = min(width, int(panel["x"] + (1 + margin) * panel["width"]))
    bottom = min(height, int(panel["y"] + (1 + margin) * panel["height"]))
    region = ink[top:bottom, left:right]

    span_v = max(10, int(AXIS_LINE_FRACTION * panel["height"]))
    span_h = max(10, int(AXIS_LINE_FRACTION * panel["width"]))
    vertical = line_clusters(long_runs(region, 0, span_v), axis=0)
    horizontal = line_clusters(long_runs(region, 1, span_h), axis=1)

    return {
        "vertical": _ticked(region, vertical, axis=0, offset=left),
        "horizontal": _ticked(region, horizontal, axis=1, offset=top),
        "vertical_candidates": [round(c["position"] + left, 1) for c in vertical],
        "horizontal_candidates": [round(c["position"] + top, 1) for c in horizontal],
    }


def _ticked(region: np.ndarray, clusters: list[dict], axis: int, offset: int) -> list[float]:
    positions = [c["position"] for c in clusters]
    if looks_like_grid(positions):
        return []
    return [
        round(c["position"] + offset, 1)
        for c in clusters
        if has_tick_marks(region, c["position"], c["from"], c["to"], axis)
    ]


def _verdict(args, panels: list[dict], result: Result) -> tuple[bool | None, str | None]:
    if args.declare:
        supported = args.declare == "supported"
        result.note(f"verdict declared from the preview: {args.declare}")
        return supported, args.reason
    if len(panels) > 1:
        reason = f"{len(panels)} panels; only single-panel figures are supported"
        result.warn(reason)
        return False, reason
    result.warn(
        "read the preview, then re-run with --declare to record whether this figure has a single "
        "X and Y axis pair and no offset secondary axes"
    )
    return None, "awaiting a verdict from the preview"


def _write_preview(workdir, image: np.ndarray) -> Path:
    height, width = image.shape[:2]
    scale = min(1.0, PREVIEW_MAX_EDGE / max(height, width))
    preview = (
        image
        if scale >= 1.0
        else cv2.resize(image, (int(width * scale), int(height * scale)), interpolation=cv2.INTER_AREA)
    )
    path = workdir.crops / "preview.png"
    write_image(path, preview)
    return path


def _draw(image: np.ndarray, panels: list[dict]) -> Overlay:
    overlay = Overlay(image)
    for index, panel in enumerate(panels):
        overlay.rectangle(panel, palette_colour(index), label=f"panel {index + 1}")
    return overlay.key(f"{len(panels)} panel(s)", palette_colour(0))


if __name__ == "__main__":
    raise SystemExit(main())
