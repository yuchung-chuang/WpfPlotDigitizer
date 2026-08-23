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
