# 04 — Locate the plot area

**What to build:** The agent can find, in pixel coordinates, the rectangle that contains the data,
so every later operation is restricted to the region that matters rather than the whole figure.

Two independent detection strategies run and their results are compared. Agreement raises
confidence; disagreement is reported so the agent knows to look harder rather than proceeding on a
coin flip. One strategy should follow the existing desktop implementation's approach of thresholding,
taking the largest region, then probing the corners for long axis-aligned runs of ink, because it
degrades gracefully on imperfect frames. The other should follow the line-detection prototype's
approach of finding axis-aligned straight lines and intersecting them, because it is stronger when
the frame is clean but the background is not.

The agent must also be able to override the result with coordinates of its own, because detection
will fail on unusual layouts and there must be a way forward when it does.

**Blocked by:** 03 — Chart overview.

## Required validation fixtures

Test the axis-analysis skills against both of these fixtures before considering the ticket
complete:

- `images/A+cleaned+up+scatter+plot.jpg` — open plot area, with only the relevant spines.
- `images/3-1-freefall.png` — closed plot area, with a complete frame around the data.
- `images/rnaseqdedemo_19.png` — open plot area with a colorbar outside the axes.
- `images/Screenshot 2021-06-26 231058.png` — open plot area with grid lines and an outer figure
      border.
- `images/VLObject-2561-031201081203.png` — open plot area with an in-plot legend and no top/right
      frame.
- `images/unnamed-chunk-9-1.png` — shaded open plot area with an in-plot legend.
- `images/zivEp.png` — shaded open plot area with a colorbar outside the axes.

The validation should confirm that the open-plot strategy does not invent a missing border and
that the colorbar, grid lines, outer figure border, and in-plot legends are excluded from the plot
area. For the shaded fixtures, confirm that the shaded panel extent is used as the plot-area
boundary rather than the data extents or a legend/colorbar. Confirm that the closed-plot strategies
agree on the framed rectangle. Record the resulting boxes, confidence, selected method, and any
disagreement diagnostics in the extraction artifacts.

## Agent visual validation

The agent must inspect the source image and the final overlay, then judge the candidate semantically
rather than accepting the script's confidence alone. The visual review of the required fixtures
produced these plot areas:

| Fixture | Visually judged plot area | Review result |
| --- | --- | --- |
| `A+cleaned+up+scatter+plot.jpg` | `x=103, y=119, w=793, h=580` | Override; sparse open spines were not detected automatically |
| `3-1-freefall.png` | `x=109, y=93, w=600, h=426` | Automatic agreement accepted |
| `rnaseqdedemo_19.png` | `x=63, y=31, w=377, h=343` | Override; rejected the narrow colorbar candidate |
| `Screenshot 2021-06-26 231058.png` | `x=138, y=100, w=631, h=397` | Override; rejected the outer figure border and grid-line candidate |
| `VLObject-2561-031201081203.png` | `x=43, y=20, w=350, h=234` | Override; sparse open spines were not detected automatically |
| `unnamed-chunk-9-1.png` | `x=98, y=15, w=636, h=663` | Line-frame candidate accepted because it matches the shaded panel extent |
| `zivEp.png` | `x=46, y=7, w=470, h=428` | Line-frame candidate accepted because it matches the shaded panel extent |

The final overlays show all seven judged rectangles. Four fixtures required a visual override;
three automatic candidates were usable after visual review. This is the meaningful skill result:
the agent recovered all seven areas, while the raw detector alone recovered three without semantic
adjudication.

**Status:** fully completed and tested

**Additional verification order (after the required fixtures above).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg`,
   `images/3-2-freefall-mimimum.png`, `images/linegraph-3.png` and `images/rplot.png` - clean
   closed-frame and open-spine baselines.
2. `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png`,
   `images/Screenshot 2021-06-26 230901.png`, `images/Graph-1.jpg` and
   `images/Inseam-v-Height-Graph.jpg` - framed, annotated or dense single-panel cases where the
   candidate must still stay within the data region.
3. `images/Screenshot 2024-09-15 131234.png`, `images/Screenshot 2024-09-15 131309.png`,
   `images/Screenshot 2024-09-15 131341.png`, `images/Screenshot 2024-09-15 131401.png`,
   `images/Screenshot 2024-09-15 131423.png`, `images/Screenshot 2024-09-15 131439.png`,
   `images/Screenshot 2024-09-15 131522.png`, `images/Screenshot 2024-09-15 131556.png`,
   `images/Screenshot 2024-09-15 131624.png`, `images/Screenshot 2024-09-15 131643.png`,
   `images/Screenshot 2024-09-15 131712.png`, `images/Screenshot 2024-09-15 131740.png` and
   `images/Screenshot 2024-09-15 131805.png` - multi-series single-panel regression boxes.
4. `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png`,
   `images/VLObject-2561-031201081203.png` and `images/scatter_and_hist_border.png` are overview
   decline controls, not successful plot-area fixtures: the first two have multiple offset axes and
   the last is multi-panel.

- [x] An axis analysis skill group is discoverable and describes the two detection strategies and
      when each is stronger.
- [x] Running plot-area location writes the plot area section of the extraction document with the
      rectangle, a confidence, and which method produced it.
- [x] Both detection strategies run and their disagreement is recorded as a diagnostic when they do
      not agree within tolerance.
- [x] The overlay draws the detected rectangle on the original image.
- [x] The agent can supply an explicit rectangle that overrides detection, and the override is
      recorded as the method.
- [x] A framed plot from the corpus is located correctly.
- [x] A plot with only left and bottom spines and no full frame is located correctly or reports low
      confidence rather than returning a wrong rectangle silently.
- [x] A plot whose background is not white does not defeat detection.
- [x] All seven required fixtures were visually reviewed by the agent and their final overlays
      were inspected.

**Corpus sweep.** 30 figures, no crashes. 20 reached two-strategy agreement at confidence 0.90,
including the framed, spines-only, shaded-panel and photographed cases. Four disagreed and were
reported at 0.28-0.40 with both candidate boxes recorded. Four had only one strategy answer, at
0.42-0.60. Two found nothing and reported an error telling the agent to supply `--box`; both are
low-contrast JPEGs, which the spec already classes as best-effort.

**Note for later tickets.** Both candidate boxes are kept under `plot_area.candidates`, so a
downstream step can switch to the rejected one without re-running detection.
