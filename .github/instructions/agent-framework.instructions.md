---
description: Conventions for the Python agent-first plot-digitizer framework and its scratch ticket tracker.
applyTo: ".scratch/plot-digitizer-agent/**, .github/skills/**"
---

# Agent-first plot-digitizer conventions

The `.scratch/plot-digitizer-agent/` tracker and `.github/skills/` implementation are a separate
Python framework. They are not part of the existing C#/.NET applications.

## Scope and validation

- Work only on the chart fixtures and scenarios named by the active ticket. Do not apply one skill
  indiscriminately across every image in `images/`.
- Validate through the relevant `uv` single-file scripts, focused Python tests when present,
  working-directory artifacts, overlays, and ground-truth CSV scoring.
- Treat the extraction document, sidecar files, overlays, and score as the framework's test seam.
- The existing C#/.NET projects and test projects are out of scope for scratch tickets. Do not run
  `dotnet build` or `dotnet test` as a scratch-ticket completion check, and do not add C# tests for
  this framework.

## Framework conventions

- Keep bulk coordinates in sidecar files, not extraction summaries.
- View every generated overlay before accepting a stage result.
- Preserve the working directory and structured diagnostics when an operation produces a weak result.
- Keep ticket status and acceptance results aligned with the fixture-specific evidence.
