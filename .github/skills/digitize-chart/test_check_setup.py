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
"""Command-level checks for the artifact-contract setup script."""

from __future__ import annotations

import json
import subprocess
import tempfile
import unittest
from pathlib import Path

from PIL import Image


ROOT = Path(__file__).resolve().parents[3]
SCRIPT = ROOT / ".github" / "skills" / "digitize-chart" / "check_setup.py"


class SetupContractTests(unittest.TestCase):
    def test_generated_image_setup_is_repeat_safe(self) -> None:
        with tempfile.TemporaryDirectory(prefix="plot-digitizer-ticket02-") as temporary:
            temporary_path = Path(temporary)
            image = temporary_path / "setup-input.png"
            workdir = temporary_path / "workdir"
            Image.new("RGB", (16, 12), (240, 240, 240)).save(image)

            first = self._run(image, workdir)
            second = self._run(image, workdir)

            extraction_path = workdir / "extraction.json"
            extraction = json.loads(extraction_path.read_text(encoding="utf-8"))
            self.assertEqual(extraction["image"]["name"], image.name)
            self.assertEqual(extraction["image"]["width"], 16)
            self.assertEqual(extraction["image"]["height"], 12)
            self.assertEqual(len(extraction["stages"]), 2)
            self.assertTrue((workdir / "overlays" / "check-setup.png").exists())
            self.assertIn("all present", first.stdout)
            self.assertIn("all present", second.stdout)

    def _run(self, image: Path, workdir: Path) -> subprocess.CompletedProcess[str]:
        completed = subprocess.run(
            ["uv", "run", str(SCRIPT), "--image", str(image), "--workdir", str(workdir)],
            cwd=ROOT,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
        )
        if completed.returncode:
            self.fail(
                f"check_setup.py exited {completed.returncode}\n"
                f"stdout:\n{completed.stdout}\nstderr:\n{completed.stderr}"
            )
        return completed


if __name__ == "__main__":
    unittest.main()
