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

**Status:** ready-for-agent

- [ ] A data segmentation skill group is discoverable and describes point extraction.
- [ ] Running extraction on a supported plot writes a series entry to the extraction document with
      its point count, mask reference and sidecar file reference.
- [ ] Pixel coordinates are written to a per-series sidecar file, never inline in the document.
- [ ] Centroids are sub-pixel and the degenerate-moment case is handled rather than producing
      undefined values.
- [ ] Connecting lines between markers do not become data points.
- [ ] Blobs whose area is a wild outlier are discarded and the count of discards is reported.
- [ ] Blobs large enough to be overlapping markers are reported as a diagnostic so the agent knows
      the count is an undercount.
- [ ] The combined noise mask is used when present and the raw plot area when absent.
- [ ] The overlay draws the extracted centroids over the original image.
- [ ] A simple single-series scatter plot from the corpus yields a visually correct point set.
