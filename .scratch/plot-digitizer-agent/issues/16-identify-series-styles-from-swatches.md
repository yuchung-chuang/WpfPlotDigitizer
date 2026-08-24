# 16 — Identify series styles from swatches

**What to build:** Each legend swatch becomes a concrete visual signature the framework can hunt for
inside the plot.

A signature is the tuple of colour, marker shape, marker size, line style and line width. Any one of
these alone is insufficient on real figures: some series share a colour and differ only by marker
shape, others share a marker shape and differ only by colour.

Colour must be captured in a perceptually uniform space, not as raw channel values, because the
question being asked is "would a reader call these the same colour?" and channel distance does not
answer it.

**Blocked by:** 15 — Find and crop the legend.

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png` - explicit
      square, circle and triangle marker taxonomy with three distinct colors.
2. `images/Inseam-v-Height-Graph.jpg` - point-only entries alongside line-only fitted entries;
      verify that the profile distinguishes the two kinds.
3. `images/rnaseqdedemo_19.png`, `images/Graph-1.jpg` and `images/zivEp.png` - color-separated
      point styles, including filled square/triangle and colored circular markers.
4. `images/Screenshot 2024-09-15 131643.png`, `images/Screenshot 2024-09-15 131740.png` and
      `images/Screenshot 2024-09-15 131805.png` - open versus filled circle/square/triangle variants
      with connecting lines.
5. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
      `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
      `images/Screenshot 2024-09-15 131624.png` and `images/Screenshot 2024-09-15 131712.png` -
      progressively larger style vocabularies, including line styles and mixed point/line entries.
6. `images/Screenshot 2021-06-26 230901.png` - same-color square/circle series with dashed versus
      solid lines; use after legend-free style correction is supported.

- [x] Running style identification writes a style profile per legend entry into the extraction
      document.
- [x] Each profile records colour in a perceptually uniform space, marker shape, marker size, line
      style and line width.
- [x] Marker shapes across a useful taxonomy are distinguished — at minimum circle, square,
      triangle, diamond, cross and star — and filled is distinguished from open.
- [x] A swatch showing a line with no marker is profiled as a line-only series.
- [x] A swatch showing a marker with no line is profiled as a point-only series.
- [x] A swatch showing both is profiled as carrying both.
- [x] Solid, dashed, dotted and dash-dot line styles are distinguished.
- [x] The agent can correct a misidentified style, and the correction is recorded.
- [x] The overlay presents each entry's swatch beside its interpreted style so the agent can check
      the reading.
- [x] The three series in the multi-series corpus plot are profiled with distinct signatures.

**Verified.** The two entries in `131643.png` are profiled as open circle and open triangle.
The three horizontal entries in `131712.png` produce distinct circle, square, and line-only
signatures. An explicit `--override 0.marker=triangle` is retained under `style_corrections`.
