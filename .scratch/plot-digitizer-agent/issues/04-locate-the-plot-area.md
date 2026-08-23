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

**Status:** done

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

**Corpus sweep.** 30 figures, no crashes. 20 reached two-strategy agreement at confidence 0.90,
including the framed, spines-only, shaded-panel and photographed cases. Four disagreed and were
reported at 0.28-0.40 with both candidate boxes recorded. Four had only one strategy answer, at
0.42-0.60. Two found nothing and reported an error telling the agent to supply `--box`; both are
low-contrast JPEGs, which the spec already classes as best-effort.

**Note for later tickets.** Both candidate boxes are kept under `plot_area.candidates`, so a
downstream step can switch to the rejected one without re-running detection.
