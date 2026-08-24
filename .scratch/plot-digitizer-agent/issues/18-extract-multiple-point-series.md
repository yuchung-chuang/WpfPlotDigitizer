# 18 — Extract multiple point series

**What to build:** Every scatter series in a segmented figure yields its own point set, carrying the
label the legend gave it, all the way through to export.

Because export was written for any number of series from the start, this ticket needs no export
work — the numbers simply flow through with a populated series column.

**Blocked by:** 06 — Extract a single point series; 17 — Segment the plot into per-series masks.

**Status:** ready-for-agent

**Verification fixture order (minimal requirements first).**

1. `images/Screenshot 2024-09-15 131643.png` - two discrete marker series with a small, readable
      legend and separate marker styles.
2. `images/Screenshot 2024-09-15 131740.png` and `images/Screenshot 2024-09-15 131805.png` -
      two-series point-and-line charts that test open/filled marker preservation.
3. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - three
      overlapping point series with different colors and marker shapes.
4. `images/rnaseqdedemo_19.png` and `images/zivEp.png` - colored multi-series scatter cases, with
      the latter requiring the background filter before extraction.
5. `images/Screenshot 2021-06-26 230901.png` - two same-color marker series with connecting lines
      and annotations; run after tickets 13 and 17, then verify both labels and sidecars.
6. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
      `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png`, `images/Screenshot 2024-09-15 131712.png` and
      `images/Screenshot 2024-09-15 131740.png` - larger multi-series regressions after the relevant
      legend/style, noise, annotation and line/point prerequisites. (`131643`, `131740` and `131805`
      are covered above.)
7. `images/Graph-1.jpg`, `images/Inseam-v-Height-Graph.jpg` and `images/data.png` - dense colored
      clouds and fitted lines; use purity, undercount and overlap diagnostics rather than requiring a
      complete marker count.

The single-series fixtures from ticket 06 and `images/linegraph-3.png` are regression controls for
their own extraction paths, not multi-series acceptance fixtures. The multi-panel and multiple-axis
images from ticket 03 must be declined before this operation is attempted.

- [ ] Running extraction over a segmented figure produces one point set per series.
- [ ] Each series keeps its label and style profile through extraction and export.
- [ ] Each series writes its own sidecar file and the extraction document holds only counts and
      references.
- [ ] Extraction parameters can be tuned for one series without disturbing the others.
- [ ] Markers of one series occluded by another are reported as a diagnostic rather than silently
      dropped.
- [ ] The overlay draws every series' extracted points in its own colour over the original image.
- [ ] Verification passes for all series at once.
- [ ] The hysteresis corpus plot exports three labelled series whose points visibly sit on the
      correct markers.
