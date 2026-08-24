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
"""Command-level regression checks for the discrete point-extraction fixtures."""

from __future__ import annotations

import csv
import json
import subprocess
import sys
import tempfile
import unittest
from pathlib import Path


ROOT = Path(__file__).resolve().parents[3]
SKILLS = ROOT / ".github" / "skills"


class PointExtractionTests(unittest.TestCase):
    def test_clean_discrete_fixtures_match_truth_counts(self) -> None:
        cases = (
            (
                "3-1-freefall",
                "images/3-1-freefall.png",
                "images/3-1-freefall.csv",
                "109,93,600,426",
                11,
            ),
            (
                "cleaned-scatter",
                "images/A+cleaned+up+scatter+plot.jpg",
                "images/A+cleaned+up+scatter+plot.CSV",
                "102,119,793,580",
                19,
            ),
        )

        with tempfile.TemporaryDirectory(prefix="plot-digitizer-ticket06-") as temporary:
            for name, image, truth, box, expected in cases:
                workdir = Path(temporary) / name
                self._run(
                    "analyse-axis",
                    "locate_plot_area.py",
                    "--image",
                    image,
                    "--workdir",
                    str(workdir),
                    "--box",
                    box,
                )
                self._run(
                    "segment-data",
                    "extract_point_series.py",
                    "--image",
                    image,
                    "--workdir",
                    str(workdir),
                    "--series-id",
                    "s1",
                )

                extraction = json.loads((workdir / "extraction.json").read_text(encoding="utf-8"))
                self.assertEqual(extraction["series"][0]["point_count"], expected, name)
                with (ROOT / truth).open(newline="", encoding="utf-8-sig") as handle:
                    rows = list(csv.reader(handle))
                self.assertEqual(expected, len(rows) - 1, name)

    def test_score_uses_extracted_centroids(self) -> None:
        with tempfile.TemporaryDirectory(prefix="plot-digitizer-ticket06-score-") as temporary:
            workdir = Path(temporary)
            image = "images/3-1-freefall.png"
            self._run(
                "analyse-axis",
                "locate_plot_area.py",
                "--image",
                image,
                "--workdir",
                str(workdir),
                "--box",
                "109,93,600,426",
            )
            self._run(
                "segment-data",
                "extract_point_series.py",
                "--image",
                image,
                "--workdir",
                str(workdir),
                "--series-id",
                "s1",
            )
            self._run(
                "analyse-axis",
                "read_axis_scale.py",
                "--image",
                image,
                "--workdir",
                str(workdir),
                "--x",
                "109=0,709=0.8",
                "--x-title",
                "Time (s)",
                "--y",
                "93=1.6,519=0",
                "--y-title",
                "Position (m)",
            )
            self._run("export-data", "export_data.py", "--image", image, "--workdir", str(workdir))
            self._run(
                "review-extraction",
                "score_extraction.py",
                "--image",
                image,
                "--workdir",
                str(workdir),
                "--truth",
                "images/3-1-freefall.csv",
            )

            score = json.loads((workdir / "extraction.json").read_text(encoding="utf-8"))["score"]["series"][0]
            self.assertEqual(score["n_extracted"], 11)
            self.assertEqual(score["coverage"], 1.0)

    def _run(self, group: str, script: str, *arguments: str) -> None:
        command = [sys.executable, str(SKILLS / group / script), *arguments]
        completed = subprocess.run(
            command,
            cwd=ROOT,
            capture_output=True,
            text=True,
            encoding="utf-8",
            errors="replace",
        )
        if completed.returncode:
            self.fail(
                f"{script} exited {completed.returncode}\nstdout:\n{completed.stdout}\nstderr:\n{completed.stderr}"
            )


if __name__ == "__main__":
    unittest.main()