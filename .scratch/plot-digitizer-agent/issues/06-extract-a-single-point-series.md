# 06 — Extract a single point series

**What to build:** Marker positions come out of a plot as pixel coordinates, accurate to better than
a whole pixel, with obviously-wrong blobs discarded.

This is the extraction half of the tracer bullet: one series, no legend, no segmentation. It should
carry over the two good ideas already proven in the existing code — the contour-moment centroid with
its fallback for degenerate moments, and the morphological opening that strips connecting lines
while leaving markers intact — plus the area outlier rejection from the recognition prototype, which
cheaply discards merged or clipped blobs.

It must consume the combined noise mask **if one is present in the working directory**, and fall
back to the raw plot area when it is not. That way the noise-filtering tickets later improve this
script's results without anyone editing it.

**Blocked by:** 04 — Locate the plot area.

**Status:** needs tuning — mechanically complete, quality not yet acceptable

- [x] A data segmentation skill group is discoverable and describes point extraction.
- [x] Running extraction on a supported plot writes a series entry to the extraction document with
      its point count, mask reference and sidecar file reference.
- [x] Pixel coordinates are written to a per-series sidecar file, never inline in the document.
- [x] Centroids are sub-pixel and the degenerate-moment case is handled rather than producing
      undefined values.
- [x] Connecting lines between markers do not become data points.
- [x] Blobs whose area is a wild outlier are discarded and the count of discards is reported.
- [x] Blobs large enough to be overlapping markers are reported as a diagnostic so the agent knows
      the count is an undercount.
- [x] The combined noise mask is used when present and the raw plot area when absent.
- [x] The overlay draws the extracted centroids over the original image.
- [ ] A simple single-series scatter plot from the corpus yields a visually correct point set.

**Deviation — the last criterion is not met.** Every mechanism works and the plumbing is right, but
the default parameters badly under-extract dense scatter. On `data.png` it kept 274 points and
discarded 88, where the figure holds several thousand; on `Inseam-v-Height-Graph.jpg` it kept 180 of
roughly two thousand. The overlay shows why: it recovers the sparse outlying markers and misses the
dense core entirely, because overlapping markers merge into large blobs which the area z-score then
rejects as outliers. The warning diagnostic does fire, so the undercount is reported rather than
silent — but a reported bad number is still a bad number.

**What the fix probably is**, for whoever picks this up: the area z-score is the wrong tool inside a
saturated cloud, because there the merges *are* the population and the z-score rejects the genuine
markers instead. Try estimating the modal single-marker area first, then splitting blobs by
dividing their area by that mode — or use distance-transform watershed to separate touching markers
before taking centroids. Both are standard and neither needs new dependencies. The median area of
2 px reported on the Inseam figure also suggests the ink threshold is fragmenting anti-aliased
markers, which is worth checking before anything else.
- [ ] A simple single-series scatter plot from the corpus yields a visually correct point set.
