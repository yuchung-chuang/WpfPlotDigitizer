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

**Status:** ready-for-agent

- [ ] Running segmentation writes one mask layer per discovered series and a series entry per mask.
- [ ] Clustering runs and produces sensible series when no legend was found.
- [ ] When a legend was found, discovered clusters are matched to legend entries and inherit their
      labels.
- [ ] A mismatch between cluster count and legend entry count is recorded as a diagnostic and
      lowers confidence.
- [ ] Colour clustering operates in a perceptually uniform space.
- [ ] Series sharing a colour but differing in marker shape are separated.
- [ ] Series sharing a marker shape but differing in colour are separated.
- [ ] The legend region is excluded from the segmented plot region.
- [ ] Regions already claimed by noise mask layers are excluded.
- [ ] The agent can merge two clusters or split one, and the change is recorded.
- [ ] The overlay renders each series mask in a distinct colour over the original image.
- [ ] The three heavily overlapping series in the hysteresis corpus plot are separated correctly.
