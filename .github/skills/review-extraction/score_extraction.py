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
"""Score an extraction against hand-authored ground truth.

Matches each extracted series to a truth series, then measures how far the extracted points sit from
the truth in axis units normalised by the axis range, so figures with wildly different scales
compare on equal terms.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import numpy as np
from scipy.optimize import linear_sum_assignment
from scipy.spatial import cKDTree

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour
from pdkit.project import axes_ready, normalise, to_pixels
from pdkit.truth import load_truth, truth_path_for

STAGE = "score-extraction"
DEFAULT_TOLERANCE = 0.005


def main() -> int:
    parser = base_parser("Score an extraction against ground truth beside the figure.")
    parser.add_argument("--truth", type=Path, default=None, help="ground-truth CSV (default: beside the image)")
    parser.add_argument(
        "--tolerance",
        type=float,
        default=DEFAULT_TOLERANCE,
        help="passing median error, as a fraction of the axis range (default %(default)s)",
    )
    parser.add_argument(
        "--swap-truth-axes",
        choices=("auto", "yes", "no"),
        default="auto",
        help="whether the truth file's X column is the chart's Y axis (default %(default)s)",
    )
    parser.add_argument("--truth-x-factor", type=float, default=1.0, help="multiply truth X values before scoring")
    parser.add_argument("--truth-y-factor", type=float, default=1.0, help="multiply truth Y values before scoring")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)

    truth_file = args.truth or truth_path_for(args.image)
    if not truth_file.exists():
        result.warn(f"no ground truth beside the figure; expected {truth_file.name}")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["nothing to score"])

    missing = axes_ready(extraction)
    if missing:
        result.fail(f"axis {' and '.join(missing)} has no fit; score needs data units")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["nothing to score"])

    axes = extraction.data["axes"]
    if not (_title(axes, "x") or _title(axes, "y")):
        result.warn(
            "the axis fit has no titles, so truth columns cannot be matched by header; "
            "re-run read_axis_scale.py with --x-title and --y-title read from the figure"
        )

    truth = load_truth(truth_file, _title(axes, "x"), _title(axes, "y"))
    if args.truth_x_factor != 1.0 or args.truth_y_factor != 1.0:
        for series in truth.series:
            series.x *= args.truth_x_factor
            series.y *= args.truth_y_factor
        result.warn(
            f"ground truth values scaled for chart units: X x{args.truth_x_factor:g}, "
            f"Y x{args.truth_y_factor:g}"
        )
    for note in truth.notes:
        result.warn(f"ground truth: {note}")

    _orient_truth(truth, axes, result, args.swap_truth_axes)
    extracted = _extracted_series(workdir, extraction, axes, result)
    if not extracted or not truth.series:
        result.fail("nothing to compare")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, ["nothing to score"])

    truth_points = [_normalised(series.x, series.y, axes) for series in truth.series]
    pairs, cost = _match(truth_points, [points for _, points in extracted])

    scores, lines = [], []
    owner = _truth_owner(truth_points)
    for truth_index, extracted_index in pairs:
        report = _score_pair(truth_points[truth_index], extracted[extracted_index][1], args.tolerance)
        report["purity"] = _purity(extracted[extracted_index][1], owner, truth_index)
        report["truth"] = truth.series[truth_index].label
        report["series"] = extracted[extracted_index][0]["id"]
        scores.append(report)

    worst = max((report["median"] for report in scores), default=float("nan"))
    passed = [report for report in scores if report["median"] <= args.tolerance]
    result.confidence = round(len(passed) / len(scores), 3) if scores else 0.0

    for report in sorted(scores, key=lambda r: -r["median"]):
        verdict = "PASS" if report["median"] <= args.tolerance else "FAIL"
        lines.append(
            f"{verdict}  {report['series']:>4s} vs {report['truth'][:24]:24s} "
            f"median {report['median'] * 100:6.2f}%  p95 {report['p95'] * 100:6.2f}%  "
            f"covered {report['coverage'] * 100:5.1f}%  pure {report['purity'] * 100:5.1f}%  "
            f"({report['n_truth']} truth, {report['n_extracted']} extracted)"
        )

    unmatched_truth = len(truth.series) - len(pairs)
    unmatched_extracted = len(extracted) - len(pairs)
    if unmatched_truth:
        result.warn(f"{unmatched_truth} truth series had no extracted counterpart")
    if unmatched_extracted:
        result.warn(f"{unmatched_extracted} extracted series matched no truth series")

    extraction.data["score"] = {
        "truth_file": truth_file.name,
        "tolerance": args.tolerance,
        "passed": len(passed),
        "of": len(scores),
        "worst_median": round(float(worst), 6),
        "series_expected": len(truth.series),
        "series_found": len(extracted),
        "mean_purity": round(float(np.mean([r["purity"] for r in scores])), 4) if scores else 0.0,
        "series": [{k: v for k, v in report.items() if k != "errors"} for report in scores],
    }

    overlay = _draw(image, truth, extracted, axes).save(workdir.overlay_path(STAGE))
    segmentation = (
        f"{len(extracted)} found of {len(truth.series)} expected, "
        f"mean purity {extraction.data['score']['mean_purity'] * 100:.1f}%"
    )
    lines.insert(0, f"localisation {len(passed)} of {len(scores)} series within {args.tolerance * 100:.2f}% of range")
    lines.insert(1, f"segmentation {segmentation}")
    lines.insert(2, f"truth        {truth_file.name}")
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines, overlay)


def _truth_owner(truth_points: list[np.ndarray]):
    """A lookup from any point in the plane to the truth series nearest to it."""
    available = [(index, points) for index, points in enumerate(truth_points) if points.size]
    if not available:
        return None
    stacked = np.vstack([points for _, points in available])
    labels = np.concatenate([np.full(points.shape[0], index) for index, points in available])
    return cKDTree(stacked), labels


def _purity(extracted: np.ndarray, owner, truth_index: int) -> float:
    """Fraction of an extracted series' points whose nearest truth point is in the matched series.

    This is the segmentation measure: a series can be perfectly located and still be wrong if it is
    carrying another series' points.
    """
    if owner is None or extracted.size == 0:
        return 0.0
    tree, labels = owner
    _, nearest = tree.query(extracted)
    return float((labels[nearest] == truth_index).mean())


def _title(axes: dict, name: str) -> str | None:
    return (axes.get(name) or {}).get("title")


def _orient_truth(truth, axes: dict, result: Result, mode: str) -> None:
    """Last resort when no axis titles were available to match the columns by header."""
    if mode == "no":
        return
    if mode == "yes":
        _swap(truth)
        result.note("ground truth axes swapped as instructed")
        return
    if any(truth.axis_titles):
        return

    upright = sum(_inside(s.x, axes["x"]) + _inside(s.y, axes["y"]) for s in truth.series)
    swapped = sum(_inside(s.y, axes["x"]) + _inside(s.x, axes["y"]) for s in truth.series)
    if swapped > upright * 1.2:
        _swap(truth)
        result.warn(
            "no axis titles, so the truth columns were matched by value range instead and came out "
            "transposed; supply axis titles to resolve this by header"
        )


def _swap(truth) -> None:
    for series in truth.series:
        series.x, series.y = series.y, series.x


def _inside(values: np.ndarray, axis: dict) -> float:
    low, high = sorted((float(axis["min"]), float(axis["max"])))
    finite = values[np.isfinite(values)]
    return float(((finite >= low) & (finite <= high)).mean()) if finite.size else 0.0


def _extracted_series(workdir, extraction, axes: dict, result: Result) -> list[tuple[dict, np.ndarray]]:
    series = []
    for entry in extraction.data["series"]:
        reference = entry.get("points_data")
        if not reference:
            result.warn(f"series {entry['id']} is not exported yet; run export_data.py")
            continue
        path = workdir.path / reference
        if not path.exists():
            result.warn(f"series {entry['id']} export is missing: {reference}")
            continue
        values = np.atleast_2d(np.genfromtxt(path, delimiter=",", skip_header=1, usecols=(0, 1)))
        if values.size:
            series.append((entry, _normalised(values[:, 0], values[:, 1], axes)))
    return series


def _normalised(x, y, axes: dict) -> np.ndarray:
    points = np.column_stack([normalise(x, axes["x"]), normalise(y, axes["y"])])
    return points[np.isfinite(points).all(axis=1)]


def _match(truth_points: list[np.ndarray], extracted_points: list[np.ndarray]):
    """Assign extracted series to truth series by whichever pairing fits best overall."""
    cost = np.zeros((len(truth_points), len(extracted_points)))
    for i, truth in enumerate(truth_points):
        for j, extracted in enumerate(extracted_points):
            cost[i, j] = _median_distance(truth, extracted)
    rows, columns = linear_sum_assignment(cost)
    return list(zip(rows.tolist(), columns.tolist())), cost


def _median_distance(truth: np.ndarray, extracted: np.ndarray) -> float:
    if truth.size == 0 or extracted.size == 0:
        return 1e3
    distances, _ = cKDTree(extracted).query(truth)
    return float(np.median(distances))


def _score_pair(truth: np.ndarray, extracted: np.ndarray, tolerance: float) -> dict:
    if truth.size == 0:
        return {
            "median": float("inf"),
            "p95": float("inf"),
            "coverage": 0.0,
            "n_truth": 0,
            "n_extracted": int(extracted.shape[0]),
        }
    if extracted.size == 0:
        return {
            "median": float("inf"),
            "p95": float("inf"),
            "coverage": 0.0,
            "n_truth": int(truth.shape[0]),
            "n_extracted": 0,
        }
    distances, _ = cKDTree(extracted).query(truth)
    return {
        "median": float(np.median(distances)),
        "p95": float(np.percentile(distances, 95)),
        "coverage": float((distances <= tolerance).mean()),
        "n_truth": int(truth.shape[0]),
        "n_extracted": int(extracted.shape[0]),
    }


def _draw(image, truth, extracted, axes: dict) -> Overlay:
    overlay = Overlay(image)
    for index, series in enumerate(truth.series):
        overlay.points(_pixels(series.x, series.y, axes), palette_colour(index), size=15)
    overlay.key("ground truth (large)", palette_colour(0))

    for index, (entry, points) in enumerate(extracted):
        colour = palette_colour(index + len(truth.series))
        x = normalise_inverse(points[:, 0], axes["x"])
        y = normalise_inverse(points[:, 1], axes["y"])
        overlay.points(np.column_stack([x, y]), colour, size=7)
        overlay.key(f"extracted {entry['id']} (small)", colour)
    return overlay


def _pixels(x, y, axes: dict) -> np.ndarray:
    points = np.column_stack([to_pixels(x, axes["x"]), to_pixels(y, axes["y"])])
    return points[np.isfinite(points).all(axis=1)]


def normalise_inverse(fraction: np.ndarray, axis: dict) -> np.ndarray:
    """Back from the normalised [0, 1] space straight to pixels."""
    low, high = float(axis["pixel_min"]), float(axis["pixel_max"])
    return low + np.asarray(fraction, dtype=float) * (high - low)


if __name__ == "__main__":
    raise SystemExit(main())
