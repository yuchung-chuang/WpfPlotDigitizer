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
"""Trace one line-series mask into an ordered, sampled pixel polyline."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np
from skimage.morphology import skeletonize

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour
from pdkit.masks import load_mask

STAGE = "extract-line-series"
DEFAULT_STEP = 2.0
DEFAULT_GAP = 3


def main() -> int:
    parser = base_parser("Trace a segmented line mask into an ordered pixel polyline.")
    parser.add_argument("--series-id", default="s1", help="series id to trace")
    parser.add_argument("--step", type=float, default=DEFAULT_STEP, help="sampling distance in pixels")
    parser.add_argument("--gap", type=int, default=DEFAULT_GAP, help="closing radius used to bridge gaps")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    area = extraction.data.get("plot_area") or {}
    entry = next((item for item in extraction.data["series"] if item.get("id") == args.series_id), None)
    if entry is None:
        result.fail(f"series {args.series_id} is not recorded; run segment_series.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["line trace skipped"])
    reference = entry.get("mask")
    if not reference:
        result.fail(f"series {args.series_id} has no mask to trace")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["line trace skipped"])
    mask_path = workdir.path / reference
    if not mask_path.exists():
        result.fail(f"series {args.series_id} mask is missing: {reference}")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["line trace skipped"])
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["line trace skipped"])

    region = _region(area, image.shape[:2])
    raw = _crop(load_mask(mask_path), region)
    raw = _clear_internal_axes(raw, extraction, region)
    bridged = _bridge(raw, args.gap)
    skeleton = skeletonize(bridged)
    path, junctions = _trace(skeleton)
    sampled = _sample(path, args.step)
    if len(sampled) < 2:
        result.fail(f"series {args.series_id} did not produce an ordered line")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["line trace skipped"])
    unvisited = _unvisited_points(skeleton, path)
    points = [
        (float(x + region["x"]), float(y + region["y"]))
        for x, y in sampled + _sample(unvisited, args.step)
    ]
    sidecar = workdir.points / f"{args.series_id}-line-pixel.csv"
    _write_points(sidecar, points)
    support_sidecar = workdir.points / f"{args.series_id}-support-pixel.csv"
    _write_points(
        support_sidecar,
        [(float(x + region["x"]), float(y + region["y"])) for y, x in np.column_stack(np.nonzero(raw))],
    )
    entry.update(
        {
            "kind": "line",
            "points_pixel": workdir.relative(sidecar),
            "support_points_pixel": workdir.relative(support_sidecar),
            "point_count": len(points),
            "support_point_count": int(raw.sum()),
        }
    )
    extraction.data.setdefault("line_traces", {})[args.series_id] = {
        "junctions": len(junctions),
        "sample_step": args.step,
        "points": len(points),
        "unvisited": len(unvisited),
    }
    if junctions:
        for x, y in junctions:
            result.warn(
                f"line trace crossed a junction; continued by direction at ({x + region['x']:.0f}, {y + region['y']:.0f})",
                region={"x": int(x + region["x"]), "y": int(y + region["y"]), "width": 1, "height": 1},
            )
    result.confidence = 0.75 if not junctions else 0.55
    overlay = Overlay(image)
    ordered_points = [(float(x + region["x"]), float(y + region["y"])) for x, y in sampled]
    overlay.polyline(ordered_points, palette_colour(0), label=f"{args.series_id} ordered")
    overlay.points(ordered_points[:: max(1, len(ordered_points) // 20)], palette_colour(0), size=4)
    overlay_path = overlay.save(workdir.overlay_path(STAGE))
    summary = [
        f"series      {args.series_id}  {len(points)} ordered points",
        f"coverage    {len(unvisited)} unvisited skeleton points appended for branch coverage",
        f"junctions   {len(junctions)} direction-aware decisions",
        f"sampling    {args.step:g} px, gap bridge radius {args.gap} px",
        f"sidecar     {workdir.relative(sidecar)}",
        f"support     {workdir.relative(support_sidecar)}  {int(raw.sum())} cleaned ink pixels",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay_path)


def _region(area, shape):
    height, width = shape
    x = max(0, int(round(area["x"])))
    y = max(0, int(round(area["y"])))
    right = min(width, int(round(area["x"] + area["width"])))
    bottom = min(height, int(round(area["y"] + area["height"])))
    return {"x": x, "y": y, "width": max(1, right - x), "height": max(1, bottom - y)}


def _crop(mask, region):
    return mask[region["y"] : region["y"] + region["height"], region["x"] : region["x"] + region["width"]]


def _clear_internal_axes(mask, extraction, region):
    chart_axes = extraction.data.get("chart", {}).get("axis_lines", {})
    cleaned = mask.copy()
    for position in chart_axes.get("vertical", []) + chart_axes.get("vertical_candidates", []):
        local = int(round(position)) - region["x"]
        if 2 <= local < region["width"] - 2:
            cleaned[:, max(0, local - 2) : min(region["width"], local + 3)] = False
    for position in chart_axes.get("horizontal", []) + chart_axes.get("horizontal_candidates", []):
        local = int(round(position)) - region["y"]
        if 2 <= local < region["height"] - 2:
            cleaned[max(0, local - 2) : min(region["height"], local + 3), :] = False
    return cleaned


def _bridge(mask, radius):
    if radius < 1:
        return mask
    size = 2 * radius + 1
    return cv2.morphologyEx(mask.astype(np.uint8), cv2.MORPH_CLOSE, np.ones((size, size), np.uint8)) > 0


def _trace(skeleton):
    points = np.column_stack(np.nonzero(skeleton))
    if not len(points):
        return [], []
    coordinates = {tuple(point): index for index, point in enumerate(points)}
    neighbours = []
    for y, x in points:
        adjacent = []
        for dy in (-1, 0, 1):
            for dx in (-1, 0, 1):
                if not dx and not dy:
                    continue
                index = coordinates.get((y + dy, x + dx))
                if index is not None:
                    adjacent.append(index)
        neighbours.append(adjacent)
    endpoints = [index for index, adjacent in enumerate(neighbours) if len(adjacent) == 1]
    start = endpoints[0] if endpoints else int(np.argmin(points[:, 0] + points[:, 1]))
    path = [start]
    junctions = []
    used_edges = set()
    previous = None
    current = start
    while True:
        choices = []
        for candidate in neighbours[current]:
            edge = tuple(sorted((current, candidate)))
            if edge in used_edges:
                continue
            vector = points[candidate].astype(float) - points[current].astype(float)
            choices.append((candidate, edge, vector))
        if not choices:
            break
        if previous is None:
            choice = choices[0]
        else:
            direction = points[current].astype(float) - points[previous].astype(float)
            direction /= max(np.linalg.norm(direction), 1.0)
            choice = max(
                choices,
                key=lambda item: float(np.dot(direction, item[2] / max(np.linalg.norm(item[2]), 1.0))),
            )
        if len(choices) > 1:
            junctions.append(tuple(points[current][::-1]))
        candidate, edge, _ = choice
        used_edges.add(edge)
        previous, current = current, candidate
        path.append(current)
        if len(path) > len(points) + 1:
            break
    return [tuple(points[index][::-1]) for index in path], junctions


def _sample(path, step):
    if len(path) < 2:
        return path
    step = max(float(step), 0.25)
    sampled = [np.asarray(path[0], dtype=float)]
    distance_since = 0.0
    for point in path[1:]:
        current = np.asarray(point, dtype=float)
        distance_since += float(np.linalg.norm(current - sampled[-1]))
        if distance_since >= step:
            sampled.append(current)
            distance_since = 0.0
    if not np.array_equal(sampled[-1], path[-1]):
        sampled.append(np.asarray(path[-1], dtype=float))
    return [tuple(point) for point in sampled]


def _unvisited_points(skeleton, path):
    visited = {(int(round(x)), int(round(y))) for x, y in path}
    return [
        (int(x), int(y))
        for y, x in np.column_stack(np.nonzero(skeleton))
        if (int(x), int(y)) not in visited
    ]


def _write_points(path, points):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("x,y\n" + "\n".join(f"{x:.3f},{y:.3f}" for x, y in points) + "\n", encoding="utf-8")


if __name__ == "__main__":
    raise SystemExit(main())