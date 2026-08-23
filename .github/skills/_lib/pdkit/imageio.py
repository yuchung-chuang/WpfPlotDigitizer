"""Reading and writing image files without tripping over non-ASCII paths."""

from __future__ import annotations

import hashlib
from pathlib import Path

import cv2
import numpy as np


def read_image(path: Path) -> np.ndarray:
    """Load an image as BGR. Uses imdecode so non-ASCII paths work on Windows."""
    data = np.fromfile(str(path), dtype=np.uint8)
    if data.size == 0:
        raise ValueError(f"{path} is empty or unreadable")
    image = cv2.imdecode(data, cv2.IMREAD_COLOR)
    if image is None:
        raise ValueError(f"{path} is not a decodable image")
    return image


def write_image(path: Path, image: np.ndarray) -> None:
    path.parent.mkdir(parents=True, exist_ok=True)
    ok, buffer = cv2.imencode(path.suffix or ".png", image)
    if not ok:
        raise ValueError(f"could not encode an image as {path.suffix}")
    buffer.tofile(str(path))


def sha256_of(path: Path) -> str:
    digest = hashlib.sha256()
    with path.open("rb") as handle:
        for block in iter(lambda: handle.read(65536), b""):
            digest.update(block)
    return digest.hexdigest()
