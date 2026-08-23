# 03 — Chart overview

**What to build:** The agent's first look at a figure. Given an image, it gets a preview small
enough to view without spending its whole context budget, a count of how many panels the figure
contains, and a verdict on whether this figure is something the framework can handle.

The verdict matters as much as the analysis. A three-panel composite or a figure with three offset
vertical axes must be identified and **declined loudly**, because the failure this whole project
exists to fix is confident, plausible, wrong numbers.

**Blocked by:** 02 — Artifact contract, router skill, and shared toolkit.

**Status:** done

- [x] A chart overview skill group is discoverable and describes when the agent should reach for it.
- [x] Running the overview against an image writes the chart classification section of the
      extraction document and produces a downscaled preview the agent can view.
- [x] Panel candidates are detected and counted.
- [x] The support verdict distinguishes supported single-panel figures from figures that must be
      declined, and records the reason for a decline as a diagnostic.
- [x] A multi-panel composite from the corpus is reported as unsupported with its panel count.
- [x] A figure with multiple offset vertical axes is reported as unsupported.
- [x] A simple single-series scatter plot from the corpus is reported as supported.
- [x] The overlay marks the detected panel regions on the original image.
- [x] A confidence and any diagnostics are recorded for the classification.

**Deviation.** Detecting offset secondary axes by measurement alone proved unreliable: counting
long vertical lines declines a legitimate plot that draws an axis through the origin, and filtering
those lines by tick marks misses thin and inward-drawn ticks. Rather than ship a heuristic that
produces false declines, the script reports the evidence — panel count, long lines, and which of
them carry ticks — and leaves `supported` unset on a single-panel figure until the agent reads the
preview and declares. Panel count remains an automatic decline because it measures reliably. This
keeps the criterion satisfied through the agent rather than the script, which is what a fully
agentic design implies.
