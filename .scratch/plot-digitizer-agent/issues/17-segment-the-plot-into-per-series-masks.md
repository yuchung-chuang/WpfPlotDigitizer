# 17 — Segment the plot into per-series masks

**What to build:** The core of the whole project — one plot region containing several overlapping
series becomes one mask per series.

**Clustering always runs, whether or not a legend was found.** This inversion is deliberate and it is
what makes the framework robust: most of the corpus has no legend, so a legend-driven design would
lose those figures entirely. The legend's job is to **name and validate** the clusters that
clustering discovered, and to supply the expected count.

Segmentation combines colour clustering in a perceptually uniform space with marker-shape template
matching, because series routinely share one attribute and differ in the other.

The legend is masked out of the plot region here. It has been read by now, and if left in place its
swatches would be digitized as data points.

When the number of clusters found disagrees with the number of legend entries, that is reported, not
silently reconciled.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal; 16 — Identify series
styles from swatches.

**Status:** needs tuning — distinct-colour segmentation is mechanically complete

**Verification fixture order (minimal requirements first).**

1. `images/Screenshot 2024-09-15 131643.png` - two same-color marker families distinguished by
      open-circle versus open-triangle shape; a small, readable separation case.
2. `images/Screenshot 2024-09-15 131740.png` and `images/Screenshot 2024-09-15 131805.png` -
      two-series open/filled square and circle variants with connecting lines.
3. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - the primary
      three-series overlap case with distinct color and marker signatures.
4. `images/rnaseqdedemo_19.png`, `images/zivEp.png` and `images/Graph-1.jpg` - colored clusters
      with legends, increasing density and an outside legend/colorbar.
5. `images/Screenshot 2021-06-26 230901.png` - two same-color square/circle series with lines and
      annotations; run after ticket 13 for a meaningful preservation check.
6. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
      `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png` and `images/Screenshot 2024-09-15 131712.png` -
      larger multi-series regressions after their relevant noise filters, legend styles and any line
      or annotation handling are available.
7. `images/Inseam-v-Height-Graph.jpg` and `images/data.png` - dense-cloud stress cases; measure
      purity and undercount diagnostics rather than expecting complete point recovery.

The multi-panel `images/scatter_and_hist_border.png` and multiple-axis
`images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` and
`images/VLObject-2561-031201081203.png` are decline controls from ticket 03, not segmentation
acceptance fixtures.

- [x] Running segmentation writes one mask layer per discovered series and a series entry per mask.
- [x] Clustering runs and produces sensible series when no legend was found.
- [x] When a legend was found, discovered clusters are matched to legend entries and inherit their
      labels.
- [x] A mismatch between cluster count and legend entry count is recorded as a diagnostic and
      lowers confidence.
- [x] Colour clustering operates in a perceptually uniform space.
- [ ] Series sharing a colour but differing in marker shape are separated.
- [x] Series sharing a marker shape but differing in colour are separated.
- [x] The legend region is excluded from the segmented plot region.
- [x] Regions already claimed by noise mask layers are excluded.
- [x] The agent can merge two clusters or split one, and the change is recorded.
- [x] The overlay renders each series mask in a distinct colour over the original image.
- [x] The three heavily overlapping series in the hysteresis corpus plot are separated correctly.

**Verified.** The hysteresis corpus figure yields three masks for CF, CF300_30, and CF600_180;
the overlay separates the black square, red circle, and blue triangle series and leaves the legend
out of the candidate region. Same-colour marker-shape separation remains the next tuning item.
