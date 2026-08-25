# 05 — Read the axis scale

**What to build:** The agent can turn pixel positions into real data values, because it reads the
tick labels itself rather than trusting a text-recognition engine.

The script does the two things the agent cannot: it finds the tick-label regions around the plot
area and crops them, upscaled, into an image the agent can actually read; then, once the agent hands
back the values it read together with their pixel positions, it fits the mapping.

The fit must handle logarithmic axes, detected from the spacing of the tick positions rather than
assumed, and reversed axes where values decrease left to right. It must report its residual, so a
misread digit surfaces as a bad fit instead of a plausible wrong answer. A figure may have several
horizontal or vertical axes: fit each one independently and preserve its axis identity and mapping
metadata. Series association is handled by the later extraction tickets.

**Blocked by:** 04 — Locate the plot area.

**Status:** fully completed and tested

**Verification fixture order (minimal requirements first).**

1. `images/3-1-freefall.png`, `images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png`,
      `images/linegraph-3.png` and `images/3-2-freefall-mimimum.png` - linear axes with readable titles,
      units and ticks.
2. `images/data.png` - reversed-axis regression case, using the already located plot area and
      supplied tick anchors.
3. `images/Screenshot 2024-09-15 131423.png` - linear horizontal and vertical axes; then
      `images/Screenshot 2024-09-15 131522.png` - logarithmic horizontal and vertical axes. These
      require only ticket 04 plus the agent's tick readings, not series segmentation.
4. `images/rnaseqdedemo_19.png` and `images/zivEp.png` - titles containing `log10` or `log2` as
      negative controls: transformed data labels alone must not cause a false logarithmic-axis fit.
5. `images/VLObject-2561-031201081203.png` - single-panel control with two colored line series
      sharing one X/Y mapping; retain as a normal axis-fit regression.
6. `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` - multi-axis
      acceptance fixture. Fit the shared reversed X axis and each colored Y axis independently:
      green deposition pressure, red annealing temperature and blue Delta T_c. Each mapping must
      retain its axis identity and mapping metadata.

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
- [x] Multiple horizontal or vertical axes can each be fitted and stored without overwriting one
      another.
- [x] Each fitted axis retains its axis identifier, role, colour, supplied title/unit, and fit
      metadata.

## Verification results

The crop and fit steps were run against the single-axis fixtures in the verification set. The
reported limits are extrapolated to the detected plot-area edges; residuals are root-mean-square
pixel residuals from the supplied tick anchors.

| Fixture | X scale and fitted limits | Y scale and fitted limits | X residual | Y residual |
| --- | --- | --- | ---: | ---: |
| `3-1-freefall.png` | linear, `0 .. 0.8` | linear, `-0.00025 .. 1.60025` | 0.00 px | 0.28 px |
| `A+cleaned+up+scatter+plot.jpg` | linear, `-0.507862 .. 50.0545` | linear, `-0.876369 .. 49.734` | 0.44 px | 0.57 px |
| `rplot.png` | linear, `1.45614 .. 5.23684` | linear, `41.0714 .. 98.0952` | 0.00 px | 0.00 px |
| `linegraph-3.png` | linear, `24.6393 .. 34.3532` | linear, `48.5654 .. 60.4486` | 0.28 px | 0.34 px |
| `3-2-freefall-mimimum.png` | linear, `0 .. 0.8` | linear, `-0.00025 .. 1.60025` | 0.00 px | 0.28 px |
| `data.png` | linear, `-4.00729 .. 6.00608` | linear, `-3.99435 .. 7.00103` | 0.46 px | 0.50 px |
| `Screenshot 2024-09-15 131423.png` | linear, `-3.99633 .. 3.9987` | linear, `249.713 .. 500.287` | 0.26 px | 0.00 px |
| `Screenshot 2024-09-15 131522.png` | log base 10, `1.00258 .. 100.258` | log base 10, `9.80121e-7 .. 0.103083` | 0.24 px | 1.37 px |
| `rnaseqdedemo_19.png` | linear, `-4.88948 .. 19.9779` | linear, `-5.99204 .. 3.99204` | 0.30 px | 0.28 px |
| `zivEp.png` | linear, `1.42213 .. 5.27459` | linear, `40.4898 .. 98.7211` | 0.00 px | 0.37 px |
| `VLObject-2561-031201081203.png` | linear, `1955.02 .. 2001.87` | linear, `309.76 .. 380.03` | 0.24 px | 0.22 px |

The results confirm that `Screenshot 2024-09-15 131423.png` has linear axes. The `log2` text in
`rnaseqdedemo_19.png` is part of the transformed data labels and does not cause a logarithmic fit.
The two axes in `Screenshot 2024-09-15 131522.png` are correctly detected as base-10 logarithmic.
The Y axis in `data.png` is fitted in the normal image-coordinate direction, so increasing data
values run toward decreasing pixel rows.

**Multi-axis verification status.** Independent mapping storage is verified. On the YBCO fixture,
four fits were written using separate runs and persisted together in
`axes.secondary`:

| Axis id | Role | Scale | Fitted limits | Residual | Identity |
| --- | --- | --- | --- | ---: | --- |
| `shared-x` | x | linear, reversed | `81.9696 .. 92.9927` | 0.46 px | shared X mapping |
| `green-pressure` | y | log base 10 | `0.000102422 .. 102.045` | 0.34 px | RGB `(0,150,0)`, Deposition pressure, Torr |
| `red-temperature` | y | linear | `764.991 .. 794.935` | 0.24 px | RGB `(200,0,0)`, Annealing temperature, K |
| `blue-delta-tc` | y | linear | `0.007194 .. 6.0056` | 0.28 px | RGB `(0,0,200)`, Delta Tc, K |

The persistence test confirmed that fitting the red and blue Y axes did not overwrite the green
mapping, and that the shared reversed X mapping remained present. Named mappings use the additive
`axes.secondary` list; the existing default `axes.x` and `axes.y` slots remain unchanged for
single-axis figures. Each named mapping retains its axis id, role, colour, supplied title/unit and
fit metadata. Series association and routing are intentionally deferred to the multi-series
extraction work, where the data series exist.[^multi-axis-scope]

[^multi-axis-scope]: The verification set does not contain a partially resolved multi-axis figure;
      that scenario is intentionally excluded from this ticket's acceptance criteria and may be
      covered by a later multi-series or corpus ticket.
