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
"""Cluster plot ink in LAB space and write one mask per discovered series."""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np
from scipy import ndimage
from scipy.optimize import linear_sum_assignment
from sklearn.cluster import KMeans

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import ink_mask
from pdkit.masks import load_mask

STAGE = "segment-series"
DEFAULT_CLUSTERS = 3
MAX_CLUSTER_PIXELS = 60_000
MIN_CLUSTER_PIXELS = 8
SAME_COLOUR_DISTANCE = 12.0
SHAPE_MATCH_THRESHOLD = 0.42
SHAPE_NMS_SIZE = 9


def main() -> int:
    parser = base_parser("Separate plot ink into one mask per series using LAB colour clustering.")
    parser.add_argument(
        "--clusters",
        type=int,
        default=None,
        help="number of clusters (default: legend entry count, or 3 without a legend)",
    )
    parser.add_argument(
        "--merge",
        action="append",
        default=[],
        metavar="A,B",
        help="merge discovered cluster indexes, repeatable",
    )
    parser.add_argument(
        "--split",
        action="append",
        default=[],
        type=int,
        metavar="INDEX",
        help="split a discovered cluster into two spatial subclusters, repeatable",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)
    region = extraction.data.get("plot_area") or {}
    if not all(key in region for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["segmentation skipped"])

    candidate = _candidate_mask(workdir, extraction, image, region, result)
    pixel_count = int(candidate.sum())
    if pixel_count < MIN_CLUSTER_PIXELS:
        result.fail("the plot area contains too little unclaimed ink to cluster")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["segmentation skipped"])

    legend = extraction.data.get("legend") or {}
    entries = legend.get("entries") or []
    cluster_count = args.clusters or len(entries) or DEFAULT_CLUSTERS
    cluster_count = max(1, min(cluster_count, pixel_count))
    labels, centres = _cluster(image, candidate, cluster_count)
    clustered_masks = _cluster_masks(labels, candidate, image.shape[:2])
    assignments = _match_styles(centres, entries)
    masks_with_entries = _series_masks(image, candidate, entries, clustered_masks, centres, assignments, result)
    masks_with_entries = _apply_merges_with_entries(masks_with_entries, args.merge, result)
    masks_with_entries = _apply_splits_with_entries(masks_with_entries, args.split, result)
    masks_with_entries = [
        (mask, entry_index)
        for mask, entry_index in masks_with_entries
        if int(mask.sum()) >= MIN_CLUSTER_PIXELS
    ]
    masks = [mask for mask, _ in masks_with_entries]
    if entries and len(masks) != len(entries):
        result.warn(
            f"found {len(masks)} series clusters but the legend has {len(entries)} entries; "
            "inspect the overlay or override --clusters"
        )
    if args.merge or args.split:
        result.note(
            f"applied {len(args.merge)} merge and {len(args.split)} split correction(s); "
            "the correction is recorded in the stage history"
        )

    extraction.data["mask_layers"] = [
        layer for layer in extraction.data["mask_layers"] if layer.get("kind") != "series"
    ]
    extraction.data["series"] = []
    overlay = Overlay(image)
    for index, (mask, entry_index) in enumerate(masks_with_entries):
        entry = entries[entry_index] if entry_index is not None and entry_index < len(entries) else {}
        series_id = f"s{index + 1}"
        path = workdir.masks / f"series-{series_id}.png"
        pixels = save_mask(path, mask)
        extraction.add_mask_layer(series_id, "series", workdir.relative(path), pixels)
        style = entry.get("style") or {}
        extraction.upsert_series(
            series_id,
            label=entry.get("label"),
            kind=style.get("kind", "point"),
            style=style,
            mask=workdir.relative(path),
            point_count=0,
            confidence=round(_series_confidence(mask, candidate), 3),
        )
        overlay.tint(mask, palette_colour(index), label=series_id, alpha=0.45)

    result.confidence = _confidence(masks, entries, pixel_count)
    overlay_path = overlay.save(workdir.overlay_path(STAGE))
    summary = [
        f"clusters    {len(masks)} found from {cluster_count} LAB clusters",
        f"pixels      {pixel_count} candidate ink pixels",
        f"legend      {len(entries)} entries" if entries else "legend      none; unsupervised clustering",
        f"masks       {len(masks)} written under {workdir.relative(workdir.masks)}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay_path)


def _candidate_mask(workdir, extraction, image: np.ndarray, region: dict, result: Result) -> np.ndarray:
    height, width = image.shape[:2]
    candidate = np.zeros((height, width), dtype=bool)
    x = max(0, int(round(region["x"])))
    y = max(0, int(round(region["y"])))
    right = min(width, int(round(region["x"] + region["width"])))
    bottom = min(height, int(round(region["y"] + region["height"])))
    candidate[y:bottom, x:right] = ink_mask(image)[y:bottom, x:right]

    for protected in extraction.data.get("protected_regions", []):
        _clear(candidate, protected)
    noise_layers = extraction.mask_layers(kind="noise")
    for layer in noise_layers:
        path = workdir.path / layer["file"]
        if path.exists():
            candidate &= ~load_mask(path)
    result.note(
        f"clustering candidate excludes {len(noise_layers)} noise layer(s) and "
        f"{len(extraction.data.get('protected_regions', []))} protected region(s)"
    )
    return candidate


def _clear(mask: np.ndarray, box: dict) -> None:
    height, width = mask.shape
    x = max(0, int(round(box.get("x", 0))))
    y = max(0, int(round(box.get("y", 0))))
    right = min(width, int(round(box.get("x", 0) + box.get("width", 0))))
    bottom = min(height, int(round(box.get("y", 0) + box.get("height", 0))))
    mask[y:bottom, x:right] = False


def _cluster(image: np.ndarray, mask: np.ndarray, cluster_count: int) -> tuple[np.ndarray, np.ndarray]:
    pixels = image[mask]
    lab = cv2.cvtColor(pixels.reshape(1, -1, 3), cv2.COLOR_BGR2LAB).reshape(-1, 3).astype(float)
    if len(lab) > MAX_CLUSTER_PIXELS:
        sample = np.linspace(0, len(lab) - 1, MAX_CLUSTER_PIXELS, dtype=int)
        training = lab[sample]
    else:
        training = lab
    model = KMeans(n_clusters=cluster_count, n_init=10, random_state=0).fit(training)
    labels = model.predict(lab)
    return labels, model.cluster_centers_


def _cluster_masks(labels: np.ndarray, candidate: np.ndarray, shape: tuple[int, int]) -> list[np.ndarray]:
    masks = []
    for index in range(int(labels.max()) + 1):
        mask = np.zeros(shape, dtype=bool)
        mask[candidate] = labels == index
        masks.append(mask)
    return masks


def _series_masks(
    image: np.ndarray,
    candidate: np.ndarray,
    entries: list[dict],
    clustered_masks: list[np.ndarray],
    centres: np.ndarray,
    assignments: list[int | None],
    result: Result,
) -> list[tuple[np.ndarray, int | None]]:
    """Prefer marker-shape masks for duplicate colours, then fall back to colour clusters."""
    if not entries:
        return [(mask, None) for mask in clustered_masks]

    represented: set[int] = set()
    output: list[tuple[np.ndarray, int | None]] = []
    for group in _colour_groups(entries):
        if len(group) == 1:
            entry_index = group[0]
            style = entries[entry_index].get("style") or {}
            threshold = 110.0 if style.get("line_style", "none") != "none" else None
            output.append((_colour_group_mask(image, candidate, entries, group, threshold), entry_index))
            represented.add(entry_index)
            continue
        detected: dict[int, list[tuple[float, float]]] = {}
        line_group = any(
            (entries[entry_index].get("style") or {}).get("line_style", "none") != "none"
            for entry_index in group
        )
        union = candidate.copy() if line_group or _same_colour_group(entries, group) else _colour_group_mask(image, candidate, entries, group)
        if line_group or len(group) > 2:
            for entry_index in group:
                output.append((union.copy(), entry_index))
                represented.add(entry_index)
            result.note(
                f"{len(group)} entries share a color; reused the complete color mask for "
                "line localization"
            )
            continue
        for entry_index in group:
            style = entries[entry_index].get("style") or {}
            marker = style.get("marker", "none")
            if marker == "none":
                continue
            centres_for_shape = _shape_centres(image, union, entries[entry_index], marker)
            if not centres_for_shape:
                result.warn(f"could not find {marker} markers for legend entry {entry_index}")
                continue
            detected[entry_index] = centres_for_shape
            represented.add(entry_index)
            result.note(
                f"entry {entry_index} uses {len(centres_for_shape)} {marker} shape matches "
                "because its colour is shared"
            )
        for entry_index, centres_for_shape in detected.items():
            marker = (entries[entry_index].get("style") or {}).get("marker", "none")
            if marker == "circle":
                centres_for_shape = _remove_shape_collisions(
                    centres_for_shape,
                    [centre for other_index, values in detected.items() if other_index != entry_index for centre in values],
                    entries[entry_index].get("style") or {},
                )
            if centres_for_shape:
                output.append(
                    (_mask_around_centres(union, centres_for_shape, entries[entry_index].get("style") or {}), entry_index)
                )
            else:
                represented.discard(entry_index)

    used_clusters: set[int] = set()
    for cluster_index, entry_index in enumerate(assignments):
        if entry_index in represented:
            used_clusters.add(cluster_index)
            continue
        if entry_index is not None:
            output.append((clustered_masks[cluster_index], entry_index))
            used_clusters.add(cluster_index)
    output.extend(
        (mask, None) for index, mask in enumerate(clustered_masks) if index not in used_clusters
    )
    return output


def _colour_groups(entries: list[dict]) -> list[list[int]]:
    groups: list[list[int]] = []
    for index, entry in enumerate(entries):
        colour = np.asarray((entry.get("style") or {}).get("colour_lab", [0, 0, 0]), dtype=float)
        for group in groups:
            other = np.asarray((entries[group[0]].get("style") or {}).get("colour_lab", [0, 0, 0]), dtype=float)
            if np.linalg.norm(colour - other) <= SAME_COLOUR_DISTANCE:
                group.append(index)
                break
        else:
            groups.append([index])
    return groups


def _same_colour_group(entries: list[dict], group: list[int]) -> bool:
    colours = [
        np.asarray((entries[index].get("style") or {}).get("colour_lab", [0, 0, 0]), dtype=float)
        for index in group
    ]
    return bool(colours) and max(
        np.linalg.norm(colour - other) for colour in colours for other in colours
    ) <= SAME_COLOUR_DISTANCE


def _colour_group_mask(image, candidate, entries, group, threshold=None):
    lab = cv2.cvtColor(image, cv2.COLOR_BGR2LAB).astype(float)
    colour = np.mean(
        [np.asarray((entries[index].get("style") or {}).get("colour_lab", [0, 0, 0]), dtype=float) for index in group],
        axis=0,
    )
    distance = np.linalg.norm(lab - colour, axis=2)
    if threshold is None:
        threshold = max(45.0, SAME_COLOUR_DISTANCE * 4) if len(group) > 1 else max(32.0, SAME_COLOUR_DISTANCE * 3)
    return candidate & (distance <= threshold)


def _shape_centres(image, union, entry, marker):
    size = max(5, int(round(float((entry.get("style") or {}).get("marker_size") or 9))))
    if marker == "circle":
        return _circle_centres(union, size)
    template = _swatch_template(image, entry.get("swatch") or {})
    if template is None:
        return []
    target = union.astype(np.uint8) * 255
    score = cv2.matchTemplate(target, template, cv2.TM_CCOEFF_NORMED)
    maximum = ndimage.maximum_filter(score, size=max(SHAPE_NMS_SIZE, size))
    threshold = SHAPE_MATCH_THRESHOLD
    rows, columns = np.where((score >= threshold) & (score >= maximum - 1e-6))
    order = sorted(zip(rows.tolist(), columns.tolist()), key=lambda point: float(score[point[0], point[1]]), reverse=True)
    centres = []
    for row, column in order:
        centre = (column + template.shape[1] / 2, row + template.shape[0] / 2)
        if _inside_plot(centre, entry, union.shape) and all(
            (centre[0] - other[0]) ** 2 + (centre[1] - other[1]) ** 2 > (size * 0.65) ** 2
            for other in centres
        ):
            centres.append(centre)
    return centres


def _circle_centres(union, size):
    field = np.full(union.shape, 255, dtype=np.uint8)
    field[union] = 0
    radius = max(2, int(round(size / 2)))
    circles = cv2.HoughCircles(
        field,
        cv2.HOUGH_GRADIENT,
        dp=1,
        minDist=max(5, int(round(size * 0.75))),
        param1=50,
        param2=max(7, int(round(size * 0.85))),
        minRadius=max(2, radius - 3),
        maxRadius=radius + 3,
    )
    if circles is None:
        return []
    minimum_radius = max(4.5, size * 0.48)
    return [
        tuple(circle[:2])
        for circle in np.round(circles[0], 2)
        if float(circle[2]) >= minimum_radius
    ]


def _swatch_template(image, box):
    crop = _crop_box(image, box)
    if crop.size == 0:
        return None
    mask = ink_mask(crop)
    line = cv2.morphologyEx(
        mask.astype(np.uint8),
        cv2.MORPH_OPEN,
        cv2.getStructuringElement(cv2.MORPH_RECT, (max(5, crop.shape[1] // 2), 1)),
    ) > 0
    columns = mask.sum(axis=0)
    peak = int(np.argmax(columns)) if columns.size else 0
    baseline = float(np.median(columns)) if columns.size else 0.0
    marker_columns = np.flatnonzero(columns >= max(2.0, baseline + 1.0))
    runs = _runs(marker_columns)
    run = next((candidate for candidate in runs if candidate[0] <= peak <= candidate[-1]), None)
    if run is None or run.size == 0:
        return None
    left = max(0, int(run[0]) - 2)
    right = min(mask.shape[1], int(run[-1]) + 3)
    marker = mask[:, left:right]
    line = line[:, left:right]
    rows = np.flatnonzero(marker.any(axis=1))
    if rows.size == 0:
        return None
    marker = marker[rows[0] : rows[-1] + 1]
    line = line[rows[0] : rows[-1] + 1]
    marker[line.sum(axis=1) >= max(3, 0.7 * line.shape[1])] = False
    marker = cv2.morphologyEx(marker.astype(np.uint8), cv2.MORPH_CLOSE, np.ones((5, 1), np.uint8))
    rows, columns = np.nonzero(marker)
    if not len(rows):
        return None
    marker = marker[rows.min() : rows.max() + 1, columns.min() : columns.max() + 1]
    return cv2.copyMakeBorder(marker * 255, 1, 1, 1, 1, cv2.BORDER_CONSTANT, value=0)


def _crop_box(image, box):
    height, width = image.shape[:2]
    x = max(0, int(round(box.get("x", 0))))
    y = max(0, int(round(box.get("y", 0))))
    right = min(width, int(round(box.get("x", 0) + box.get("width", 0))))
    bottom = min(height, int(round(box.get("y", 0) + box.get("height", 0))))
    return image[y:bottom, x:right]


def _inside_plot(centre, entry, shape):
    x, y = centre
    return 0 <= x < shape[1] and 0 <= y < shape[0]


def _mask_around_centres(union, centres, style):
    size = max(5, int(round(float(style.get("marker_size") or 9))))
    radius = max(3, int(round(size * 0.72)))
    mask = np.zeros_like(union, dtype=bool)
    for x, y in centres:
        cv2.circle(mask, (int(round(x)), int(round(y))), radius, True, -1)
    return mask


def _runs(indices: np.ndarray) -> list[np.ndarray]:
    if indices.size == 0:
        return []
    return np.split(indices, np.flatnonzero(np.diff(indices) > 1) + 1)


def _remove_shape_collisions(centres, other_centres, style):
    size = max(5, int(round(float(style.get("marker_size") or 9))))
    limit = (size * 1.25) ** 2
    return [
        centre
        for centre in centres
        if all((centre[0] - other[0]) ** 2 + (centre[1] - other[1]) ** 2 > limit for other in other_centres)
    ]


def _apply_merges_with_entries(masks, values, result):
    for value in values:
        try:
            left, right = (int(part.strip()) for part in value.split(","))
            if left == right or min(left, right) < 0 or max(left, right) >= len(masks):
                raise ValueError
            first_mask, first_entry = masks[left]
            second_mask, _ = masks[right]
            masks[left] = (first_mask | second_mask, first_entry)
            del masks[right]
            result.note(f"merged series mask {left} with {right}")
        except ValueError:
            result.warn(f"ignored malformed merge {value!r}; expected two valid series indexes")
    return masks


def _apply_splits_with_entries(masks, values, result):
    for index in sorted(values, reverse=True):
        if index < 0 or index >= len(masks) or int(masks[index][0].sum()) < 2:
            result.warn(f"ignored invalid split series {index}")
            continue
        source, entry = masks[index]
        ys, xs = np.nonzero(source)
        labels = KMeans(n_clusters=2, n_init=10, random_state=0).fit(np.column_stack((xs, ys))).labels_
        first = np.zeros_like(source)
        second = np.zeros_like(source)
        first[ys[labels == 0], xs[labels == 0]] = True
        second[ys[labels == 1], xs[labels == 1]] = True
        masks[index : index + 1] = [(first, entry), (second, None)]
        result.note(f"split series mask {index} into two spatial masks")
    return masks


def _apply_merges(masks: list[np.ndarray], values: list[str], result: Result) -> list[np.ndarray]:
    for value in values:
        try:
            left, right = (int(part.strip()) for part in value.split(","))
            if left == right or min(left, right) < 0 or max(left, right) >= len(masks):
                raise ValueError
            masks[left] |= masks[right]
            del masks[right]
            result.note(f"merged cluster {left} with cluster {right}")
        except ValueError:
            result.warn(f"ignored malformed merge {value!r}; expected two valid cluster indexes")
    return masks


def _apply_splits(masks: list[np.ndarray], values: list[int], image: np.ndarray, result: Result) -> list[np.ndarray]:
    for index in sorted(values, reverse=True):
        if index < 0 or index >= len(masks) or int(masks[index].sum()) < 2:
            result.warn(f"ignored invalid split cluster {index}")
            continue
        ys, xs = np.nonzero(masks[index])
        points = np.column_stack((xs, ys)).astype(float)
        split = KMeans(n_clusters=2, n_init=10, random_state=0).fit(points).labels_
        first = np.zeros(masks[index].shape, dtype=bool)
        second = np.zeros(masks[index].shape, dtype=bool)
        first[ys[split == 0], xs[split == 0]] = True
        second[ys[split == 1], xs[split == 1]] = True
        masks[index : index + 1] = [first, second]
        result.note(f"split cluster {index} into two spatial masks")
    return masks


def _match_styles(centres: np.ndarray, entries: list[dict]) -> list[int | None]:
    if not entries:
        return [None] * len(centres)
    expected = np.array(
        [entry.get("style", {}).get("colour_lab", [0, 0, 0]) for entry in entries], dtype=float
    )
    distances = np.linalg.norm(centres[:, None, :] - expected[None, :, :], axis=2)
    rows, columns = linear_sum_assignment(distances)
    assignments: list[int | None] = [None] * len(centres)
    for row, column in zip(rows, columns):
        assignments[row] = int(column)
    return assignments


def _series_confidence(mask: np.ndarray, candidate: np.ndarray) -> float:
    share = float(mask.sum() / max(candidate.sum(), 1))
    return round(max(0.2, min(0.9, 0.9 - share * 0.2)), 3)


def _confidence(masks: list[np.ndarray], entries: list[dict], pixels: int) -> float:
    if not masks or not pixels:
        return 0.0
    confidence = 0.7
    if entries and len(masks) != len(entries):
        confidence -= 0.25
    return round(max(0.1, confidence), 3)


if __name__ == "__main__":
    raise SystemExit(main())