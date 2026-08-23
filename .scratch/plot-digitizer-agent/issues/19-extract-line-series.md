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
