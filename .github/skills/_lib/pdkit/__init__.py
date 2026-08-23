"""Shared toolkit for the chart digitizing skills.

Every skill script bootstraps this package with::

    import sys
    from pathlib import Path
    sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))
"""

from pdkit.cli import base_parser, finish, open_workspace
from pdkit.extraction import Extraction, rect
from pdkit.imageio import read_image, sha256_of, write_image
from pdkit.masks import combine_masks, load_mask, save_mask
from pdkit.overlay import Overlay, palette_colour
from pdkit.result import Diagnostic, Result
from pdkit.workdir import WorkDir

__all__ = [
    "Diagnostic",
    "Extraction",
    "Overlay",
    "Result",
    "WorkDir",
    "base_parser",
    "combine_masks",
    "finish",
    "load_mask",
    "open_workspace",
    "palette_colour",
    "read_image",
    "rect",
    "save_mask",
    "sha256_of",
    "write_image",
]
