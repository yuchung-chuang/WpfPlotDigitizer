# 14 — Remove error bars

**What to build:** Whiskers stop dragging centroids away from the true data point.

This is the noise filter that most directly corrupts numbers rather than merely adding clutter. An
error bar shares its colour with its marker, so colour-based segmentation cannot help; the filter
has to recognise the *shape* — a thin segment, usually with a perpendicular cap, attached to a
marker and aligned with an axis.

The marker at the centre must survive. Removing the whole structure would delete the data point the
error bar belongs to.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** ready-for-agent

- [ ] Running the filter writes an error-bar mask layer and records its pixel count.
- [ ] Vertical and horizontal error bars are both detected.
- [ ] Capped and uncapped whiskers are both detected.
- [ ] The marker at the centre of an error bar survives.
- [ ] Asymmetric error bars are handled.
- [ ] A thin line data series is not mistaken for a whisker.
- [ ] Detected error-bar extents are recorded, so the uncertainty could be exported later.
- [ ] The corpus plot carrying error bars comes out with whiskers gone and markers intact, and the
      extracted centroids visibly sit on the markers.
