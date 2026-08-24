# 11 — Remove grid lines

**What to build:** Regularly spaced rules across the plot stop being mistaken for data.

The danger here is precise: a grid line and a thin dashed data series look almost identical to a
naive filter. What separates them is regular spacing and alignment with the tick positions, and the
filter must lean on that rather than on thinness alone.

**Blocked by:** 10 — Noise filter group, mask layers, and border removal.

**Validation scope:** Test the completed skill and workflow only against these two fixtures:
`Screenshot 2021-06-26 231058.png` and `3-2-freefall-mimimum.png`. Both fixtures contain grid lines
and a single data series. Do not validate this ticket against any other image; other images are not
applicable to this skill.

**Status:** needs tuning - grid mask is mechanically complete

**Verification fixtures (scope locked; minimal fixture first).**

1. `images/3-2-freefall-mimimum.png` - sparse diamond markers with regular grid lines; verify the
      grid mask leaves every marker intact.
2. `images/Screenshot 2021-06-26 231058.png` - the second and more realistic scoped fixture; verify
      the grid mask, extraction overlay and point count together.

No other image in `images/` is an acceptance fixture for this ticket. Other grid-bearing figures
are reserved for their owning multi-feature tickets or for the corpus pass after this filter is
verified.

- [x] Running the filter writes a grid-line mask layer and records its pixel count.
- [x] Detection uses regular spacing and alignment with known tick positions, not thinness alone.
- [x] Both major and minor grids are handled, including grids on one axis only.
- [x] Dotted and dashed grids are detected, not just solid ones.
- [ ] The single data series in each scoped gridded fixture survives the filter, and the overlay
      makes it obvious whether it did.
- [x] Aggressiveness is adjustable so the agent can re-run when the overlay shows over-removal.
- [ ] `Screenshot 2021-06-26 231058.png` and `3-2-freefall-mimimum.png` come out clean within the
      scoped workflow.

**Verified.** No scoped validation has been run yet. Validation must use only the two fixtures
listed above.
