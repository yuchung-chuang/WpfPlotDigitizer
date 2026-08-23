"""Overlays - the annotated copies of the figure that the agent must look at.

The overlay is the only assertion this framework has. If an operation went wrong, the overlay is
where it shows.
"""

from __future__ import annotations

from pathlib import Path

import cv2
import numpy as np

from pdkit.imageio import write_image

# BGR, chosen to stay distinguishable against white, black and mid greys.
PALETTE = [
    (60, 76, 231),
    (231, 158, 60),
    (76, 187, 23),
    (196, 76, 231),
    (23, 208, 231),
    (231, 76, 141),
    (120, 120, 120),
]


def palette_colour(index: int) -> tuple[int, int, int]:
    return PALETTE[index % len(PALETTE)]


class Overlay:
    """Draws on a copy of the figure. The original is never modified."""

    def __init__(self, image: np.ndarray) -> None:
        self.canvas = image.copy()
        self._legend: list[tuple[str, tuple[int, int, int]]] = []

    def rectangle(self, box: dict, colour: tuple[int, int, int], label: str = "", thickness: int = 2):
        x, y = int(round(box["x"])), int(round(box["y"]))
        w, h = int(round(box["width"])), int(round(box["height"]))
        cv2.rectangle(self.canvas, (x, y), (x + w, y + h), colour, thickness)
        if label:
            self._label(label, (x, max(y - 6, 12)), colour)
        return self

    def points(self, points, colour: tuple[int, int, int], size: int = 6, label: str = ""):
        for point in points:
            centre = (int(round(point[0])), int(round(point[1])))
            cv2.drawMarker(self.canvas, centre, colour, cv2.MARKER_CROSS, size, 1)
        if label:
            self.key(label, colour)
        return self

    def polyline(self, points, colour: tuple[int, int, int], label: str = ""):
        if len(points) >= 2:
            path = np.asarray(points, dtype=np.int32).reshape(-1, 1, 2)
            cv2.polylines(self.canvas, [path], False, colour, 2)
        if label:
            self.key(label, colour)
        return self

    def tint(self, mask: np.ndarray, colour: tuple[int, int, int], label: str = "", alpha: float = 0.55):
        """Wash the pixels a mask layer claimed, so an over-eager filter is visible."""
        selected = np.asarray(mask) > 0
        if selected.any():
            wash = np.zeros_like(self.canvas)
            wash[:] = colour
            self.canvas[selected] = cv2.addWeighted(
                self.canvas, 1 - alpha, wash, alpha, 0
            )[selected]
        if label:
            self.key(label, colour)
        return self

    def key(self, label: str, colour: tuple[int, int, int]):
        self._legend.append((label, colour))
        return self

    def _label(self, text: str, origin: tuple[int, int], colour: tuple[int, int, int]) -> None:
        cv2.putText(self.canvas, text, origin, cv2.FONT_HERSHEY_SIMPLEX, 0.5, (255, 255, 255), 3)
        cv2.putText(self.canvas, text, origin, cv2.FONT_HERSHEY_SIMPLEX, 0.5, colour, 1)

    def save(self, path: Path) -> Path:
        self._draw_key()
        write_image(path, self.canvas)
        return path

    def _draw_key(self) -> None:
        # Bottom-left, because rectangle labels sit above a box's top edge and plot areas are usually high.
        if not self._legend:
            return
        pad, row = 8, 18
        height = pad * 2 + row * len(self._legend)
        width = pad * 2 + 12 + max(len(text) for text, _ in self._legend) * 8
        top = max(self.canvas.shape[0] - height, 0)
        panel = self.canvas[top:top + height, 0:width]
        if panel.size:
            self.canvas[top:top + height, 0:width] = cv2.addWeighted(
                panel, 0.25, np.full_like(panel, 255), 0.75, 0
            )
        for index, (text, colour) in enumerate(self._legend):
            y = top + pad + row * index + 12
            cv2.rectangle(self.canvas, (pad, y - 8), (pad + 10, y + 2), colour, -1)
            cv2.putText(
                self.canvas, text, (pad + 16, y), cv2.FONT_HERSHEY_SIMPLEX, 0.45, (20, 20, 20), 1
            )
