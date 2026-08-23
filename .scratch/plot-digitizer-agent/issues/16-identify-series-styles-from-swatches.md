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

**Status:** ready-for-agent

- [ ] Running style identification writes a style profile per legend entry into the extraction
      document.
- [ ] Each profile records colour in a perceptually uniform space, marker shape, marker size, line
      style and line width.
- [ ] Marker shapes across a useful taxonomy are distinguished — at minimum circle, square,
      triangle, diamond, cross and star — and filled is distinguished from open.
- [ ] A swatch showing a line with no marker is profiled as a line-only series.
- [ ] A swatch showing a marker with no line is profiled as a point-only series.
- [ ] A swatch showing both is profiled as carrying both.
- [ ] Solid, dashed, dotted and dash-dot line styles are distinguished.
- [ ] The agent can correct a misidentified style, and the correction is recorded.
- [ ] The overlay presents each entry's swatch beside its interpreted style so the agent can check
      the reading.
- [ ] The three series in the multi-series corpus plot are profiled with distinct signatures.
