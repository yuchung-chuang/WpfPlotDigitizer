"""The value/confidence/diagnostics convention every operation returns.

Expected conditions - no legend, an ambiguous axis, an unsupported figure - are reported through
this type. Only genuine programming errors raise.
"""

from __future__ import annotations

from dataclasses import dataclass, field
from typing import Any

INFO = "info"
WARNING = "warning"
ERROR = "error"

_SEVERITY_ORDER = {INFO: 0, WARNING: 1, ERROR: 2}


@dataclass
class Diagnostic:
    stage: str
    severity: str
    message: str
    region: dict[str, int] | None = None

    def as_dict(self) -> dict[str, Any]:
        return {
            "stage": self.stage,
            "severity": self.severity,
            "message": self.message,
            "region": self.region,
        }


@dataclass
class Result:
    """A value, how much the operation trusts it, and anything worth the agent's attention."""

    value: Any = None
    confidence: float = 0.0
    diagnostics: list[Diagnostic] = field(default_factory=list)
    stage: str = ""

    def note(self, message: str, region: dict[str, int] | None = None) -> None:
        self.diagnostics.append(Diagnostic(self.stage, INFO, message, region))

    def warn(self, message: str, region: dict[str, int] | None = None) -> None:
        self.diagnostics.append(Diagnostic(self.stage, WARNING, message, region))

    def fail(self, message: str, region: dict[str, int] | None = None) -> None:
        self.diagnostics.append(Diagnostic(self.stage, ERROR, message, region))
        self.confidence = 0.0

    @property
    def worst_severity(self) -> str | None:
        if not self.diagnostics:
            return None
        return max((d.severity for d in self.diagnostics), key=lambda s: _SEVERITY_ORDER[s])

    @property
    def failed(self) -> bool:
        return any(d.severity == ERROR for d in self.diagnostics)

    def counts(self) -> dict[str, int]:
        counts = {INFO: 0, WARNING: 0, ERROR: 0}
        for diagnostic in self.diagnostics:
            counts[diagnostic.severity] += 1
        return counts
