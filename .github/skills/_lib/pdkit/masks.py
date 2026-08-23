"""Mask layers.

Each operation claims pixels in its own named layer. The combined mask is derived by union and is
never edited directly, so any single layer can be dropped or recomputed without redoing the others.
"""

from __future__ import annotations

from pathlib import Path

import numpy as np

from pdkit.imageio import read_image, write_image


def save_mask(path: Path, mask: np.ndarray) -> int:
    """Write a boolean or 0/255 mask as a single-channel PNG. Returns the claimed pixel count."""
    binary = (np.asarray(mask) > 0).astype(np.uint8) * 255
    write_image(path, binary)
    return int(np.count_nonzero(binary))


def load_mask(path: Path) -> np.ndarray:
    image = read_image(path)
    if image.ndim == 3:
        image = image[:, :, 0]
    return image > 0


def combine_masks(masks: list[np.ndarray], shape: tuple[int, int]) -> np.ndarray:
    combined = np.zeros(shape, dtype=bool)
    for mask in masks:
        combined |= np.asarray(mask) > 0
    return combined


def rebuild_combined(workdir, extraction, shape: tuple[int, int]) -> int:
    """Union every noise layer into the combined mask. Call after adding or dropping a layer."""
    layers = []
    for layer in extraction.mask_layers(kind="noise"):
        path = workdir.path / layer["file"]
        if path.exists():
            layers.append(load_mask(path))
    combined = combine_masks(layers, shape)
    return save_mask(workdir.combined_mask_path, combined)
