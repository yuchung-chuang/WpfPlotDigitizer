# 01 — Domain glossary and architecture decisions

**What to build:** A shared vocabulary and a written record of the two decisions that shape
everything else, so that every later ticket uses the same words for the same things and a future
reader understands why this work is not another page in the desktop app.

The glossary must settle the terms that would otherwise cause two skills to fight over the same
pixels — in particular the boundaries between background, grid lines, annotations and the plot
border, and the dual role of the legend as something inspected during analysis but removed before
extraction.

The architecture records must capture the two hard-to-reverse choices: that the deliverable is an
agentic skill framework rather than a library or application, and that the scripts are Python
invoked through a file-based artifact contract rather than compiled code passing data in memory.

Version control must ignore the per-image working directories that every later ticket produces.

**Blocked by:** None — can start immediately.

**Status:** done

- [x] A domain glossary exists at the repository root and is a glossary only — no implementation
      detail, no specification content, no scratch notes.
- [x] The glossary defines: plot area, series, series style, marker, mask layer, extraction,
      working directory, overlay, confidence, diagnostic.
- [x] The glossary defines the four noise terms with non-overlapping boundaries: background is area
      fill and shaded bands rather than ink; grid lines are regularly spaced thin lines aligned to
      ticks; annotation is any text or arrow overlaid on the chart; border is the plot frame plus
      tick marks.
- [x] The glossary states that the legend is inspected during analysis as a protected region and
      removed before data extraction.
- [x] An architecture decision record captures the choice of an agentic skill framework over
      extending the existing .NET pipeline, naming the alternatives considered and why they lost.
- [x] A second architecture decision record captures the Python plus file-based artifact contract,
      including why bulk coordinate data must never travel through the agent's context.
- [x] Per-image working directories are ignored by version control.
- [x] No existing .NET project is modified.
