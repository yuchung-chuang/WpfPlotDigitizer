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
from scipy.optimize import linear_sum_assignment
from sklearn.cluster import KMeans

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import ink_mask
from pdkit.masks import load_mask

STAGE = "segment-series"
DEFAULT_CLUSTERS = 3
MAX_CLUSTER_PIXELS = 60_000
MIN_CLUSTER_PIXELS = 8


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
    masks = _cluster_masks(labels, candidate, image.shape[:2])
    masks = _apply_merges(masks, args.merge, result)
    masks = _apply_splits(masks, args.split, image, result)
    masks = [mask for mask in masks if int(mask.sum()) >= MIN_CLUSTER_PIXELS]

    assignments = _match_styles(centres, entries)
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
    for index, mask in enumerate(masks):
        entry_index = assignments[index] if index < len(assignments) else None
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