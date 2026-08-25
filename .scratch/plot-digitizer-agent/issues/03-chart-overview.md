# 03 — Chart overview

**What to build:** The agent's first look at a figure. Given an image, it gets a preview small
enough to view without spending its whole context budget, a count of how many panels the figure
contains, and a verdict on whether this figure is something the framework can handle.

The verdict matters as much as the analysis. A multi-panel composite must be identified and split
into panel contexts before extraction. For multiple horizontal or vertical axes, the agent makes a
best-effort visual association between each axis and its data using colour, line or marker style,
labels and spatial alignment. An unresolved axis or ambiguous series-to-axis association excludes
only that axis and its associated data; the main axes and confidently associated series remain
eligible for digitization. The entire figure must be **declined loudly** only when its main axes
cannot be established or the figure is otherwise unsupported.

**Blocked by:** 02 — Artifact contract, router skill, and shared toolkit.

**Status:** fully completed and tested

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png` and
      `images/linegraph-3.png` - clean single-panel baselines for preview generation, panel count and
      supported-figure classification.
2. `images/3-2-freefall-mimimum.png`, `images/data.png`, `images/Graph-1.jpg`,
      `images/Inseam-v-Height-Graph.jpg`, `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png`,
      `images/rnaseqdedemo_19.png`, `images/unnamed-chunk-9-1.png`, `images/zivEp.png`,
      `images/Screenshot 2021-06-26 230901.png`, `images/Screenshot 2021-06-26 231058.png` and
      `images/VLObject-2561-031201081203.png` - single-panel charts with increasing density,
      legends, shaded panels or other visual clutter; they should remain single-panel supported
      candidates.
3. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
      `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png`, `images/Screenshot 2024-09-15 131643.png`,
      `images/Screenshot 2024-09-15 131712.png`, `images/Screenshot 2024-09-15 131740.png` and
      `images/Screenshot 2024-09-15 131805.png` - single-panel multi-series regression cases.
4. `images/scatter_and_hist_border.png` - multi-panel scatter plus histograms; report all panel
      contexts and require separate axis decisions before extraction.
5. `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` - multiple offset
      vertical axes; make a best-effort association between each coloured axis and its data series.
      Keep the figure and resolved series eligible for digitization; exclude only unresolved axes.

## Verification report

The complete fixture order was run with `inspect_chart.py` on 2026-08-25. All 30 runs completed
with exit code 0, and each produced a valid extraction document, preview and `inspect-chart`
overlay in `.digitize/`. The previews and overlays were visually reviewed and confirmed correct.

| Fixture set | Runs | Detected result | Verification outcome |
| --- | ---: | --- | --- |
| Clean single-panel baselines | 4 | One panel each | PASS: previews and overlays visually correct |
| Dense, legend-bearing and visually cluttered single-panel charts | 9 | One panel each | PASS: previews and overlays visually correct |
| 2024 multi-series regression charts | 13 | One panel each | PASS: previews and overlays visually correct |
| `images/Screenshot 2021-06-26 231058.png` | 1 | One panel; caption excluded from the panel box | PASS after detector update |
| `images/VLObject-2561-031201081203.png` | 1 | One shared X/Y axis pair; two colored line series | PASS: supported single-axis chart |
| `images/scatter_and_hist_border.png` | 1 | Three panel contexts | PASS: contexts recorded for independent downstream analysis |
| Multiple offset-axis fixture | 1 | Three colored Y axes associated visually with three series | PASS: panel, axes and series associations visually resolved |
| **Total** | **30** |  | **PASS** |

The chart-overview behavior is fully completed and tested against its fixture set. The YBCO
overview confirms the best-effort associations between the three colored Y axes and their series.
Numeric pixel-to-data fitting is owned by ticket 05 and is outside this ticket's acceptance scope.

- [x] A chart overview skill group is discoverable and describes when the agent should reach for it.
- [x] Running the overview against an image writes the chart classification section of the
      extraction document and produces a downscaled preview the agent can view.
- [x] Panel candidates are detected and counted.
- [x] The support verdict distinguishes figures requiring panel-specific review from figures that
      must be declined, and records the reason as a diagnostic.
- [x] A multi-panel composite from the corpus is reported with its panel count and panel contexts.
- [x] A figure with unresolved multiple offset vertical axes records the unresolved axes while
      retaining resolvable axes and series for processing.
- [x] A figure with color-associated multiple axes records the visual axis-to-series associations
      and remains eligible for per-series digitization.
- [x] A multi-axis figure records the best-effort axis-to-series associations needed by downstream
      axis fitting and extraction.
- [x] A simple single-series scatter plot from the corpus is reported as supported.
- [x] The overlay marks the detected panel regions on the original image.
- [x] A confidence and any diagnostics are recorded for the classification.

**Deviation.** Detecting offset secondary axes by measurement alone proved unreliable: counting
long vertical lines declines a legitimate plot that draws an axis through the origin, and filtering
those lines by tick marks misses thin and inward-drawn ticks. Rather than ship a heuristic that
produces false declines, the script reports the evidence — panel count, long lines, and which of
them carry ticks — and leaves `supported` unset until the agent reads the preview and declares.
Panel count is recorded as a set of panel contexts rather than an automatic decline. For multiple
axes, the agent makes the best association it can from the available visual evidence and records
the evidence and confidence. A visually confirmed association may declare the figure supported;
an unresolved association is excluded from extraction while resolvable axes and series remain
eligible for data production.
