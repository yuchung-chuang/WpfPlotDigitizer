# 10 — Noise filter group, mask layers, and border removal

**What to build:** The contract that makes noise removal safe to experiment with, proven by removing
the plot frame and tick marks.

Filters never modify a shared working image. Each contributes its own **named mask layer**, and a
combined mask is derived by union. Any single layer can be dropped or re-run without redoing the
others — which matters enormously when the agent is choosing its own order and will want to undo one
step constantly.

The overlay renders each layer in its own colour. This is not decoration: the most likely failure in
this entire project is an over-eager filter eating a thin data series, and per-layer colouring is
what makes that visible rather than merely suspected.

Border removal is the first filter and the proof: ink touching the plot-area boundary, the frame
itself, and the tick marks, following the flood-fill approach already used in the desktop code.

**Blocked by:** 04 — Locate the plot area.

**Status:** done

- [x] A noise filtering skill group is discoverable and documents the mask-layer contract, the
      union into a combined mask, and the per-layer overlay colouring.
- [x] The group document defines the four noise categories using the glossary's boundaries so two
      filters never claim the same pixels.
- [x] Each filter writes a separate named mask layer and records it in the extraction document with
      its kind and pixel count.
- [x] A combined mask is derived by union and regenerated whenever a layer is added or removed.
- [x] Any single layer can be dropped or re-run with different parameters without affecting others.
- [x] Protected regions recorded in the extraction document are never removed by any filter.
- [x] The overlay renders each mask layer in a distinct colour over the original image.
- [x] Border removal strips the plot frame and tick marks from a framed corpus plot without touching
      data near the edges.
- [x] Running border removal twice produces the same result rather than eroding further.

**Verified.** On `data.png` the filter claimed 12,354 px of frame and 569 px of tick stubs across 19
stubs, wrote `masks/border.png`, and rebuilt `masks/plot_mask.png`. The cross-ticket contract was
confirmed working end to end: with no filter run, point extraction reported "raw plot area - no
noise mask yet"; after the filter ran, the same unmodified script reported "masks/plot_mask.png
minus 1 noise layer(s)". That is the property the whole non-destructive design exists for.

**Ruling recorded.** An axis drawn through the origin is *not* border — it does not bound the plot
area — so `clear_border` leaves it and it is treated as an annotation. Documented in the group
skill so the four sibling filters stay consistent.
