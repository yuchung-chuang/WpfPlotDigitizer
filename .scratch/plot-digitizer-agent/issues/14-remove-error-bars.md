# 14 — Remove error bars

**What to build:** Whiskers stop dragging centroids away from the true data point.

This is the noise filter that most directly corrupts numbers rather than merely adding clutter. An
error bar shares its colour with its marker, so colour-based segmentation cannot help; the filter
has to recognise the *shape* — a thin segment, usually with a perpendicular cap, attached to a
marker and aligned with an axis.

The marker at the centre must survive. Removing the whole structure would delete the data point the
error bar belongs to.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** needs tuning - capped vertical bars verified

- [x] Running the filter writes an error-bar mask layer and records its pixel count.
- [x] Vertical error bars are detected.
- [ ] Horizontal error bars are detected.
- [x] Capped whiskers are detected.
- [ ] Uncapped whiskers are detected.
- [x] The marker at the centre of an error bar survives.
- [ ] Asymmetric error bars are handled.
- [ ] A thin line data series is not mistaken for a whisker.
- [x] Detected error-bar extents are recorded, so the uncertainty could be exported later.
- [x] The corpus plot carrying error bars comes out with whiskers gone and markers intact, and the
      extracted centroids visibly sit on the markers.

**Verified.** `131341.png` records 107 capped vertical error-bar extents; its overlay shows blue
claims on whiskers and visible marker centres. Thin-line false-positive resistance remains open.
