# 15 — Find and crop the legend

**What to build:** The agent can read what the series in a figure are called, because the legend is
located and its entries are cropped into pieces the agent can actually see.

Each entry yields two crops: the **swatch**, which carries the visual style, and the **label**, which
carries the name. Keeping them separate is what lets a later ticket pair a style with a name.

Legends sit inside the plot area on some figures and outside it on others, and both must be found.

The moment the legend is located, its box is recorded as a **protected region**, so the noise filters
cannot destroy it before it has been read.

**Blocked by:** 04 — Locate the plot area.

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - boxed,
      vertically stacked in-plot legend with three entries.
2. `images/Graph-1.jpg` - boxed legend in the margin, outside the plot area.
3. `images/Inseam-v-Height-Graph.jpg` - horizontally arranged legend containing point and fitted-line
      entries below the plot.
4. `images/rnaseqdedemo_19.png`, `images/unnamed-chunk-9-1.png` and `images/zivEp.png` - inside,
      inside and outside legend/colorbar layouts, respectively; preserve the plot boundary and record
      the legend as protected.
5. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
      `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png`, `images/Screenshot 2024-09-15 131643.png`,
      `images/Screenshot 2024-09-15 131712.png`, `images/Screenshot 2024-09-15 131740.png` and
      `images/Screenshot 2024-09-15 131805.png` - larger in-plot or margin legends for regression.
6. `images/3-1-freefall.png`, `images/3-2-freefall-mimimum.png`,
      `images/A+cleaned+up+scatter+plot.jpg`, `images/linegraph-3.png`,
      `images/Screenshot 2021-06-26 230901.png` and `images/Screenshot 2021-06-26 231058.png` -
      legend-free controls that must report an ordinary low-confidence result.
7. `images/VLObject-2561-031201081203.png` - legend-detection stress case only; its multiple axes
      make it unsuitable for full digitization.

- [x] Running legend detection writes the legend section of the extraction document with the box,
      per-entry swatch boxes and per-entry label boxes.
- [x] Legends inside the plot area are found.
- [x] Legends outside the plot area, including in the margin, are found.
- [x] Vertically stacked and horizontally arranged legends are both handled.
- [x] Upscaled crops of the label regions are produced for the agent to read.
- [x] Swatch crops are produced separately from label crops.
- [x] The legend box is recorded as a protected region.
- [x] A figure with no legend reports that cleanly with low confidence and no diagnostics implying
      failure, because most of the corpus has no legend.
- [x] The overlay marks the legend box and each entry on the original image.
- [x] The multi-series corpus plot with a boxed in-plot legend yields the correct number of entries.

**Verified.** `131643.png` yields two vertically stacked entries and `131712.png` yields three
horizontally arranged entries. Both overlays and representative label crops were inspected. A
legend-free figure reports an ordinary low-confidence result.
