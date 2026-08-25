"""The extraction document - the single record every skill reads from and adds to.

Bulk coordinate arrays never live here. Series carry counts and sidecar file references instead, so
the document stays small enough for an agent to read in full.
"""

from __future__ import annotations

import json
from datetime import datetime, timezone
from pathlib import Path
from typing import Any

from pdkit.result import Result

SCHEMA_VERSION = 1

_LIST_SECTIONS = ("protected_regions", "mask_layers", "series", "diagnostics", "stages")
_DICT_SECTIONS = ("image", "chart", "plot_area", "axes", "legend")
_RECT_KEYS = ("x", "y", "width", "height")


def rect(x: float, y: float, width: float, height: float) -> dict[str, float]:
    return {"x": float(x), "y": float(y), "width": float(width), "height": float(height)}


def _empty() -> dict[str, Any]:
    return {
        "schema_version": SCHEMA_VERSION,
        "image": {},
        "chart": {},
        "plot_area": {},
        "axes": {},
        "legend": {},
        "protected_regions": [],
        "mask_layers": [],
        "series": [],
        "diagnostics": [],
        "stages": [],
    }


class Extraction:
    """A thin, forgiving wrapper over the document. Unknown keys are preserved untouched."""

    def __init__(self, data: dict[str, Any] | None = None) -> None:
        self.data = _empty()
        if data:
            self.data.update(data)
            for section in _LIST_SECTIONS:
                self.data.setdefault(section, [])
            for section in _DICT_SECTIONS:
                self.data.setdefault(section, {})

    # -- persistence -------------------------------------------------------

    @classmethod
    def load(cls, path: Path) -> Extraction:
        if not path.exists():
            return cls()
        return cls(json.loads(path.read_text(encoding="utf-8")))

    def save(self, path: Path) -> None:
        path.parent.mkdir(parents=True, exist_ok=True)
        path.write_text(json.dumps(self.data, indent=2, ensure_ascii=False), encoding="utf-8")

    # -- sections ----------------------------------------------------------

    def set_image(self, path: Path, width: int, height: int, sha256: str) -> None:
        self.data["image"] = {
            "path": str(path),
            "name": path.name,
            "width": int(width),
            "height": int(height),
            "sha256": sha256,
        }

    @property
    def plot_area(self) -> dict[str, Any]:
        return self.data["plot_area"]

    def set_plot_area(self, box: dict[str, float], method: str, confidence: float) -> None:
        self.data["plot_area"] = {**box, "method": method, "confidence": float(confidence)}

    def set_axis(self, role: str, fit: dict[str, Any], axis_id: str | None = None) -> None:
        """Store a fitted pixel-to-value mapping for one axis role, "x" or "y".

        The bare role is the default slot every single-axis figure uses, read by
        `extraction.data["axes"]["x"]` / `["y"]` exactly as before. A distinct `axis_id` stores the
        fit under `axes.secondary` instead, so a figure with several X or Y mappings keeps every one
        of them rather than the newest overwriting the last.
        """
        stored = {**fit, "id": axis_id or role, "role": role}
        if axis_id is None or axis_id == role:
            self.data["axes"][role] = stored
            return
        secondary = [a for a in self.data["axes"].get("secondary", []) if a.get("id") != axis_id]
        secondary.append(stored)
        self.data["axes"]["secondary"] = secondary

    def get_axis(self, role: str, axis_id: str | None = None) -> dict[str, Any] | None:
        """The default mapping for `role`, or the named secondary one fitted alongside it."""
        if axis_id is None or axis_id == role:
            return self.data["axes"].get(role) or None
        for axis in self.data["axes"].get("secondary", []):
            if axis.get("role") == role and axis.get("id") == axis_id:
                return axis
        return None

    def axes_for(self, role: str) -> list[dict[str, Any]]:
        """Every mapping fitted for one role: the default axis first, then each named secondary one."""
        axes = [axis for axis in (self.data["axes"].get(role),) if axis]
        axes += [a for a in self.data["axes"].get("secondary", []) if a.get("role") == role]
        return axes

    def protect(self, name: str, box: dict[str, float]) -> None:
        """Mark a region no filter may claim. Re-protecting the same name replaces it."""
        regions = [r for r in self.data["protected_regions"] if r.get("name") != name]
        regions.append({"name": name, **box})
        self.data["protected_regions"] = regions

    def add_mask_layer(self, name: str, kind: str, file: str, pixels: int) -> None:
        layers = [layer for layer in self.data["mask_layers"] if layer.get("name") != name]
        layers.append({"name": name, "kind": kind, "file": file, "pixels": int(pixels)})
        self.data["mask_layers"] = layers

    def mask_layers(self, kind: str | None = None) -> list[dict[str, Any]]:
        layers = self.data["mask_layers"]
        return [layer for layer in layers if kind is None or layer.get("kind") == kind]

    def upsert_series(self, series_id: str, **fields: Any) -> dict[str, Any]:
        for existing in self.data["series"]:
            if existing.get("id") == series_id:
                existing.update(fields)
                return existing
        entry = {
            "id": series_id,
            "label": None,
            "kind": None,
            "style": {},
            "mask": None,
            "points_pixel": None,
            "points_data": None,
            "point_count": 0,
            "confidence": 0.0,
            **fields,
        }
        self.data["series"].append(entry)
        return entry

    def record(self, stage: str, script: str, result: Result, summary: str) -> None:
        """Append this run's diagnostics and a stage-log entry."""
        self.data["diagnostics"].extend(d.as_dict() for d in result.diagnostics)
        self.data["stages"].append(
            {
                "name": stage,
                "script": script,
                "ran_at": datetime.now(timezone.utc).isoformat(timespec="seconds"),
                "confidence": round(float(result.confidence), 3),
                "summary": summary,
            }
        )

    # -- validation --------------------------------------------------------

    def validate(self) -> list[str]:
        """Structural problems with the document. Empty means valid."""
        problems: list[str] = []
        if self.data.get("schema_version") != SCHEMA_VERSION:
            problems.append(f"schema_version must be {SCHEMA_VERSION}")
        for section in _DICT_SECTIONS:
            if not isinstance(self.data.get(section), dict):
                problems.append(f"'{section}' must be an object")
        for section in _LIST_SECTIONS:
            if not isinstance(self.data.get(section), list):
                problems.append(f"'{section}' must be an array")

        image = self.data.get("image")
        if isinstance(image, dict) and image:
            for key in ("path", "width", "height", "sha256"):
                if key not in image:
                    problems.append(f"image is missing '{key}'")

        area = self.data.get("plot_area")
        if isinstance(area, dict) and area:
            problems.extend(_rect_problems(area, "plot_area"))

        secondary_axes = self.data.get("axes", {}).get("secondary", [])
        for index, axis in enumerate(secondary_axes):
            for key in ("id", "role", "min", "max", "pixel_min", "pixel_max", "scale", "confidence"):
                if key not in axis:
                    problems.append(f"axes.secondary[{index}] is missing '{key}'")
            if axis.get("role") not in ("x", "y"):
                problems.append(f"axes.secondary[{index}].role must be 'x' or 'y'")
        ids = [axis.get("id") for axis in secondary_axes]
        if len(ids) != len(set(ids)):
            problems.append("axes.secondary ids must be unique")

        for index, region in enumerate(self.data.get("protected_regions", [])):
            problems.extend(_rect_problems(region, f"protected_regions[{index}]"))

        for index, layer in enumerate(self.data.get("mask_layers", [])):
            for key in ("name", "kind", "file", "pixels"):
                if key not in layer:
                    problems.append(f"mask_layers[{index}] is missing '{key}'")

        for index, entry in enumerate(self.data.get("series", [])):
            if "id" not in entry:
                problems.append(f"series[{index}] is missing 'id'")
            for key in ("points_pixel", "points_data"):
                value = entry.get(key)
                if isinstance(value, (list, tuple)):
                    problems.append(
                        f"series[{index}].{key} must be a sidecar file reference, not inline data"
                    )

        for index, diagnostic in enumerate(self.data.get("diagnostics", [])):
            for key in ("stage", "severity", "message"):
                if key not in diagnostic:
                    problems.append(f"diagnostics[{index}] is missing '{key}'")
        return problems


def _rect_problems(box: dict[str, Any], where: str) -> list[str]:
    return [f"{where} is missing '{key}'" for key in _RECT_KEYS if key not in box]
