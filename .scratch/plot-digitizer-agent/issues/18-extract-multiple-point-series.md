# 18 — Extract multiple point series

**What to build:** Every scatter series in a segmented figure yields its own point set, carrying the
label the legend gave it, all the way through to export.

Because export was written for any number of series from the start, this ticket needs no export
work — the numbers simply flow through with a populated series column.

**Blocked by:** 06 — Extract a single point series; 17 — Segment the plot into per-series masks.

**Status:** ready-for-agent

- [ ] Running extraction over a segmented figure produces one point set per series.
- [ ] Each series keeps its label and style profile through extraction and export.
- [ ] Each series writes its own sidecar file and the extraction document holds only counts and
      references.
- [ ] Extraction parameters can be tuned for one series without disturbing the others.
- [ ] Markers of one series occluded by another are reported as a diagnostic rather than silently
      dropped.
- [ ] The overlay draws every series' extracted points in its own colour over the original image.
- [ ] Verification passes for all series at once.
- [ ] The hysteresis corpus plot exports three labelled series whose points visibly sit on the
      correct markers.
