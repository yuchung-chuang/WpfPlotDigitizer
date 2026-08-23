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
"""Turn the markers inside the plot area into one series of sub-pixel centroids.

Writes the coordinates to a sidecar under points/, records the series and its mask in the
extraction, and draws every centroid it kept over the figure.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np
from scipy import ndimage
from skimage.feature import peak_local_max
from skimage.segmentation import watershed

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, save_mask
from pdkit.ink import clear_border, ink_mask
from pdkit.masks import load_mask

STAGE = "extract-point-series"
OPENING_RADIUS = 1
Z_THRESHOLD = 3.5
INSET = 2
MIN_SPREAD_FRACTION = 0.25
OVERLAP_FACTOR = 2.0
MIN_BLOBS_FOR_REJECTION = 8
SPLIT_AREA_FACTOR = 1.5
FADE = 0.35


def main() -> int:
    parser = base_parser("Extract marker centroids from the plot area as a single point series.")
    parser.add_argument("--series-id", default="s1", help="id recorded in the extraction (default %(default)s)")
    parser.add_argument("--label", default=None, help="what the legend calls this series")
    parser.add_argument(
        "--radius",
        type=int,
        default=OPENING_RADIUS,
        help="radius of the disk opened out of the ink to erase connecting lines while markers "
        "survive; 0 disables the opening (default %(default)s)",
    )
    parser.add_argument(
        "--z-threshold",
        type=float,
        default=Z_THRESHOLD,
        help="how far from the median area a blob may sit, in robust standard deviations, before "
        "it is discarded as an outlier (default %(default)s)",
    )
    parser.add_argument(
        "--inset",
        type=int,
        default=INSET,
        help="pixels trimmed off each side of the plot area, so the frame is not extracted "
        "(default %(default)s)",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)

    area = extraction.plot_area
    if not area or "width" not in area:
        result.fail("no plot area in the extraction; run analyse-axis/locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["plot area  missing"])

    region = _inset(area, args.inset, image.shape[:2])
    if region["width"] < 4 or region["height"] < 4:
        result.fail(f"the plot area {_describe(area)} is too small to extract anything from")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["plot area  too small"])

    crop = _crop(image, region)
    noise, source = _noise(workdir, extraction, region, result)
    ink = ink_mask(crop)
    if noise is not None:
        ink &= ~noise

    filled = ndimage.binary_fill_holes(clear_border(ink))
    opened = _open_disk(filled, args.radius)
    erased = _fraction_removed(filled, opened)

    raw_blobs = _blobs(opened)
    marker_area = _modal_area(raw_blobs)
    blobs, split_count = _split_blobs(opened, marker_area)
    kept, discarded, median = _reject_outliers(blobs, args.z_threshold, result)
    oversized = [blob for blob in raw_blobs if marker_area > 0 and blob[2] >= OVERLAP_FACTOR * marker_area]
    if oversized:
        result.warn(
            f"{len(oversized)} blob(s) are at least {OVERLAP_FACTOR:g}x the median marker area, so "
            f"they are overlapping markers drawn as one; {len(kept)} is an undercount",
            region=_integers(_bounds(oversized, region)),
        )
    if not kept:
        result.warn(
            "no markers survived; try a smaller --radius, or filter the noise first if the plot "
            "area is full of grid lines"
        )

    points = [(x + region["x"], y + region["y"]) for x, y, _ in kept]
    sidecar = workdir.points / f"{args.series_id}-pixel.csv"
    _write_points(sidecar, points)

    mask_path = workdir.masks / f"series-{args.series_id}.png"
    pixels = save_mask(mask_path, _placed(opened, region, image.shape[:2]))
    extraction.add_mask_layer(f"series-{args.series_id}", "series", workdir.relative(mask_path), pixels)

    result.confidence = _confidence(kept, discarded, oversized, noise is not None)
    extraction.upsert_series(
        args.series_id,
        label=args.label,
        kind="point",
        mask=workdir.relative(mask_path),
        points_pixel=workdir.relative(sidecar),
        point_count=len(points),
        confidence=result.confidence,
    )

    overlay = _draw(image, region, points, discarded).save(workdir.overlay_path(STAGE))
    summary = [
        f"points      {len(points)} kept, {len(discarded)} discarded as area outliers",
        f"area        {_areas(kept, median)}",
        f"splitting   {split_count} overlapping blob(s) with watershed from modal area {marker_area:.0f} px",
        f"opening     disk radius {args.radius}, erased {erased:.0%} of the ink",
        f"ink source  {source}",
        f"region      {_describe(region)}   (plot area inset by {args.inset} px)",
        f"series      {args.series_id}  ->  {workdir.relative(sidecar)}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, summary, overlay)


def _inset(area: dict, inset: int, shape: tuple[int, int]) -> dict:
    """The plot area pulled in from its own frame, clamped to the figure."""
    height, width = shape
    x = max(0, int(round(area["x"])) + inset)
    y = max(0, int(round(area["y"])) + inset)
    right = min(width, int(round(area["x"] + area["width"])) - inset)
    bottom = min(height, int(round(area["y"] + area["height"])) - inset)
    return {"x": x, "y": y, "width": max(0, right - x), "height": max(0, bottom - y)}


def _crop(image: np.ndarray, region: dict) -> np.ndarray:
    return image[region["y"] : region["y"] + region["height"], region["x"] : region["x"] + region["width"]]


def _noise(workdir, extraction, region: dict, result: Result) -> tuple[np.ndarray | None, str]:
    """The combined noise mask cropped to the region, when the filtering steps have written one."""
    layers = extraction.mask_layers(kind="noise")
    path = workdir.combined_mask_path
    if not layers or not path.exists():
        return None, "raw plot area - no noise mask yet, so run the noise filters for a cleaner one"

    mask = load_mask(path)
    names = ", ".join(layer["name"] for layer in layers)
    result.note(f"subtracting the combined noise mask built from {len(layers)} layer(s): {names}")
    return _crop(mask, region), f"{workdir.relative(path)} minus {len(layers)} noise layer(s)"


def _open_disk(mask: np.ndarray, radius: int) -> np.ndarray:
    """Erase anything thinner than a marker.

    Ported from the recognition prototype: a connecting line or a trend line is narrower than the
    markers it joins, so opening with a disk of the marker's own scale deletes the line and leaves
    the markers standing.
    """
    if radius < 1:
        return mask
    disk = cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (2 * radius + 1, 2 * radius + 1))
    return cv2.morphologyEx(mask.astype(np.uint8), cv2.MORPH_OPEN, disk) > 0


def _fraction_removed(before: np.ndarray, after: np.ndarray) -> float:
    total = int(np.count_nonzero(before))
    if total == 0:
        return 0.0
    return 1.0 - int(np.count_nonzero(after)) / total


def _blobs(mask: np.ndarray) -> list[tuple[float, float, float]]:
    """One sub-pixel centroid and area per external contour."""
    contours, _ = cv2.findContours(
        mask.astype(np.uint8), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
    )
    return [_centroid(contour) for contour in contours]


def _modal_area(blobs: list[tuple[float, float, float]]) -> float:
    """Estimate the area of one marker without letting merged blobs set the scale."""
    if not blobs:
        return 0.0
    areas = np.asarray([blob[2] for blob in blobs], dtype=float)
    typical = max(float(np.median(areas)), 1.0)
    width = max(1.0, round(typical * 0.2))
    bins = np.floor(areas / width).astype(int)
    mode = max(np.unique(bins), key=lambda value: (int(np.count_nonzero(bins == value)), -value))
    members = areas[bins == mode]
    return float(np.median(members))


def _split_blobs(
    mask: np.ndarray, marker_area: float
) -> tuple[list[tuple[float, float, float]], int]:
    """Split touching markers with distance-transform watershed when the component is oversized."""
    if marker_area <= 0:
        return [], 0

    count, labels, stats, _ = cv2.connectedComponentsWithStats(
        mask.astype(np.uint8), connectivity=8
    )
    blobs: list[tuple[float, float, float]] = []
    split_count = 0
    minimum_distance = max(1, int(round(np.sqrt(marker_area / np.pi))))
    for label in range(1, count):
        component = labels == label
        area = float(stats[label, cv2.CC_STAT_AREA])
        if area < SPLIT_AREA_FACTOR * marker_area:
            blobs.extend(_component_blobs(component))
            continue

        distance = ndimage.distance_transform_edt(component)
        peaks = peak_local_max(
            distance,
            min_distance=minimum_distance,
            threshold_abs=0.5,
            labels=component,
            exclude_border=False,
        )
        if len(peaks) < 2:
            blobs.extend(_component_blobs(component))
            continue

        markers = np.zeros(component.shape, dtype=np.int32)
        markers[peaks[:, 0], peaks[:, 1]] = np.arange(1, len(peaks) + 1)
        regions = watershed(-distance, markers, mask=component)
        pieces = 0
        for region in range(1, len(peaks) + 1):
            blobs.extend(_component_blobs(regions == region))
            pieces += 1
        split_count += 1 if pieces > 1 else 0
    return blobs, split_count


def _component_blobs(component: np.ndarray) -> list[tuple[float, float, float]]:
    contours, _ = cv2.findContours(
        component.astype(np.uint8), cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE
    )
    return [_centroid(contour) for contour in contours]


def _centroid(contour: np.ndarray) -> tuple[float, float, float]:
    """The moment centroid, falling back to the mean of the outline when the moments vanish.

    Ported from the desktop client, degenerate case included: a single pixel or a one-pixel-wide
    streak encloses no area, so the moment centroid divides by zero and the mean is all there is.
    Never rounded - a whole-pixel centroid is a whole-pixel error in the extracted value.
    """
    moments = cv2.moments(contour)
    points = contour.reshape(-1, 2).astype(float)
    if moments["m00"] > 0:
        return moments["m10"] / moments["m00"], moments["m01"] / moments["m00"], moments["m00"]
    return float(points[:, 0].mean()), float(points[:, 1].mean()), float(len(points))


def _reject_outliers(
    blobs: list[tuple[float, float, float]], threshold: float, result: Result
) -> tuple[list, list, float]:
    """Discard blobs whose area is nothing like the rest.

    The prototype used a plain z-score, which a handful of merged blobs inflate enough to hide
    themselves in; the median and the median absolute deviation do not move. The spread is floored
    at a fraction of the median so a figure whose markers are all identical - the common case -
    does not reject its own antialiasing.
    """
    if len(blobs) < MIN_BLOBS_FOR_REJECTION:
        if blobs:
            result.note(f"only {len(blobs)} blob(s); too few to call any of them an area outlier")
        return list(blobs), [], float(np.median([b[2] for b in blobs])) if blobs else 0.0

    areas = np.array([blob[2] for blob in blobs], dtype=float)
    median = float(np.median(areas))
    spread = max(1.4826 * float(np.median(np.abs(areas - median))), MIN_SPREAD_FRACTION * median, 1.0)
    scores = np.abs(areas - median) / spread

    kept = [blob for blob, score in zip(blobs, scores) if score <= threshold]
    discarded = [blob for blob, score in zip(blobs, scores) if score > threshold]
    if discarded:
        result.note(
            f"discarded {len(discarded)} of {len(blobs)} blob(s) with areas outside "
            f"{max(median - threshold * spread, 0):.0f}-{median + threshold * spread:.0f} px "
            f"around a median of {median:.0f} px"
        )
    return kept, discarded, median


def _bounds(blobs: list[tuple[float, float, float]], region: dict) -> dict:
    xs = [blob[0] + region["x"] for blob in blobs]
    ys = [blob[1] + region["y"] for blob in blobs]
    return {
        "x": min(xs),
        "y": min(ys),
        "width": max(max(xs) - min(xs), 1),
        "height": max(max(ys) - min(ys), 1),
    }


def _placed(mask: np.ndarray, region: dict, shape: tuple[int, int]) -> np.ndarray:
    """A region-sized mask put back where it came from, so mask layers share the figure's frame."""
    full = np.zeros(shape, dtype=bool)
    full[region["y"] : region["y"] + region["height"], region["x"] : region["x"] + region["width"]] = mask
    return full


