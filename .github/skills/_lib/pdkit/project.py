"""Turning pixels into data values and back.

Shared because three steps need the same mapping: export projects forward, verification projects
back, and scoring needs both to sit in the same space.
"""

from __future__ import annotations

import numpy as np


def to_data(pixels: np.ndarray, axis: dict) -> np.ndarray:
    """Project pixel positions along one axis into data values."""
    low, high = float(axis["pixel_min"]), float(axis["pixel_max"])
    if high == low:
        return np.full_like(np.asarray(pixels, dtype=float), np.nan)

    fraction = (np.asarray(pixels, dtype=float) - low) / (high - low)
    base = axis.get("log_base")
    if axis.get("scale") == "log" and base:
        span = np.log(float(axis["max"])) - np.log(float(axis["min"]))
        return np.exp(np.log(float(axis["min"])) + fraction * span)
    return float(axis["min"]) + fraction * (float(axis["max"]) - float(axis["min"]))


def to_pixels(values: np.ndarray, axis: dict) -> np.ndarray:
    """The inverse of `to_data`, for drawing known values back onto the figure."""
    low, high = float(axis["pixel_min"]), float(axis["pixel_max"])
    base = axis.get("log_base")
    values = np.asarray(values, dtype=float)

    if axis.get("scale") == "log" and base:
        span = np.log(float(axis["max"])) - np.log(float(axis["min"]))
        if span == 0:
            return np.full_like(values, np.nan)
        with np.errstate(divide="ignore", invalid="ignore"):
            fraction = (np.log(values) - np.log(float(axis["min"]))) / span
    else:
        span = float(axis["max"]) - float(axis["min"])
        if span == 0:
            return np.full_like(values, np.nan)
        fraction = (values - float(axis["min"])) / span
    return low + fraction * (high - low)


def axis_span(axis: dict) -> float:
    """The axis range, used to normalise an error so figures compare on equal terms."""
    base = axis.get("log_base")
    if axis.get("scale") == "log" and base:
        return abs(np.log10(float(axis["max"])) - np.log10(float(axis["min"])))
    return abs(float(axis["max"]) - float(axis["min"]))


def normalise(values: np.ndarray, axis: dict) -> np.ndarray:
    """Values mapped onto [0, 1] across the axis, so x and y errors are comparable."""
    base = axis.get("log_base")
    values = np.asarray(values, dtype=float)
    if axis.get("scale") == "log" and base:
        with np.errstate(divide="ignore", invalid="ignore"):
            low, high = np.log10(float(axis["min"])), np.log10(float(axis["max"]))
            return (np.log10(values) - low) / (high - low) if high != low else values * 0
    low, high = float(axis["min"]), float(axis["max"])
    return (values - low) / (high - low) if high != low else values * 0


def axes_ready(extraction) -> list[str]:
    """Which axes are missing a usable fit. Empty means both are ready."""
    missing = []
    for name in ("x", "y"):
        axis = extraction.data.get("axes", {}).get(name) or {}
        if not all(key in axis for key in ("min", "max", "pixel_min", "pixel_max")):
            missing.append(name)
    return missing
