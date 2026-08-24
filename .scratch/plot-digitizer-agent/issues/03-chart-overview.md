# 03 — Chart overview

**What to build:** The agent's first look at a figure. Given an image, it gets a preview small
enough to view without spending its whole context budget, a count of how many panels the figure
contains, and a verdict on whether this figure is something the framework can handle.

The verdict matters as much as the analysis. A three-panel composite or a figure with three offset
vertical axes must be identified and **declined loudly**, because the failure this whole project
exists to fix is confident, plausible, wrong numbers.

**Blocked by:** 02 — Artifact contract, router skill, and shared toolkit.

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png` and
      `images/linegraph-3.png` - clean single-panel baselines for preview generation, panel count and
      supported-figure classification.
2. `images/3-2-freefall-mimimum.png`, `images/data.png`, `images/Graph-1.jpg`,
      `images/Inseam-v-Height-Graph.jpg`, `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png`,
      `images/rnaseqdedemo_19.png`, `images/unnamed-chunk-9-1.png`, `images/zivEp.png`,
      `images/Screenshot 2021-06-26 230901.png` and `images/Screenshot 2021-06-26 231058.png` -
      single-panel charts with increasing density, legends, shaded panels or other visual clutter;
      they should remain single-panel supported candidates.
3. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
      `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png`, `images/Screenshot 2024-09-15 131643.png`,
      `images/Screenshot 2024-09-15 131712.png`, `images/Screenshot 2024-09-15 131740.png` and
      `images/Screenshot 2024-09-15 131805.png` - single-panel multi-series regression cases.
4. `images/scatter_and_hist_border.png` - multi-panel scatter plus histograms; must be declined with
      its panel count rather than sent to extraction.
5. `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` and
      `images/VLObject-2561-031201081203.png` - multiple offset vertical axes; must be declined as
      unsupported even though each contains a visually bounded plotting region.

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
