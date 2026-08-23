# 07 — Export digitized data

**What to build:** The researcher gets usable numbers out — a tidy table they can load straight into
their analysis tool, plus the full extraction document when they want to know how much to trust it.

This is where pixel coordinates become data values, using the fitted axis mapping and handling both
linear and logarithmic axes as the existing desktop implementation does.

Write it for **any number of series from the start**, even though only one exists today. The series
column is present with one value in it. This is deliberate: it means the multi-series tickets later
need no export work at all.

**Blocked by:** 05 — Read the axis scale; 06 — Extract a single point series.

**Status:** ready-for-agent

- [ ] An export skill group is discoverable and describes the output artifacts.
- [ ] Running export projects every series' pixel coordinates into data units using the fitted axis
      mapping.
- [ ] Logarithmic and reversed axes project correctly.
- [ ] A tidy table is produced with a series column, so multiple series need no format change.
- [ ] Per-series files are produced as well as the combined table.
- [ ] The extraction document is exported complete with confidences, diagnostics and the stage log.
- [ ] Bulk arrays remain in sidecar files and are not inlined into the exported document.
- [ ] Axis titles and units, where known, appear in the export.
- [ ] Exporting when no axis fit exists fails as a reported diagnostic rather than producing
      meaningless numbers.
- [ ] A simple scatter plot from the corpus exports values that match the chart when spot-checked
      against its printed tick labels.
