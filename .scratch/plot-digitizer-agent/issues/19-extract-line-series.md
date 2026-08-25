# 19 — Extract line series

**What to build:** Curves come out as ordered sequences of points that can be plotted back as a
line, not as an unordered cloud of pixels.

Ordering is the whole difficulty. A line mask must be reduced to a skeleton and then traversed, and
the traversal has to survive three things that break naive approaches: gaps in dashed strokes,
junctions where a curve crosses itself or another series, and endpoints that are ambiguous on a
closed loop.

Crossings are resolved by direction continuity — the branch that best continues the current heading
is the correct one — which is what prevents two intersecting curves from being swapped at the
intersection.

**Blocked by:** 17 — Segment the plot into per-series masks.

**Status:** ready-for-agent

**Verification fixture order (minimal requirements first).**

1. `images/linegraph-3.png` - one solid, unmarked line in a clean single-panel plot; the first
      ordered-polyline and sampling-density check.
2. `images/Screenshot 2024-09-15 131740.png`, `images/Screenshot 2024-09-15 131805.png` and
      `images/Screenshot 2024-09-15 131643.png` - short two-series lines with visible markers, useful
      for confirming that line tracing and point extraction remain distinct.
3. `images/Screenshot 2021-06-26 230901.png` - solid and dashed curves with markers; use for gap
      bridging and annotation exclusion after tickets 13 and 17.
4. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - overlapping
      non-function curves and loop traversal; use for direction continuity at crossings and ambiguous
      endpoints after segmentation.
5. `images/Screenshot 2024-09-15 131712.png` - mixed point series, an oscillation stroke and phase
      annotations; run after tickets 13, 17 and 18 to check that only the intended line is traced.
6. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
   `images/Screenshot 2024-09-15 131401.png`, `images/Screenshot 2024-09-15 131423.png`,
   `images/Screenshot 2024-09-15 131439.png`, `images/Screenshot 2024-09-15 131522.png` and
   `images/Screenshot 2024-09-15 131556.png` - larger multi-line regressions after per-series
   segmentation and any required error-bar filtering.
7. `images/Inseam-v-Height-Graph.jpg` - fitted lines over dense point clouds; a late stress case for
      line-mask isolation, not a point-count acceptance fixture.

`images/3-1-freefall.png`, `images/3-2-freefall-mimimum.png`,
`images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png`, `images/rnaseqdedemo_19.png`,
`images/zivEp.png`, `images/Graph-1.jpg` and `images/Screenshot 2021-06-26 231058.png` are
point-only or scatter-oriented controls and should not be routed to line extraction. The composite
image requires independent panel contexts, while the multiple-axis images require axis-to-series
association before line extraction.

- [ ] Running line extraction over a line series mask produces an ordered sequence of pixel
      coordinates.
- [ ] The sequence is written to a sidecar file and the series is recorded as a line kind.
- [ ] Dashed and dotted strokes are traced as one line, with gaps bridged.
- [ ] A crossing is resolved by direction continuity rather than by arbitrary branch choice.
- [ ] A curve that is not a function of the horizontal axis — such as a hysteresis loop — is traced
      correctly rather than collapsed to one value per column.
- [ ] Sampling density is adjustable.
- [ ] An unresolvable junction is reported as a diagnostic with its pixel location rather than
      guessed silently.
- [ ] A figure containing both line and scatter series extracts both in one pass.
- [ ] The overlay draws the traced polyline with its direction indicated over the original image.
- [ ] A line plot from the corpus traces cleanly end to end.
