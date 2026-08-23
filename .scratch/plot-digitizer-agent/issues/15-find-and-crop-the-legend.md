# 15 — Find and crop the legend

**What to build:** The agent can read what the series in a figure are called, because the legend is
located and its entries are cropped into pieces the agent can actually see.

Each entry yields two crops: the **swatch**, which carries the visual style, and the **label**, which
carries the name. Keeping them separate is what lets a later ticket pair a style with a name.

Legends sit inside the plot area on some figures and outside it on others, and both must be found.

The moment the legend is located, its box is recorded as a **protected region**, so the noise filters
cannot destroy it before it has been read.

**Blocked by:** 04 — Locate the plot area.

**Status:** ready-for-agent

- [ ] Running legend detection writes the legend section of the extraction document with the box,
      per-entry swatch boxes and per-entry label boxes.
- [ ] Legends inside the plot area are found.
- [ ] Legends outside the plot area, including in the margin, are found.
- [ ] Vertically stacked and horizontally arranged legends are both handled.
- [ ] Upscaled crops of the label regions are produced for the agent to read.
- [ ] Swatch crops are produced separately from label crops.
- [ ] The legend box is recorded as a protected region.
- [ ] A figure with no legend reports that cleanly with low confidence and no diagnostics implying
      failure, because most of the corpus has no legend.
- [ ] The overlay marks the legend box and each entry on the original image.
- [ ] The multi-series corpus plot with a boxed in-plot legend yields the correct number of entries.
