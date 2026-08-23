# 13 — Remove in-plot annotations

**What to build:** Text and arrows drawn inside the plot stop being mistaken for markers.

Annotation is any text or arrow overlaid on the chart — axis labels, fit equations, callouts,
significance stars, arrows pointing at features. The one thing this filter must never touch is the
legend, which is a protected region because a later ticket needs to read it.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** ready-for-agent

- [ ] Running the filter writes an annotation mask layer and records its pixel count.
- [ ] Text inside the plot area is detected regardless of orientation.
- [ ] Arrows and leader lines are detected.
- [ ] The legend is never removed by this filter, whether or not it has been located yet.
- [ ] Any region recorded as protected is left untouched.
- [ ] Dense clusters of small markers are not mistaken for text.
- [ ] A corpus plot carrying an in-plot equation label comes out with the text gone and the data
      intact.
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
