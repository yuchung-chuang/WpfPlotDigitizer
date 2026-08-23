"""Shared command-line conventions.

Every skill script takes the same two arguments, prints the same shape of summary, and ends by
pointing the agent at the overlay it must look at.
"""

from __future__ import annotations

import argparse
from pathlib import Path

import numpy as np

from pdkit.extraction import Extraction
from pdkit.imageio import read_image, sha256_of
from pdkit.result import ERROR, INFO, Result, WARNING
from pdkit.workdir import WorkDir


def base_parser(description: str) -> argparse.ArgumentParser:
    parser = argparse.ArgumentParser(description=description)
    parser.add_argument("--image", type=Path, required=True, help="the figure to work on")
    parser.add_argument(
        "--workdir",
        type=Path,
        default=None,
        help="working directory (default: .digitize/<figure name>/)",
    )
    return parser


def open_workspace(args: argparse.Namespace) -> tuple[WorkDir, Extraction, np.ndarray]:
    """Create or reopen the working directory and load the figure and its extraction."""
    if not args.image.exists():
        raise SystemExit(f"no such image: {args.image}")

    workdir = WorkDir(args.image, args.workdir).create()
    image = read_image(args.image)
    extraction = Extraction.load(workdir.extraction_path)
    if not extraction.data["image"]:
        height, width = image.shape[:2]
        extraction.set_image(args.image.resolve(), width, height, sha256_of(args.image))
    return workdir, extraction, image


def finish(
    workdir: WorkDir,
    extraction: Extraction,
    stage: str,
    script: str,
    result: Result,
    lines: list[str] | None = None,
    overlay: Path | None = None,
) -> int:
    """Record the run, save the extraction, print the summary, and return an exit code."""
    lines = lines or []
    summary = "; ".join(" ".join(line.split()) for line in lines) if lines else stage
    extraction.record(stage, script, result, summary)

    problems = extraction.validate()
    if problems:
        print(f"{stage}  EXTRACTION DOCUMENT INVALID")
        for problem in problems:
            print(f"  {problem}")
        return 1

    extraction.save(workdir.extraction_path)

    print(f"{stage}  confidence {result.confidence:.2f}")
    for line in lines:
        print(f"  {line}")
    print(f"  extraction  {workdir.display(workdir.extraction_path)}")
    if overlay is not None:
        print(f"  overlay     {workdir.display(overlay)}   <-- view this before continuing")

    counts = result.counts()
    if any(counts.values()):
        parts = [f"{counts[s]} {s}" for s in (ERROR, WARNING, INFO) if counts[s]]
        print(f"  diagnostics {', '.join(parts)}")
        for diagnostic in result.diagnostics:
            if diagnostic.severity != INFO:
                print(f"    {diagnostic.severity}: {diagnostic.message}")

    return 1 if result.failed else 0
