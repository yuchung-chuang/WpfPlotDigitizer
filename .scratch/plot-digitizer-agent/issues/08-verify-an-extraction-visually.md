# 08 — Verify an extraction visually

**What to build:** The closing check of the whole workflow: the extracted data is projected back
onto the original image so both the agent and the researcher can see whether it landed on the actual
markers or somewhere else.

This is the only assertion the framework has. There is no automated suite by decision, so this
overlay *is* the test, and it must be good enough to judge by eye.

**Blocked by:** 07 — Export digitized data.

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png` and `images/A+cleaned+up+scatter+plot.jpg` - clean single-series
      overlays; the reprojected points should land on the visible markers.
2. `images/rplot.png`, `images/3-2-freefall-mimimum.png` and
      `images/Screenshot 2021-06-26 231058.png` - sparse point overlays, with the latter two run only
      after their grid masks have been checked.
3. `images/linegraph-3.png` - the first ordered-polyline overlay after ticket 19.
4. `images/Screenshot 2024-09-15 131643.png`, `images/Screenshot 2024-09-15 131740.png`,
      `images/Screenshot 2024-09-15 131805.png` and
      `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - distinct
      multi-series overlays after segmentation and per-series extraction.
5. `images/Screenshot 2021-06-26 230901.png`, `images/rnaseqdedemo_19.png`,
      `images/zivEp.png`, `images/Graph-1.jpg`, `images/Inseam-v-Height-Graph.jpg` and the
      truth-bearing 2024 screenshot fixtures - full regression overlays after their annotation,
      background, error-bar, segmentation and line/point prerequisites.
6. `images/scatter_and_hist_border.png`, `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png`
      and `images/VLObject-2561-031201081203.png` must remain decline controls, not be given a
      misleading verification overlay.

- [x] A review skill group is discoverable and states that the agent must view the verification
      overlay before declaring the digitization complete.
- [x] Running verification reads the exported data values, projects them back through the axis
      mapping into pixels, and draws them on the original image.
- [x] Each series is drawn distinctly so they can be told apart.
- [x] A summary reports the point count per series and the mean distance between each reprojected
      point and the nearest ink in the plot area.
- [x] A large reprojection error is reported as a diagnostic identifying the likely stage at fault.
- [x] Deliberately corrupting the axis fit produces a visibly wrong overlay and a raised error
      figure, confirming the check actually detects failure.
- [x] The simple scatter plot from the corpus verifies cleanly. **Milestone: one chart digitized end
      to end.**

**Verified.** `131643.png` was exported and reprojected with distinct overlay points. A normal
run reports the point-to-ink distance; changing the X-axis fit to a deliberately shifted mapping
raises the configured error diagnostic and changes the overlay visibly.
