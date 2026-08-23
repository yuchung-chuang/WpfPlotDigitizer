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
"""Find the rectangle the data is drawn in, twice over, and compare the two answers.

Writes the plot area section of the extraction and an overlay showing the box it chose, plus the
box it rejected whenever the two strategies disagreed.
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, rect
from pdkit.ink import clear_border, covered_runs, ink_mask, line_clusters, outline_mask, run_coverage

STAGE = "locate-plot-area"
PROBE_COVERAGE = 0.95
REFINE_WINDOW = 10
AGREEMENT_FRACTION = 0.02
LINE_SPAN_FRACTION = 0.5
MIN_AREA_FRACTION = 0.15
MAX_LINE_THICKNESS_FRACTION = 0.02
WHOLE_REGION_FRACTION = 0.99


def main() -> int:
    parser = base_parser("Locate the plot area with two independent strategies and compare them.")
    parser.add_argument(
        "--box",
        type=_box_argument,
        default=None,
        metavar="X,Y,WIDTH,HEIGHT",
        help="set the plot area yourself instead of detecting it",
    )
    parser.add_argument(
        "--tolerance",
        type=float,
        default=AGREEMENT_FRACTION,
        help="how far apart the two strategies may sit, as a fraction of the figure's smaller "
        "side, and still count as agreeing (default %(default)s)",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    height, width = image.shape[:2]

    region, source, panelled = _search_region(extraction, width, height)
    if args.box is not None:
        return _override(workdir, extraction, image, args.box, result)

    mask = outline_mask(image)
    probed, corners = _corner_probe(ink_mask(image), region, trim_border=not panelled)
    framed, lines = _line_frame(mask, region)
    probed = _plausible(probed, region, result, "corner probe")
    framed = _plausible(framed, region, result, "line frame")

    tolerance = args.tolerance * min(width, height)
    box, method, confidence, offset, rejected = _choose(probed, framed, tolerance, result)
    if box is None:
        result.fail("neither strategy found a plot area; supply one with --box")
        overlay = Overlay(image).rectangle(region, palette_colour(6), label="searched").save(
            workdir.overlay_path(STAGE)
        )
        lines_out = [f"search      {source}", "box         none found"]
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines_out, overlay)

    if corners < 4 and method != "line-frame":
        confidence *= {3: 0.85, 2: 0.7}.get(corners, 1.0)
        result.note(f"only {corners} of 4 corners probed; the frame is incomplete")

    result.confidence = round(min(confidence, 1.0), 3)
    extraction.set_plot_area(box, method, result.confidence)
    extraction.plot_area["candidates"] = {"corner_probe": probed, "line_frame": framed}

    overlay = _draw(image, box, method, rejected).save(workdir.overlay_path(STAGE))
    summary = [
        f"box         {_describe(box)}",
        f"method      {method}",
        f"corners     {corners} of 4 probed   {_describe(probed)}",
        f"lines       {lines}   {_describe(framed)}",
        f"agreement   {_agreement(probed, framed, tolerance, offset)}",
        f"search      {source}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _box_argument(text: str) -> dict:
    parts = text.replace(" ", "").split(",")
    if len(parts) != 4:
        raise argparse.ArgumentTypeError("expected X,Y,WIDTH,HEIGHT")
    try:
        x, y, width, height = (float(part) for part in parts)
    except ValueError as error:
        raise argparse.ArgumentTypeError("expected four numbers") from error
    if width <= 0 or height <= 0:
        raise argparse.ArgumentTypeError("width and height must be positive")
    return rect(x, y, width, height)


def _override(workdir, extraction, image, box: dict, result: Result) -> int:
    result.confidence = 1.0
    result.note("plot area supplied by the agent, not detected")
    extraction.set_plot_area(box, "override", result.confidence)
    extraction.plot_area["candidates"] = {"corner_probe": None, "line_frame": None}
    overlay = _draw(image, box, "override", None).save(workdir.overlay_path(STAGE))
    summary = [f"box         {_describe(box)}", "method      override"]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _search_region(extraction, width: int, height: int) -> tuple[dict, str, bool]:
    """The primary panel when a chart overview found one, the whole figure otherwise."""
    panels = extraction.data.get("chart", {}).get("panels") or []
    if not panels:
        return rect(0, 0, width, height), "whole figure - run inspect_chart.py first for a panel", False
    panel = panels[0]
    return panel, f"primary panel of {len(panels)}   {_describe(panel)}", True


def _corner_probe(mask: np.ndarray, region: dict, trim_border: bool) -> tuple[dict | None, int]:
    """Take the largest region of ink, then probe its corners for long axis-aligned runs.

    Ported from the desktop client's axis search. Each corner is accepted only where a long run
    heads away from it along both edges, so a broken frame loses that corner rather than the box.
    Reads plain ink: fill outlines would hand a dense scatter cluster enough coverage to pass.
    """
    field = _crop(mask, region)
    bounds = _largest_bounds(field)
    if bounds is None:
        return None, 0
    if trim_border and _fills(bounds, field.shape):
        bounds = _largest_bounds(clear_border(field)) or bounds

    left, top, right, bottom = bounds
    reach = max(8, min(right - left, bottom - top) // 2)
    probes = {
        name: run_coverage(field, name, reach, PROBE_COVERAGE)
        for name in ("right", "down", "left", "up")
    }

    rows_down = np.arange(top, min(top + reach, field.shape[0]))
    rows_up = np.arange(bottom - 1, max(bottom - 1 - reach, -1), -1)
    cols_right = np.arange(left, min(left + reach, field.shape[1]))
    cols_left = np.arange(right - 1, max(right - 1 - reach, -1), -1)

    top_left = _corner(probes["right"] & probes["down"], rows_down, cols_right)
    top_right = _corner(probes["left"] & probes["down"], rows_down, cols_left)
    bottom_left = _corner(probes["right"] & probes["up"], rows_up, cols_right)
    bottom_right = _corner(probes["left"] & probes["up"], rows_up, cols_left)

    found = [corner for corner in (top_left, top_right, bottom_left, bottom_right) if corner]
    if len(found) < 2:
        return None, len(found)

    x0 = _first(top_left, bottom_left, 0, left)
    y0 = _first(top_left, top_right, 1, top)
    x1 = _first(bottom_right, top_right, 0, right)
    y1 = _first(bottom_right, bottom_left, 1, bottom)
    if x1 <= x0 or y1 <= y0:
        return None, len(found)
    return rect(x0 + region["x"], y0 + region["y"], x1 - x0, y1 - y0), len(found)


def _largest_bounds(mask: np.ndarray) -> tuple[int, int, int, int] | None:
    contours, _ = cv2.findContours(
        (mask > 0).astype(np.uint8), cv2.RETR_LIST, cv2.CHAIN_APPROX_SIMPLE
    )
    if not contours:
        return None
    x, y, width, height = max((cv2.boundingRect(c) for c in contours), key=lambda b: b[2] * b[3])
    return x, y, x + width, y + height


def _fills(bounds: tuple[int, int, int, int], shape: tuple[int, ...]) -> bool:
    left, top, right, bottom = bounds
    return (right - left) > WHOLE_REGION_FRACTION * shape[1] and (
        bottom - top
    ) > WHOLE_REGION_FRACTION * shape[0]


def _corner(accepted: np.ndarray, rows: np.ndarray, cols: np.ndarray) -> tuple[float, float] | None:
    """First accepted position in the scan order, averaged over its immediate neighbourhood."""
    if rows.size == 0 or cols.size == 0:
        return None
    hits = np.argwhere(accepted[np.ix_(rows, cols)])
    if hits.size == 0:
        return None

    row, col = hits[0]
    near_rows = rows[row : row + REFINE_WINDOW]
    near_cols = cols[col : col + REFINE_WINDOW]
    ys, xs = np.nonzero(accepted[np.ix_(near_rows, near_cols)])
    return float(near_cols[xs].mean()), float(near_rows[ys].mean())


def _first(primary, secondary, axis: int, fallback: int) -> float:
    for corner in (primary, secondary):
        if corner is not None:
            return corner[axis]
    return float(fallback)


def _plausible(box: dict | None, region: dict, result: Result, name: str) -> dict | None:
    """Discard a box too small to be the plot area, so a colourbar or a dense streak cannot win."""
    if box is None:
        return None
    share = box["width"] * box["height"] / (region["width"] * region["height"])
    if share >= MIN_AREA_FRACTION:
        return box
    result.warn(
        f"the {name} box {_describe(box)} covers {share:.0%} of the searched region, "
        "too little to be a plot area; discarding it",
        region=_integers(box),
    )
    return None


def _line_frame(mask: np.ndarray, region: dict) -> tuple[dict | None, str]:
    """Take the outermost long axis-aligned lines and intersect them.

    Ported from the line-detection prototype, which edge-detected, kept only lines at 0 or 90
    degrees, and crossed them. Long unbroken runs give the same lines without the Hough accumulator
    and carry their extent, which is what tells a frame edge from a tick or a legend rule.
    """
    field = _crop(mask, region)
    height, width = field.shape
    vertical = _spanning(field, axis=0, span=int(LINE_SPAN_FRACTION * height), across=width)
    horizontal = _spanning(field, axis=1, span=int(LINE_SPAN_FRACTION * width), across=height)

    detail = f"{len(vertical)} vertical, {len(horizontal)} horizontal"
    if len(vertical) < 2 or len(horizontal) < 2:
        return None, detail + " - need two of each"

    left, right = vertical[0]["position"], vertical[-1]["position"]
    top, bottom = horizontal[0]["position"], horizontal[-1]["position"]
    return rect(left + region["x"], top + region["y"], right - left, bottom - top), detail


def _spanning(field: np.ndarray, axis: int, span: int, across: int) -> list[dict]:
    """Lines long enough to bound the region, and thin enough to be lines rather than data."""
    limit = max(6, int(MAX_LINE_THICKNESS_FRACTION * across))
    runs = covered_runs(field, axis, max(span, 8), PROBE_COVERAGE)
    clusters = line_clusters(runs, axis=axis)
    return sorted(
        (
            cluster
            for cluster in clusters
            if cluster["length"] >= PROBE_COVERAGE * span and cluster["thickness"] <= limit
        ),
        key=lambda cluster: cluster["position"],
    )


def _choose(
    probed: dict | None, framed: dict | None, tolerance: float, result: Result
) -> tuple[dict | None, str, float, float, dict | None]:
    if probed is None and framed is None:
        return None, "none", 0.0, float("nan"), None
    if framed is None:
        result.warn("no line frame found; only the corner probe answered")
        return probed, "corner-probe", 0.6, float("nan"), None
    if probed is None:
        result.warn("the corner probe found no complete corner; only the line frame answered")
        return framed, "line-frame", 0.55, float("nan"), None

    offset = _offset(probed, framed)
    if offset <= tolerance:
        result.note(f"both strategies agree to within {offset:.1f} px")
        return probed, "agreement", 0.9, offset, None
    result.warn(
        f"the two strategies disagree by {offset:.1f} px (tolerance {tolerance:.1f}): "
        f"corner probe {_describe(probed)}, line frame {_describe(framed)}. "
        "Using the corner probe - look at the overlay and override with --box if it is wrong.",
        region=_integers(framed),
    )
    return probed, "corner-probe", 0.4, offset, framed


def _offset(a: dict, b: dict) -> float:
    """How far apart two boxes are, as the worst disagreement over their four edges."""
    return max(
        abs(a["x"] - b["x"]),
        abs(a["y"] - b["y"]),
        abs(a["x"] + a["width"] - b["x"] - b["width"]),
        abs(a["y"] + a["height"] - b["y"] - b["height"]),
    )


def _agreement(probed: dict | None, framed: dict | None, tolerance: float, offset: float) -> str:
    if probed is None or framed is None:
        return "not comparable - one strategy had nothing to say"
    verdict = "agreed" if offset <= tolerance else "DISAGREED"
    return f"{verdict}, {offset:.1f} px apart (tolerance {tolerance:.1f})"


def _crop(mask: np.ndarray, region: dict) -> np.ndarray:
    x, y = int(region["x"]), int(region["y"])
    return mask[y : y + int(region["height"]), x : x + int(region["width"])]


def _describe(box: dict | None) -> str:
    if box is None:
        return "none"
    return (
        f"x={box['x']:.0f} y={box['y']:.0f} "
        f"w={box['width']:.0f} h={box['height']:.0f}"
    )


def _integers(box: dict) -> dict[str, int]:
    return {key: int(round(value)) for key, value in box.items()}


def _draw(image: np.ndarray, box: dict, method: str, rejected: dict | None) -> Overlay:
    overlay = Overlay(image)
    if rejected is not None:
        rejected_name = "corner probe" if method == "line-frame" else "line frame"
        overlay.rectangle(rejected, palette_colour(1), label="rejected", thickness=2)
        overlay.key(f"rejected: {rejected_name}", palette_colour(1))
    overlay.rectangle(box, palette_colour(0), label="plot area", thickness=3)
    return overlay.key(f"plot area: {method}", palette_colour(0))


if __name__ == "__main__":
    raise SystemExit(main())
