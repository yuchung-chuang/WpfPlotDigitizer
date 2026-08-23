# 03 — Chart overview

**What to build:** The agent's first look at a figure. Given an image, it gets a preview small
enough to view without spending its whole context budget, a count of how many panels the figure
contains, and a verdict on whether this figure is something the framework can handle.

The verdict matters as much as the analysis. A three-panel composite or a figure with three offset
vertical axes must be identified and **declined loudly**, because the failure this whole project
exists to fix is confident, plausible, wrong numbers.

**Blocked by:** 02 — Artifact contract, router skill, and shared toolkit.

**Status:** ready-for-agent

- [ ] A chart overview skill group is discoverable and describes when the agent should reach for it.
- [ ] Running the overview against an image writes the chart classification section of the
      extraction document and produces a downscaled preview the agent can view.
- [ ] Panel candidates are detected and counted.
- [ ] The support verdict distinguishes supported single-panel figures from figures that must be
      declined, and records the reason for a decline as a diagnostic.
- [ ] A multi-panel composite from the corpus is reported as unsupported with its panel count.
- [ ] A figure with multiple offset vertical axes is reported as unsupported.
- [ ] A simple single-series scatter plot from the corpus is reported as supported.
- [ ] The overlay marks the detected panel regions on the original image.
- [ ] A confidence and any diagnostics are recorded for the classification.
