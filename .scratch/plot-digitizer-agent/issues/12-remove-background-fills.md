# 12 — Remove background fills

**What to build:** Coloured panels, gradients and shaded bands stop swamping colour-based
segmentation.

Background is **area, not ink** — that boundary is what keeps this filter from competing with the
others. A large uniform or smoothly varying region is background; a stroke is not.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** needs tuning - uniform panel fills verified

**Verification fixture order (minimal requirements first).**

1. `images/unnamed-chunk-9-1.png` - uniform gray panel with two colored marker series; verify the
	panel is claimed while both data colors survive.
2. `images/zivEp.png` - non-white gray plotting panel with two colored point groups and an outside
	colorbar; verify the colorbar is outside the plot-area mask and the data remains.
3. `images/scatter_and_hist_border.png` - gray-free composite control only after ticket 03 has
	declined it; do not use it as a background-filter acceptance result because it is multi-panel.

The corpus has no dedicated smooth-gradient or alternating-band image. Those two acceptance cases
remain unverified until a suitable fixture is added; the two shaded-panel images above are the
minimal available corpus tests.

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
