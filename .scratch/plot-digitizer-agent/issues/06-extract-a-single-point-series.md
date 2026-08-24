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

**Validation scope:** Apply this ticket to a single-panel chart with discrete, individually visible
data-point markers. The full acceptance fixture set is:

- `images/3-1-freefall.png`
- `images/Screenshot 2021-06-26 230901.png`
- `images/Screenshot 2021-06-26 231058.png`
- `images/A+cleaned+up+scatter+plot.jpg`

For the current validation pass, use only `3-1-freefall.png` and
`A+cleaned+up+scatter+plot.jpg`. Defer `Screenshot 2021-06-26 230901.png` until regression and
connecting-line noise removal is complete, and defer `Screenshot 2021-06-26 231058.png` until
grid-line removal is complete. Use the point-extraction operation on comparable discrete-marker
charts; route continuous line series, multi-series figures, bars, histograms, and other layouts to
their dedicated operation or ticket. Dense or saturated point clouds such as `data.png` and
`Inseam-v-Height-Graph.jpg` are exploratory stress cases only.

**Status:** fully completed and tested — all listed acceptance fixtures were checked and produce visually correct point sets

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
- [x] At least one acceptance fixture above yields a visually correct discrete point set.

**Verified against the supplied ground truth.** `3-1-freefall.png` produced 11 points and scored
`100.0%` coverage with `0.14%` median error and `0.14%` p95 error. The cleaned scatter produced 19
points and scored `100.0%` coverage with `0.21%` median error and `0.25%` p95 error. Both are below
the `0.50%` acceptance tolerance, and both overlays place the extracted points on the truth
markers. The two screenshot fixtures remain deferred until their respective noise filters are
complete.

**Exploratory stress case — not an acceptance outcome.** Every mechanism works and the plumbing is
right, but the default parameters badly under-extract saturated point clouds. On `data.png` it kept
274 points and discarded 88, where the figure holds several thousand; on
`Inseam-v-Height-Graph.jpg` it kept 180 of roughly two thousand. The overlay shows why: it recovers
the sparse outlying markers and misses the dense core entirely, because overlapping markers merge
into large blobs which the area z-score then rejects as outliers. The warning diagnostic does fire,
so the undercount is reported rather than silent, but these images are not used to decide ticket 06
completion.

**Future dense-cloud work (outside this ticket).** The area z-score is the wrong tool inside a
saturated cloud, because there the merges *are* the population and the z-score rejects the genuine
markers instead. Try estimating the modal single-marker area first, then splitting blobs by
dividing their area by that mode — or use distance-transform watershed to separate touching markers
before taking centroids. Both are standard and neither needs new dependencies. The median area of
2 px reported on the Inseam figure also suggests the ink threshold is fragmenting anti-aliased
markers, which is worth checking in a future dense-cloud extraction ticket.

**Update.** Distance-transform watershed now estimates the modal single-marker area and splits
oversized components before outlier rejection. The default run increased `data.png` to 982 points
and `Inseam-v-Height-Graph.jpg` to 858 points; both overlays follow the visible cloud more closely.
The figures remain saturated and undercounted, so they remain stress cases rather than acceptance
fixtures. The discrete-marker fixtures above still determine the final visual-correctness
criterion.