def _write_points(path: Path, points: list[tuple[float, float]]) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    rows = "\n".join(f"{x:.3f},{y:.3f}" for x, y in points)
    path.write_text("x,y\n" + rows + ("\n" if rows else ""), encoding="utf-8")


def _confidence(kept: list, discarded: list, oversized: list, denoised: bool) -> float:
    if not kept:
        return 0.0
    confidence = 0.8 if denoised else 0.7
    if oversized:
        confidence -= 0.15
    if len(discarded) > 0.1 * (len(kept) + len(discarded)):
        confidence -= 0.1
    return round(max(confidence, 0.1), 3)


def _areas(kept: list[tuple[float, float, float]], median: float) -> str:
    if not kept:
        return "no blobs measured"
    areas = [blob[2] for blob in kept]
    return f"median {median:.0f} px, kept {min(areas):.0f}-{max(areas):.0f} px"


def _describe(box: dict) -> str:
    return f"x={box['x']:.0f} y={box['y']:.0f} w={box['width']:.0f} h={box['height']:.0f}"


def _integers(box: dict) -> dict[str, int]:
    return {key: int(round(value)) for key, value in box.items()}


def _draw(image: np.ndarray, region: dict, points: list, discarded: list) -> Overlay:
    overlay = Overlay(_faded(image))
    overlay.rectangle(region, palette_colour(6), thickness=1)
    if discarded:
        rejected = [(blob[0] + region["x"], blob[1] + region["y"]) for blob in discarded]
        overlay.points(rejected, palette_colour(1), size=9, label=f"{len(rejected)} area outliers")
    overlay.points(points, palette_colour(0), size=5)
    return overlay.key(f"{len(points)} centroids", palette_colour(0))


def _faded(image: np.ndarray) -> np.ndarray:
    """The figure washed out towards white, so a cross on a marker of its own colour still shows."""
    return cv2.addWeighted(image, FADE, np.full_like(image, 255), 1 - FADE, 0)


if __name__ == "__main__":
    raise SystemExit(main())
