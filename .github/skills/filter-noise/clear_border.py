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
"""Claim the plot frame and its tick marks into a mask layer of their own.

Writes `masks/border.png`, records it in the extraction, rebuilds the combined mask by union, and
draws an overlay tinting every noise layer in its own colour. Nothing else is touched: the figure
is never modified, and the other layers are neither read nor rewritten.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import ink_mask
from pdkit.masks import load_mask, rebuild_combined

STAGE = "remove-border"
LAYER = "border"
KIND = "noise"
TOUCH = 3
TICK_REACH = 12
EDGE_COVERAGE = 0.6
MAX_TICK_THICKNESS = 4
EDGE_CONFIDENCE = {4: 0.9, 3: 0.75, 2: 0.6, 1: 0.4, 0: 0.2}


def main() -> int:
    parser = base_parser("Claim the plot frame and its tick marks into their own mask layer.")
    parser.add_argument(
        "--touch",
        type=int,
        default=TOUCH,
        help="how far from a plot-area edge ink still counts as lying on the boundary, in pixels "
        "(default %(default)s)",
    )
    parser.add_argument(
        "--tick-reach",
        type=int,
        default=TICK_REACH,
        help="longest tick mark to strip, in pixels, and how far outside the plot area this filter "
        "looks for one (default %(default)s)",
    )
    parser.add_argument(
        "--coverage",
        type=float,
        default=EDGE_COVERAGE,
        help="fraction of an edge a line must cover before it counts as the frame "
        "(default %(default)s)",
    )
    parser.add_argument(
        "--drop",
        action="store_true",
        help="delete this filter's layer, rebuild the combined mask, and stop",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    shape = image.shape[:2]

    if args.drop:
        return _drop(workdir, extraction, image, result)

    area = extraction.data.get("plot_area") or {}
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail(
            "no plot area recorded, so there is no boundary to clear; run "
            "analyse-axis/locate_plot_area.py first, or set one with its --box"
        )
        return finish(
            workdir, extraction, STAGE, Path(__file__).name, result, ["plot area   none recorded"]
        )

    window = _window(area, shape, args.tick_reach)
    field = _crop(ink_mask(image), window)
    frame, edges = _frame_lines(field, area, window, args.touch, args.coverage)
    ticks, stubs = _tick_stubs(field, frame, _edge_offsets(area, window), args.tick_reach)

    claim = np.zeros(shape, dtype=bool)
    _paste(claim, frame | ticks, window)
    spared = _spare_protected(claim, extraction)

    result.confidence = EDGE_CONFIDENCE[len(edges)]
    if not edges:
        result.warn(
            "no frame found on any edge of the plot area; the layer claims nothing. Either the "
            "panel is drawn without a frame, or the plot area is wrong - look at the overlay"
        )
    elif len(edges) < 4:
        missing = [name for name in ("left", "right", "top", "bottom") if name not in edges]
        result.note(f"frame found on {len(edges)} of 4 edges; nothing on {', '.join(missing)}")

    mask_path = workdir.masks / f"{LAYER}.png"
    pixels = save_mask(mask_path, claim)
    extraction.add_mask_layer(LAYER, KIND, workdir.relative(mask_path), pixels)
    combined = rebuild_combined(workdir, extraction, shape)

    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"plot area   {_describe(area)}",
        f"edges       {len(edges)} of 4   {', '.join(edges) if edges else 'none'}",
        f"frame       {int(np.count_nonzero(frame))} px",
        f"ticks       {int(np.count_nonzero(ticks))} px in {stubs} stubs",
        f"protected   {spared} px spared across "
        f"{len(extraction.data['protected_regions'])} regions",
        f"layer       {LAYER}   {pixels} px   {workdir.relative(mask_path)}",
        f"combined    {combined} px over {len(extraction.mask_layers(KIND))} noise layers   "
        f"{workdir.relative(workdir.combined_mask_path)}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _drop(workdir, extraction, image: np.ndarray, result: Result) -> int:
    """Remove this filter's own layer and re-derive the combined mask from whatever remains."""
    mask_path = workdir.masks / f"{LAYER}.png"
    mask_path.unlink(missing_ok=True)
    layers = [layer for layer in extraction.data["mask_layers"] if layer.get("name") != LAYER]
    dropped = len(extraction.data["mask_layers"]) - len(layers)
    extraction.data["mask_layers"] = layers

    combined = rebuild_combined(workdir, extraction, image.shape[:2])
    result.confidence = 1.0
    if not dropped:
        result.note(f"there was no '{LAYER}' layer to drop; the combined mask was rebuilt anyway")

    overlay = _draw(image, workdir, extraction).save(workdir.overlay_path(STAGE))
    summary = [
        f"dropped     {LAYER}" if dropped else f"dropped     nothing - no '{LAYER}' layer existed",
        f"combined    {combined} px over {len(extraction.mask_layers(KIND))} noise layers   "
        f"{workdir.relative(workdir.combined_mask_path)}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _window(area: dict, shape: tuple[int, int], reach: int) -> dict[str, int]:
    """The plot area, grown by the tick reach.

    Tick marks are part of the border and are usually drawn outside the plot area, so refusing to
    look past the boundary would leave half of them behind. This is the only ink claimed from
    outside the plot area, and it is bounded by `--tick-reach`.
    """
    height, width = shape
    x = max(int(round(area["x"])) - reach, 0)
    y = max(int(round(area["y"])) - reach, 0)
    right = min(int(round(area["x"] + area["width"])) + reach, width)
    bottom = min(int(round(area["y"] + area["height"])) + reach, height)
    return {"x": x, "y": y, "width": max(right - x, 1), "height": max(bottom - y, 1)}


def _crop(mask: np.ndarray, window: dict) -> np.ndarray:
    x, y = window["x"], window["y"]
    return mask[y : y + window["height"], x : x + window["width"]]


def _paste(target: np.ndarray, claim: np.ndarray, window: dict) -> None:
    x, y = window["x"], window["y"]
    target[y : y + window["height"], x : x + window["width"]] |= claim


def _frame_lines(
    field: np.ndarray, area: dict, window: dict, touch: float, coverage: float
) -> tuple[np.ndarray, list[str]]:
    """The frame: a thin, heavily inked run of rows or columns lying on a plot-area edge.

    Only the ink on those rows is claimed, never the whole connected component. The desktop
    client's ClearBorder flood-fills away every component touching the boundary, which also
    swallows any series that happens to reach the frame; anchoring on the line instead lets a data
    point sitting on the edge keep everything except the pixels it shares with the frame.
    """
    edges = _edge_offsets(area, window)
    frame = np.zeros(field.shape, dtype=bool)
    found: list[str] = []

    for name, position in edges.items():
        vertical = name in ("left", "right")
        lines = field.T if vertical else field
        low, high = (edges["top"], edges["bottom"]) if vertical else (edges["left"], edges["right"])
        low, high = max(low, 0), min(high, lines.shape[1])
        if high - low < 4:
            continue

        share = lines[:, low:high].sum(axis=1) / (high - low)
        band = range(max(position - touch, 0), min(position + touch + 1, lines.shape[0]))
        candidates = [index for index in band if share[index] >= coverage]
        if not candidates:
            continue

        run = _run_around(candidates, max(candidates, key=lambda index: share[index]))
        claimed = np.zeros(lines.shape, dtype=bool)
        claimed[run, :] = True
        frame |= (claimed & lines).T if vertical else (claimed & lines)
        found.append(name)

    return frame, found


def _edge_offsets(area: dict, window: dict) -> dict[str, int]:
    """Where each plot-area edge sits inside the cropped window, in scan order per orientation."""
    return {
        "left": int(round(area["x"])) - window["x"],
        "right": int(round(area["x"] + area["width"])) - window["x"],
        "top": int(round(area["y"])) - window["y"],
        "bottom": int(round(area["y"] + area["height"])) - window["y"],
    }


def _run_around(candidates: list[int], best: int) -> list[int]:
    """The contiguous group of inked lines containing the best one.

    Thickness is bounded by `--touch` rather than by a limit of its own: a run that stopped short
    of the line's own anti-aliased edge would leave a sliver of frame behind, and that sliver welds
    every tick mark into one component too big to recognise.
    """
    accepted = set(candidates)
    run = [best]
    for step in (-1, 1):
        position = best + step
        while position in accepted:
            run.append(position)
            position += step
    return sorted(run)


def _tick_stubs(
    field: np.ndarray, frame: np.ndarray, edges: dict[str, int], reach: int
) -> tuple[np.ndarray, int]:
    """Short ink attached to the frame - the tick marks, pointing inward or outward.

    This is the flood fill, seeded from the frame rather than from the figure's boundary, and
    stopped by two tests. Nothing longer than `--tick-reach` is a tick. Of what is left, a stub
    lying wholly outside the plot area is a tick whatever its shape, because data is never drawn
    there; a stub inside the plot area must also be thin, which a data marker resting against the
    frame is not.
    """
    empty = np.zeros(field.shape, dtype=bool)
    rest = field & ~frame
    if not rest.any() or not frame.any():
        return empty, 0

    count, labels, stats, _ = cv2.connectedComponentsWithStats(rest.astype(np.uint8), 8)
    neighbourhood = cv2.dilate(frame.astype(np.uint8), np.ones((3, 3), np.uint8)) > 0
    attached = np.unique(labels[neighbourhood])

    keep = np.zeros(count, dtype=bool)
    for index in attached[attached > 0]:
        x, y, width, height, _ = stats[index]
        if max(width, height) > reach:
            continue
        outside = (
            x + width <= edges["left"]
            or x >= edges["right"]
            or y + height <= edges["top"]
            or y >= edges["bottom"]
        )
        if outside or min(width, height) <= MAX_TICK_THICKNESS:
            keep[index] = True
    return keep[labels], int(np.count_nonzero(keep))


def _spare_protected(claim: np.ndarray, extraction) -> int:
    """Give back every claimed pixel that falls inside a protected region."""
    spared = 0
    height, width = claim.shape
    for region in extraction.data["protected_regions"]:
        x = max(int(round(region["x"])), 0)
        y = max(int(round(region["y"])), 0)
        right = min(int(round(region["x"] + region["width"])), width)
        bottom = min(int(round(region["y"] + region["height"])), height)
        if right <= x or bottom <= y:
            continue
        inside = claim[y:bottom, x:right]
        spared += int(np.count_nonzero(inside))
        inside[:] = False
    return spared


def _draw(image: np.ndarray, workdir, extraction) -> Overlay:
    """Every noise layer in a colour of its own, keyed to its position in `mask_layers`.

    Same layer, same colour on every run, so a filter that suddenly eats more than it used to is
    visible at a glance rather than merely suspected.
    """
    overlay = Overlay(image)
    for index, layer in enumerate(extraction.data["mask_layers"]):
        if layer.get("kind") != KIND:
            continue
        path = workdir.path / layer["file"]
        if not path.exists():
            continue
        label = f"{layer['name']}  {layer['pixels']} px"
        overlay.tint(load_mask(path), palette_colour(index), label=label)
    for region in extraction.data["protected_regions"]:
        overlay.rectangle(region, palette_colour(6), label=f"protected: {region.get('name', '?')}")
    return overlay


def _describe(box: dict) -> str:
    return (
        f"x={box['x']:.0f} y={box['y']:.0f} w={box['width']:.0f} h={box['height']:.0f}"
    )


if __name__ == "__main__":
    raise SystemExit(main())
