"""Reading hand-authored ground truth.

The truth files are spreadsheets a person made, not a format a program designed: series sit in
column blocks separated by blank columns, headers appear one or two rows down, a block may hold one
shared X with several Y columns, uncertainty columns are mixed in, and the X column is not always
first. So this parses by evidence rather than by position, reports what it decided, and lets the
agent correct it.
"""

from __future__ import annotations

import csv
import re
from dataclasses import dataclass, field
from pathlib import Path

import numpy as np

UNCERTAINTY = re.compile(r"std|deviation|error|sd\b|±", re.IGNORECASE)
LABEL_KEY = re.compile(r"^label$", re.IGNORECASE)


@dataclass
class TruthSeries:
    label: str
    x: np.ndarray
    y: np.ndarray
    uncertainty: np.ndarray | None = None
    source: str = ""

    def __len__(self) -> int:
        return int(self.x.size)


@dataclass
class Truth:
    path: Path
    series: list[TruthSeries] = field(default_factory=list)
    notes: list[str] = field(default_factory=list)

    def __len__(self) -> int:
        return len(self.series)


def truth_path_for(image: Path) -> Path:
    return image.with_suffix(".csv")


def load_truth(path: Path) -> Truth:
    with path.open(newline="", encoding="utf-8-sig") as handle:
        grid = [row for row in csv.reader(handle)]
    if not grid:
        return Truth(path, notes=["file is empty"])

    width = max(len(row) for row in grid)
    grid = [row + [""] * (width - len(row)) for row in grid]

    truth = Truth(path)
    for block in _column_blocks(grid, width):
        truth.series.extend(_series_in_block(grid, block, truth.notes))
    if not truth.series:
        truth.notes.append("no numeric columns found")
    _warn_about_ambiguity(truth)
    return truth


def _warn_about_ambiguity(truth: Truth) -> None:
    """Surface the two ways this parse goes wrong, so the agent checks rather than trusts."""
    names = [series.label for series in truth.series]
    for name in sorted(set(names)):
        if names.count(name) > 1:
            truth.notes.append(f"{names.count(name)} series share the label '{name}'")

    x_names = {_bare(series.source.split(" vs ")[0]) for series in truth.series}
    if len(x_names) > 1:
        truth.notes.append(
            "blocks disagree about which column is X ("
            + ", ".join(sorted(x_names))
            + "); check the axes against the figure"
        )


def _bare(header: str) -> str:
    """Header without its unit, so 'Temperature' and 'Temperature (C)' compare equal."""
    return header.split("(")[0].strip().lower()


def _column_blocks(grid: list[list[str]], width: int) -> list[list[int]]:
    """Split columns into groups separated by entirely blank columns."""
    used = [any(row[col].strip() for row in grid) for col in range(width)]
    blocks, current = [], []
    for col in range(width):
        if used[col]:
            current.append(col)
        elif current:
            blocks.append(current)
            current = []
    if current:
        blocks.append(current)
    return blocks


def _series_in_block(grid: list[list[str]], columns: list[int], notes: list[str]) -> list[TruthSeries]:
    first_data = _first_data_row(grid, columns)
    if first_data is None:
        return []

    header_row = _header_row(grid, columns, first_data)
    headers = {col: grid[header_row][col].strip() if header_row is not None else "" for col in columns}
    block_label = _block_label(grid, columns, header_row)

    values = {col: _column_values(grid, col, first_data) for col in columns}
    numeric = [col for col in columns if np.isfinite(values[col]).sum() >= 2]
    if len(numeric) < 2:
        return []

    uncertainty = [col for col in numeric if UNCERTAINTY.search(headers[col])]
    measured = [col for col in numeric if col not in uncertainty]
    if len(measured) < 2:
        return []

    x_col = _x_column(measured, values)
    y_cols = [col for col in measured if col != x_col]

    series = []
    for index, y_col in enumerate(y_cols):
        own = _column_label(grid, y_col, header_row)
        name = _series_name(own or block_label, headers[y_col], index, len(y_cols), bool(own))
        error = _nearest_uncertainty(y_col, uncertainty)
        x, y, sd = _aligned(values[x_col], values[y_col], values[error] if error else None)
        if x.size:
            series.append(
                TruthSeries(name, x, y, sd, source=f"{headers[x_col] or 'x'} vs {headers[y_col] or 'y'}")
            )
    if len(y_cols) > 1:
        notes.append(
            f"block at column {columns[0]}: one X ({headers[x_col] or 'unnamed'}) shared by "
            f"{len(y_cols)} Y columns"
        )
    return series


def _first_data_row(grid: list[list[str]], columns: list[int]) -> int | None:
    for index, row in enumerate(grid):
        if sum(1 for col in columns if _number(row[col]) is not None) >= 2:
            return index
    return None


def _header_row(grid: list[list[str]], columns: list[int], first_data: int) -> int | None:
    for index in range(first_data - 1, -1, -1):
        if any(grid[index][col].strip() for col in columns):
            return index
    return None


def _block_label(grid: list[list[str]], columns: list[int], header_row: int | None) -> str:
    """Rows above the header hold the series name, sometimes behind a 'Label' key."""
    if header_row is None:
        return ""
    for index in range(header_row - 1, -1, -1):
        cells = [grid[index][col].strip() for col in columns]
        named = [cell for cell in cells if cell and not LABEL_KEY.match(cell)]
        if named:
            return named[0]
    return ""


def _column_values(grid: list[list[str]], col: int, first_data: int) -> np.ndarray:
    return np.array([_number(row[col]) for row in grid[first_data:]], dtype=float)


def _x_column(measured: list[int], values: dict[int, np.ndarray]) -> int:
    """The independent variable is the monotonic one - position is not reliable in these files."""
    ranked = sorted(measured, key=lambda col: (-_monotonicity(values[col]), measured.index(col)))
    return ranked[0]


def _monotonicity(column: np.ndarray) -> float:
    finite = column[np.isfinite(column)]
    if finite.size < 3:
        return 0.0
    steps = np.diff(finite)
    if not steps.size:
        return 0.0
    return float(max((steps > 0).mean(), (steps < 0).mean()))


def _nearest_uncertainty(y_col: int, uncertainty: list[int]) -> int | None:
    following = [col for col in uncertainty if col > y_col]
    return min(following) if following else None


def _column_label(grid: list[list[str]], col: int, header_row: int | None) -> str:
    """A name written above this column alone, which is how a shared-X block names its series."""
    if header_row is None:
        return ""
    for index in range(header_row - 1, -1, -1):
        cell = grid[index][col].strip()
        if cell and not LABEL_KEY.match(cell):
            return cell
    return ""


def _series_name(label: str, header: str, index: int, total: int, label_is_own: bool) -> str:
    if label and (label_is_own or total == 1):
        return label
    if label and header:
        return f"{label} {header}".strip()
    return header or label or f"series {index + 1}"


def _aligned(x: np.ndarray, y: np.ndarray, sd: np.ndarray | None):
    keep = np.isfinite(x) & np.isfinite(y)
    return x[keep], y[keep], (sd[keep] if sd is not None else None)


def _number(cell: str) -> float | None:
    text = cell.strip().replace(",", "")
    if not text:
        return None
    try:
        return float(text)
    except ValueError:
        return None
