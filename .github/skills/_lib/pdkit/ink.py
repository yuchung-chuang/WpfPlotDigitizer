"""Ink, and the long straight lines drawn in it.

Separating ink from background, and finding the long axis-aligned lines, is the first thing almost
every operation does. Sharing it here keeps one answer to "is this pixel ink?" across the framework.
"""

from __future__ import annotations

import cv2
import numpy as np


def background_colour(image: np.ndarray) -> np.ndarray:
    """The page colour the chart is drawn on: the most common colour in the figure.

    Sampling the outermost pixels instead would pick up a figure's own border line and invert
    every ink test that follows.
    """
    height, width = image.shape[:2]
    stride = max(1, int(np.sqrt(height * width / 50_000)))
    samples = image[::stride, ::stride].reshape(-1, 3)

    quantised = (samples // 16).astype(np.int32)
    keys = quantised[:, 0] * 4096 + quantised[:, 1] * 64 + quantised[:, 2]
    counts = np.bincount(keys)
    dominant = counts.argmax()
    return np.median(samples[keys == dominant], axis=0)


def ink_mask(image: np.ndarray, tolerance: int = 40) -> np.ndarray:
    """True where a pixel differs from the page colour. Works on light and dark backgrounds alike."""
    background = background_colour(image).astype(np.int16)
    difference = np.abs(image.astype(np.int16) - background).max(axis=2)
    return difference > tolerance


def outline_mask(image: np.ndarray, tolerance: int = 40, step: int = 8) -> np.ndarray:
    """Ink, plus the boundary of every area fill.

    A panel drawn as a pale shaded rectangle rather than a framed one carries no ink at its edge,
    so anything looking for the plot boundary sees nothing without the fill's outline. `step` is
    the smallest brightness step that counts as a boundary, low enough for a faint panel wash.
    """
    grey = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    kernel = cv2.getStructuringElement(cv2.MORPH_RECT, (3, 3))
    gradient = cv2.morphologyEx(grey, cv2.MORPH_GRADIENT, kernel)
    return ink_mask(image, tolerance) | (gradient > step)


def clear_border(mask: np.ndarray) -> np.ndarray:
    """Drop every component touching the edge of the mask.

    A figure's own outer border swallows everything inside it into one component; removing it
    exposes the structure that was really wanted.
    """
    binary = (np.asarray(mask) > 0).astype(np.uint8)
    count, labels = cv2.connectedComponents(binary, connectivity=8)
    if count <= 1:
        return binary > 0
    touching = np.unique(
        np.concatenate([labels[0, :], labels[-1, :], labels[:, 0], labels[:, -1]])
    )
    keep = np.ones(count, dtype=bool)
    keep[touching[touching > 0]] = False
    keep[0] = False
    return keep[labels]


def run_coverage(
    mask: np.ndarray, direction: str, length: int, coverage: float = 0.95
) -> np.ndarray:
    """True where a run of `length` pixels heading `direction` is at least `coverage` ink.

    Accepting most of a run rather than all of it is what lets a probe follow an axis line across
    the gaps where a curve, a tick label or a compression artefact interrupts it.
    """
    field = (np.asarray(mask) > 0).astype(np.int32)
    axis = 1 if direction in ("right", "left") else 0
    backwards = direction in ("left", "up")
    if backwards:
        field = field[:, ::-1] if axis == 1 else field[::-1, :]

    span = int(max(1, min(length, field.shape[axis])))
    pad = list(field.shape)
    pad[axis] = 1
    cumulative = np.concatenate([np.zeros(pad, np.int32), field.cumsum(axis=axis)], axis=axis)
    totals = (
        cumulative[:, span:] - cumulative[:, :-span]
        if axis == 1
        else cumulative[span:, :] - cumulative[:-span, :]
    )

    covered = np.zeros(field.shape, dtype=bool)
    enough = totals >= coverage * span
    if axis == 1:
        covered[:, : enough.shape[1]] = enough
    else:
        covered[: enough.shape[0], :] = enough
    if backwards:
        covered = covered[:, ::-1] if axis == 1 else covered[::-1, :]
    return covered


def long_runs(mask: np.ndarray, axis: int, min_length: int) -> np.ndarray:
    """Keep only ink belonging to an unbroken run of at least `min_length` along `axis`."""
    length = max(int(min_length), 2)
    kernel = (
        cv2.getStructuringElement(cv2.MORPH_RECT, (length, 1))
        if axis == 1
        else cv2.getStructuringElement(cv2.MORPH_RECT, (1, length))
    )
    binary = (mask > 0).astype(np.uint8) * 255
    return cv2.morphologyEx(binary, cv2.MORPH_OPEN, kernel) > 0


def covered_runs(
    mask: np.ndarray, axis: int, min_length: int, coverage: float = 0.95
) -> np.ndarray:
    """`long_runs` for lines that data crosses: a run may be `coverage` ink rather than all ink.

    A frame edge or a grid line interrupted by a marker sitting on it is still one line, and an
    unbroken-run test throws it away.
    """
    length = max(int(min_length), 2)
    starts = run_coverage(mask, "right" if axis == 1 else "down", length, coverage)
    kernel = np.ones((1, length) if axis == 1 else (length, 1), np.uint8)
    anchor = (length - 1, 0) if axis == 1 else (0, length - 1)
    spread = cv2.dilate(starts.astype(np.uint8), kernel, anchor=anchor) > 0
    return spread & (np.asarray(mask) > 0)


def line_clusters(lines: np.ndarray, axis: int, gap: int = 3) -> list[dict]:
    """Group a long-run mask into one entry per line.

    `axis=1` finds horizontal lines keyed by row, `axis=0` vertical lines keyed by column.
    Each entry carries its position, how far it extends, and how much ink it holds.
    """
    counts = lines.sum(axis=axis)
    occupied = np.flatnonzero(counts > 0)
    if occupied.size == 0:
        return []

    clusters: list[dict] = []
    start = occupied[0]
    previous = occupied[0]
    for position in occupied[1:]:
        if position - previous > gap:
            clusters.append(_cluster(lines, axis, start, previous, counts))
            start = position
        previous = position
    clusters.append(_cluster(lines, axis, start, previous, counts))
    return clusters


def _cluster(lines: np.ndarray, axis: int, start: int, end: int, counts: np.ndarray) -> dict:
    band = lines[start : end + 1, :] if axis == 1 else lines[:, start : end + 1]
    along = band.any(axis=0 if axis == 1 else 1)
    present = np.flatnonzero(along)
    weights = counts[start : end + 1]
    centre = float(np.average(np.arange(start, end + 1), weights=weights)) if weights.sum() else start
    return {
        "position": centre,
        "thickness": int(end - start + 1),
        "from": int(present[0]) if present.size else 0,
        "to": int(present[-1]) if present.size else 0,
        "length": int(present.size),
    }


def looks_like_grid(positions: list[float], tolerance: float = 0.2) -> bool:
    """Four or more near-evenly spaced lines are a grid, not a set of axes."""
    if len(positions) < 4:
        return False
    gaps = np.diff(sorted(positions))
    if gaps.size == 0 or gaps.mean() <= 0:
        return False
    return bool(gaps.std() / gaps.mean() < tolerance)


def has_tick_marks(
    ink: np.ndarray,
    position: float,
    start: int,
    end: int,
    axis: int,
    min_ticks: int = 4,
) -> bool:
    """Whether a long line carries tick marks, which is what makes it an axis.

    A tick is a short stub attached to the line with nothing beyond it, repeated at regular
    intervals. Data crossing the line leaves ink too, but irregularly, and gridlines leave none —
    so this is what separates an axis from a zero-line or a rule.
    """
    field = ink if axis == 0 else ink.T
    column = int(round(position))
    lo, hi = max(0, start), min(field.shape[0], end + 1)
    if hi - lo < 4:
        return False

    for sign in (-1, 1):
        near = _band(field, column, sign, 2, 9)[lo:hi]
        far = _band(field, column, sign, 13, 26)[lo:hi]
        if near.size == 0:
            continue
        stubs = np.flatnonzero(near.any(axis=1) & ~far.any(axis=1))
        centres = [float(np.mean(group)) for group in _consecutive(stubs)]
        if len(centres) >= min_ticks and looks_like_grid(centres, tolerance=0.35):
            return True
    return False


def _band(field: np.ndarray, column: int, sign: int, inner: int, outer: int) -> np.ndarray:
    low, high = sorted((column + sign * inner, column + sign * outer))
    low, high = max(0, low), min(field.shape[1], high)
    if high <= low:
        return np.empty((field.shape[0], 0), dtype=bool)
    return field[:, low:high]


def _consecutive(indices: np.ndarray, gap: int = 2) -> list[np.ndarray]:
    if indices.size == 0:
        return []
    breaks = np.flatnonzero(np.diff(indices) > gap) + 1
    return np.split(indices, breaks)
