# 11 — Remove grid lines

**What to build:** Regularly spaced rules across the plot stop being mistaken for data.

The danger here is precise: a grid line and a thin dashed data series look almost identical to a
naive filter. What separates them is regular spacing and alignment with the tick positions, and the
filter must lean on that rather than on thinness alone.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Status:** ready-for-agent

- [ ] Running the filter writes a grid-line mask layer and records its pixel count.
- [ ] Detection uses regular spacing and alignment with known tick positions, not thinness alone.
- [ ] Both major and minor grids are handled, including grids on one axis only.
- [ ] Dotted and dashed grids are detected, not just solid ones.
- [ ] A thin line data series in a gridded corpus plot survives the filter, and the overlay makes it
      obvious whether it did.
- [ ] Aggressiveness is adjustable so the agent can re-run when the overlay shows over-removal.
- [ ] A gridded screenshot from the corpus comes out clean.
