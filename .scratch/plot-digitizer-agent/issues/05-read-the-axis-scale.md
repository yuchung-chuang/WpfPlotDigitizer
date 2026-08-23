# 05 — Read the axis scale

**What to build:** The agent can turn pixel positions into real data values, because it reads the
tick labels itself rather than trusting a text-recognition engine.

The script does the two things the agent cannot: it finds the tick-label regions around the plot
area and crops them, upscaled, into an image the agent can actually read; then, once the agent hands
back the values it read together with their pixel positions, it fits the mapping.

The fit must handle logarithmic axes, detected from the spacing of the tick positions rather than
assumed, and reversed axes where values decrease left to right. It must report its residual, so a
misread digit surfaces as a bad fit instead of a plausible wrong answer.

**Blocked by:** 04 — Locate the plot area.

**Status:** ready-for-agent

- [ ] Running the crop step produces an upscaled image of the tick-label regions for each axis,
      grouped so the agent can tell which label sits at which pixel position.
- [ ] Vertical axis labels are presented in a readable orientation.
- [ ] The agent can submit label values with their pixel anchors, and the fit is computed and stored
      in the axes section of the extraction document.
- [ ] Each axis records its minimum, maximum, scale type, log base where applicable, reversal flag,
      pixel anchors, fit residual and confidence.
- [ ] A logarithmic axis is detected from tick spacing and not assumed.
- [ ] A reversed axis produces correct values rather than mirrored ones.
- [ ] A large fit residual is recorded as a diagnostic warning the agent to re-read the labels.
- [ ] Axis titles and units, when the agent supplies them, are stored alongside the numbers.
- [ ] The overlay marks the located label regions and the fitted anchors on the original image.
