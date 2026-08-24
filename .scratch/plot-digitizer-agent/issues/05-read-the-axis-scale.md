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

**Status:** done

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png`,
      `images/linegraph-3.png` and `images/3-2-freefall-mimimum.png` - linear axes with readable titles,
      units and ticks.
2. `images/data.png` - reversed-axis regression case, using the already located plot area and
      supplied tick anchors.
3. `images/Screenshot 2024-09-15 131423.png` - logarithmic horizontal axis; then
      `images/Screenshot 2024-09-15 131522.png` - logarithmic horizontal and vertical axes. These
      require only ticket 04 plus the agent's tick readings, not series segmentation.
4. `images/rnaseqdedemo_19.png` and `images/zivEp.png` - titles containing `log10` or `log2` as
      negative controls: transformed data labels alone must not cause a false logarithmic-axis fit.
5. `images/VLObject-2561-031201081203.png` and
      `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` - axis-fit
      diagnostics only after ticket 03 has identified their multiple-axis layouts; they are not
      supported full-chart verification fixtures.

- [x] Running the crop step produces an upscaled image of the tick-label regions for each axis,
      grouped so the agent can tell which label sits at which pixel position.
- [x] Vertical axis labels are presented in a readable orientation.
- [x] The agent can submit label values with their pixel anchors, and the fit is computed and stored
      in the axes section of the extraction document.
- [x] Each axis records its minimum, maximum, scale type, log base where applicable, reversal flag,
      pixel anchors, fit residual and confidence.
- [x] A logarithmic axis is detected from tick spacing and not assumed.
- [x] A reversed axis produces correct values rather than mirrored ones.
- [x] A large fit residual is recorded as a diagnostic warning the agent to re-read the labels.
- [x] Axis titles and units, when the agent supplies them, are stored alongside the numbers.
- [x] The overlay marks the located label regions and the fitted anchors on the original image.

**Verified.** On `data.png` the crop step found 11 x ticks and 12 y ticks and produced a composite
image pairing every pixel position with its label; those labels were read straight off the crop and
fed back, giving `x = -4.013 .. 6.013` over px 85..834 and `y = -4.020 .. 7.010` over px 567..18 —
the reversed y axis handled correctly, residual zero, and limits extrapolated to the frame rather
than stopping at the outermost tick. The crop being genuinely readable was the criterion that
mattered most, and it is.
