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

**Verification fixture order (minimal requirements first).**

1. `images/Screenshot 2024-09-15 131341.png` - the existing capped vertical-bar acceptance case;
      verify whiskers are claimed while marker centres remain.
2. `images/Screenshot 2024-09-15 131439.png` - a second multi-series chart with visible vertical
      error bars; use as a preservation and false-positive regression after the first fixture.
3. `images/VLObject-2561-031201081203.png` - geometry-only vertical-bar stress case after ticket 03
      has identified and manually isolated one axis; it is not a supported full-chart fixture because
      the figure has multiple offset vertical axes.

No image in the corpus provides a clean horizontal or uncapped-only error-bar case. Those unchecked
acceptance items cannot be promoted by the current image set, and thin-line false-positive checks
must use the later line-series fixtures rather than a dedicated image.

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
