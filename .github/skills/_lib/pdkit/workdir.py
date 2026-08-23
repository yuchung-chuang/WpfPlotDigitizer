"""The per-figure working directory.

Everything produced while digitizing one figure lives here: the extraction document, the overlays
the agent looks at, the mask layers each filter contributes, and the coordinate sidecars that keep
bulk data out of the agent's context.
"""

from __future__ import annotations

import re
from pathlib import Path

DEFAULT_ROOT = ".digitize"
EXTRACTION_NAME = "extraction.json"
COMBINED_MASK_NAME = "plot_mask.png"


def _slug(name: str) -> str:
    return re.sub(r"[^A-Za-z0-9._-]+", "-", name).strip("-").lower() or "figure"


class WorkDir:
    """Resolves and creates the folder tree for one figure."""

    def __init__(self, image_path: Path, root: Path | None = None) -> None:
        self.image_path = image_path.resolve()
        base = Path(root) if root is not None else Path.cwd() / DEFAULT_ROOT / _slug(image_path.stem)
        self.path = base.resolve()

    def create(self) -> WorkDir:
        for folder in (self.path, self.overlays, self.masks, self.points, self.crops):
            folder.mkdir(parents=True, exist_ok=True)
        return self

    @property
    def overlays(self) -> Path:
        return self.path / "overlays"

    @property
    def masks(self) -> Path:
        return self.path / "masks"

    @property
    def points(self) -> Path:
        return self.path / "points"

    @property
    def crops(self) -> Path:
        return self.path / "crops"

    @property
    def extraction_path(self) -> Path:
        return self.path / EXTRACTION_NAME

    @property
    def combined_mask_path(self) -> Path:
        return self.masks / COMBINED_MASK_NAME

    def overlay_path(self, stage: str) -> Path:
        return self.overlays / f"{_slug(stage)}.png"

    def relative(self, path: Path) -> str:
        """Path recorded in the extraction document, relative to the working directory."""
        return path.resolve().relative_to(self.path).as_posix()

    def display(self, path: Path) -> str:
        """Path shown to the agent - relative to the current directory when that is shorter."""
        resolved = path.resolve()
        try:
            return resolved.relative_to(Path.cwd()).as_posix()
        except ValueError:
            return str(resolved)
