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
"""Prove the artifact contract works end to end on a real figure.

Creates the working directory, records the figure's identity in the extraction document, renders an
overlay, and reports whether every dependency the other skills rely on imported cleanly.
"""

from __future__ import annotations

import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, rect

STAGE = "check-setup"


def main() -> int:
    parser = base_parser("Verify the chart digitizing toolkit is installed and the contract works.")
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE, confidence=1.0)

    missing = _missing_dependencies()
    for name in missing:
        result.fail(f"dependency '{name}' failed to import")

    height, width = image.shape[:2]
    border = rect(0, 0, width - 1, height - 1)
    overlay = (
        Overlay(image)
        .rectangle(border, palette_colour(0), label=f"{width} x {height}")
        .key("figure bounds", palette_colour(0))
        .save(workdir.overlay_path(STAGE))
    )

    if not missing:
        result.note("toolkit imported and the working directory is writable")

    lines = [
        f"figure      {extraction.data['image']['name']} ({width} x {height})",
        f"workdir     {workdir.display(workdir.path)}",
        f"depends     {'all present' if not missing else 'MISSING ' + ', '.join(missing)}",
    ]
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines, overlay)


def _missing_dependencies() -> list[str]:
    import importlib

    missing = []
    for module in ("numpy", "cv2", "scipy", "skimage", "sklearn", "PIL"):
        try:
            importlib.import_module(module)
        except ImportError:
            missing.append(module)
    return missing


if __name__ == "__main__":
    raise SystemExit(main())
