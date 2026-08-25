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

For the initial validation pass, use only `3-1-freefall.png` and
`A+cleaned+up+scatter+plot.jpg`. The two screenshot fixtures were checked subsequently, after
regression and connecting-line noise removal, and after grid-line removal, respectively. Use the
point-extraction operation on comparable discrete-marker
charts; route continuous line series, multi-series figures, bars, histograms, and other layouts to
their dedicated operation or ticket. Dense or saturated point clouds such as `data.png` and
`Inseam-v-Height-Graph.jpg` are exploratory stress cases only.

**Status:** in progress — the `230901` acceptance fixture still produces noisy, undercounted output

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png` and `images/A+cleaned+up+scatter+plot.jpg` - clean, individually
      visible marker baselines with no legend or required noise filter.
2. `images/rplot.png` - an additional sparse, open-marker single-series regression case.
3. `images/3-2-freefall-mimimum.png` - a sparse marker case with regular grid lines; run after
      ticket 11 so the grid mask is part of the extraction input.
4. `images/Screenshot 2021-06-26 231058.png` - a single marker series with grid lines; run after
      ticket 11 and compare the overlay against the markers.
5. `images/Screenshot 2021-06-26 230901.png` - a regression-only case containing two marker
      styles, connecting lines and in-plot annotations; use after the relevant border, annotation and
      series-isolation work rather than treating it as a minimal single-series input.
6. `images/data.png` and `images/Inseam-v-Height-Graph.jpg` - dense-cloud stress cases only; use
      the undercount diagnostic and do not treat their counts as acceptance results.

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

**Verified against the supplied ground truth.**

| Fixture | Points | Coverage | Median error | P95 error | Result |
| --- | ---: | ---: | ---: | ---: | --- |
| `3-1-freefall.png` | 11 | 100.0% | 0.14% | 0.14% | PASS |
| `A+cleaned+up+scatter+plot.jpg` | 19 | 100.0% | 0.21% | 0.25% | PASS |
| `Screenshot 2021-06-26 231058.png` | 4 | n/a | n/a | n/a | PASS: command-level check after grid removal |
| `Screenshot 2021-06-26 230901.png` | 1,412 | n/a | n/a | n/a | FAIL: noisy and undercounted; 167 blobs discarded |

Both fixtures are below the `0.50%` acceptance tolerance, and both overlays place the extracted
points on the truth markers. `231058.png` produced four points after grid removal and passed its
command-level check. `230901.png` produced centroid marks across connecting curves and in-plot text
instead of only the discrete markers, so it does not satisfy the visually correct point-set
criterion.

**Exploratory stress case — not an acceptance outcome.** Every mechanism works and the plumbing is
right, but the default parameters badly under-extract saturated point clouds. On `data.png` it kept
274 points and discarded 88, where the figure holds several thousand; on
`Inseam-v-Height-Graph.jpg` it kept 180 of roughly two thousand. The overlay shows why: it recovers
the sparse outlying markers and misses the dense core entirely, because overlapping markers merge
into large blobs which the area z-score then rejects as outliers. The warning diagnostic does fire,
so the undercount is reported rather than silent, but these images are not used to decide ticket 06
completion.

**Dense-cloud scope note.** Distance-transform watershed improved the exploratory results to 982
points on `data.png` and 858 points on `Inseam-v-Height-Graph.jpg`, but both saturated figures
remain undercounted. They are stress cases only and do not determine ticket completion; the clean,
individually visible marker fixtures above determine the acceptance result.
