# 07 — Export digitized data

**What to build:** The researcher gets usable numbers out — a tidy table they can load straight into
their analysis tool, plus the full extraction document when they want to know how much to trust it.

This is where pixel coordinates become data values, using the fitted axis mapping and handling both
linear and logarithmic axes as the existing desktop implementation does.

Write it for **any number of series from the start**, even though only one exists today. The series
column is present with one value in it. This is deliberate: it means the multi-series tickets later
need no export work at all.

**Blocked by:** 05 — Read the axis scale; 06 — Extract a single point series.

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg` and `images/rplot.png` -
      one fitted axis pair and one point sidecar; verify the tidy combined table, per-series file and
      complete extraction document.
2. `images/3-2-freefall-mimimum.png` and `images/Screenshot 2021-06-26 231058.png` - export after
      the grid filter and single-series extraction have been verified.
3. `images/linegraph-3.png` - export an ordered line sidecar after ticket 19; confirm line samples
      use the series column just like point samples.
4. `images/Screenshot 2024-09-15 131643.png`, `images/Screenshot 2024-09-15 131740.png`,
      `images/Screenshot 2024-09-15 131805.png` and
      `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - first
      multi-series exports after tickets 15-18, with labels and separate sidecars preserved.
5. `images/rnaseqdedemo_19.png`, `images/zivEp.png`, `images/Graph-1.jpg`,
      `images/Inseam-v-Height-Graph.jpg`, `images/Screenshot 2021-06-26 230901.png`, and all
      truth-bearing `images/Screenshot 2024-09-15 *.png` fixtures - extended multi-series exports
      after their required noise, segmentation and extraction tickets are complete.
6. `images/scatter_and_hist_border.png`, `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png`
      and `images/VLObject-2561-031201081203.png` are decline controls from ticket 03, not export
      acceptance fixtures.

- [x] An export skill group is discoverable and describes the output artifacts.
- [x] Running export projects every series' pixel coordinates into data units using the fitted axis
      mapping.
- [x] Logarithmic and reversed axes project correctly.
- [x] A tidy table is produced with a series column, so multiple series need no format change.
- [x] Per-series files are produced as well as the combined table.
- [x] The extraction document is exported complete with confidences, diagnostics and the stage log.
- [x] Bulk arrays remain in sidecar files and are not inlined into the exported document.
- [x] Axis titles and units, where known, appear in the export.
- [x] Exporting when no axis fit exists fails as a reported diagnostic rather than producing
      meaningless numbers.
- [x] A simple scatter plot from the corpus exports values that match the chart when spot-checked
      against its printed tick labels.

**Note.** The projection maths moved into the shared toolkit rather than living in this script,
because verification and scoring both need the same mapping and the inverse of it. Written for any
number of series from the start, so the multi-series tickets need no export work.
