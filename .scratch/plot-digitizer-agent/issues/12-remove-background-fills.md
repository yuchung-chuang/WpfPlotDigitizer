# 12 — Remove background fills

**What to build:** Coloured panels, gradients and shaded bands stop swamping colour-based
segmentation.

Background is **area, not ink** — that boundary is what keeps this filter from competing with the
others. A large uniform or smoothly varying region is background; a stroke is not.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** needs tuning - uniform panel fills verified

- [x] Running the filter writes a background mask layer and records its pixel count.
- [ ] Uniform fills, smooth gradients and alternating or shaded bands are all detected.
- [x] A non-white page background is handled as readily as a white one.
- [x] Data ink drawn on top of a shaded region survives.
- [x] Filled markers are not mistaken for background regardless of their size.
- [x] The dominant background colour is recorded so later segmentation can exclude it.
- [x] A corpus plot with a shaded panel comes out with the shading gone and the data intact.

**Verified.** `remove_background.py` detects the gray panel in `unnamed-chunk-9-1.png` from outer
page samples, leaves both colored marker series intact, records the page colour and thresholds, and
supports drop/rebuild. Gradient and alternating-band tuning remains open.
