# 13 — Remove in-plot annotations

**What to build:** Text and arrows drawn inside the plot stop being mistaken for markers.

Annotation is any text or arrow overlaid on the chart — axis labels, fit equations, callouts,
significance stars, arrows pointing at features. The one thing this filter must never touch is the
legend, which is a protected region because a later ticket needs to read it.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** needs tuning - text masking is conservative; arrow cases remain open

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png` - negative control; titles and axis labels outside the plot area must
      remain untouched and must not be claimed as in-plot annotations.
2. `images/Screenshot 2021-06-26 230901.png` - in-plot labels, fit annotations and downward arrows;
      inspect the annotation mask after ticket 04, then check data preservation after series isolation.
3. `images/Screenshot 2024-09-15 131624.png` - in-plot equation/group labels and dashed ellipses;
      run after tickets 15-17 so the legend is protected and the marker series can be checked.
4. `images/Screenshot 2024-09-15 131401.png` - region labels and directional arrows over three
      curves; full preservation requires the grid, segmentation and extraction prerequisites.
5. `images/Screenshot 2024-09-15 131712.png` - phase labels and arrows over a mixed point/line
      figure; run only after tickets 17-19 so the oscillation stroke is not mistaken for annotation.

The corpus has no clean rotated-text-only fixture and no separate arrow-only fixture. The first
fixture is therefore a required false-positive control, while the remaining cases are multi-feature
regressions rather than minimal tests.

- [x] Running the filter writes an annotation mask layer and records its pixel count.
- [ ] Text inside the plot area is detected regardless of orientation.
- [ ] Arrows and leader lines are detected.
- [x] The legend is never removed by this filter, whether or not it has been located yet.
- [x] Any region recorded as protected is left untouched.
- [x] Dense clusters of small markers are not mistaken for text.
- [x] A corpus plot carrying an in-plot equation label comes out with the text gone and the data
      intact.

**Verified.** `131624.png` claims the equation and in-plot labels as glyph pixels, leaves the
protected legend untouched, and does not tint marker-sized components. Arrow detection remains
gated on a geometric arrowhead and needs a corpus example.
# 13 — Remove in-plot annotations

**What to build:** Text and arrows drawn over the chart stop being digitized as data points.

Annotation is any text or arrow overlaid on the chart — in-plot labels, fit equations, callouts,
significance markers, arrows. The one thing this filter must never touch is the legend, which is
recorded as a protected region precisely because a later ticket needs to read it.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** ready-for-agent

- [ ] Running the filter writes an annotation mask layer and records its pixel count.
- [ ] Text inside the plot area is detected, including rotated text.
- [ ] Arrows and callout leaders are detected.
- [ ] The legend is never removed by this filter, whether or not it has yet been located.
- [ ] Any region recorded as protected is left untouched.
- [ ] Dense clusters of small markers are not mistaken for text.
- [ ] A corpus plot carrying an in-plot label comes out with the label gone and the data intact.
