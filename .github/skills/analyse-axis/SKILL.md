---
name: analyse-axis
description: Find a chart's plot area in pixels and turn pixel positions into data values. Use when locating the axes of a figure being digitized, reading its tick labels, or fitting the mapping from pixels to data units.
---

# Analysing the axes

Everything downstream is measured against the plot area, so get this right before filtering noise or
extracting anything. Run the chart overview first — this skill uses the panel it found.

## Locating the plot area

```
uv run .github/skills/analyse-axis/locate_plot_area.py --image images/<figure>.png
```

**Look at the overlay.** A plot area that is slightly off puts every extracted point slightly off,
and the error is invisible in the numbers.

### Agent visual review is required

The scripts produce candidates; they do not decide whether a candidate is the chart's data panel.
Before accepting any result, inspect the original image and the locator overlay as an agent:

1. Identify the data panel, its visible spines or background fill, and the extent of the X and Y
  axes. The plot area contains the data and grid lines, but not the title, axis labels, caption,
  legend outside the panel, or colorbar.
2. Compare each candidate with those visual cues. A high numeric confidence is not evidence that
  the candidate is semantically the right panel.
3. Reject a candidate when it encloses a title or caption, hugs a colorbar, follows a legend rule,
  or stops at the data cloud instead of the panel boundary. Treat repeated equally spaced lines as
  grid lines unless the panel extent shows that they are its outer edges.
4. If the candidate is wrong or missing, determine the rectangle visually using the procedure below
  and record it with `--box`. Inspect the resulting override overlay again.

Use this short review record when assessing a fixture: `visible boundaries`, `excluded objects`,
`chosen rectangle`, and `why the candidate was accepted or overridden`. This makes a silent false
frame distinguishable from a deliberate visual judgment.

Two strategies run independently and are compared, because a single heuristic that is confidently
wrong is the worst outcome:

- **Corner probe** walks in from each corner of the largest ink region looking for a long run of ink
  heading along each edge, accepting a run that is mostly ink so a curve or a tick label crossing
  the frame does not break it. It copes with an incomplete frame.
- **Line frame** takes the outermost long, thin, axis-aligned lines and intersects them. Stronger
  when the frame is clean, and it finds a panel that has no drawn frame at all — a shaded ggplot
  panel, for instance.

| `method` | What it means |
| --- | --- |
| `agreement` | Both strategies landed within tolerance. Trust it |
| `corner-probe` / `line-frame` | Only one answered, or they disagreed and this one was preferred |
| `override` | You supplied the box |

Confidence follows: agreement scores high, a lone strategy is middling, and a disagreement scores
low and records a warning naming both boxes. **A disagreement is a signal to look, not a failure** —
it usually means the figure has a second frame, a boxed legend, or more than one panel.

When neither strategy finds anything, the script reports an error and no box rather than guessing.

## Overriding

```
uv run .github/skills/analyse-axis/locate_plot_area.py --image images/<figure>.png --box 85,18,749,548
```

Read the corner pixel coordinates off the overlay or the preview. An override is recorded as the
method, so the extraction says a human-or-agent judgement was used.

### Visually bounding an open plot

When the plot has only some of its spines, determine the boundary from the visible axes and the
primary panel, not from the data ink. If chart overview says that no panel frame was found and uses
the whole figure as a fallback panel, find the inner panel from its spines, stable background fill,
or grid extent before applying these steps; the whole image is not automatically the plot area:

1. Identify each visible axis spine in the preview or original image and read its pixel coordinate.
  Use the spine for every boundary that is actually drawn.
2. For a missing boundary, use the corresponding edge of the primary panel reported by
  `inspect_chart.py` when that panel is visually valid. Otherwise use the inner edge of the stable
  plot background or the outermost grid line that bounds the axes. Do not infer a top, right, or
  other missing edge from the outermost data points, tick labels, or whitespace.
3. Check that the resulting rectangle contains the data region and excludes the surrounding labels,
  title, caption, legend outside the panel, colorbar, and margins. A legend drawn inside the data
  panel does not define a new boundary and remains inside the rectangle.
4. Supply the rectangle with `--box X,Y,WIDTH,HEIGHT`. The extraction records `override`; this
  means the boundary was judged visually, not that automatic detection achieved high certainty.

For example, if an open plot has a left spine at `x=103` and a bottom spine at `y=699`, while its
primary panel spans `x=95..896` and `y=119..707`, use the panel's top edge and the left spine for
the left/top corner, and the panel's right edge and the bottom spine for the right/bottom corner:

```
uv run .github/skills/analyse-axis/locate_plot_area.py \
  --image images/<figure>.png --box 103,119,793,580
```

The overlay must still be inspected after the override. If either visible spine is ambiguous, keep
the automatic failure diagnostic and ask for a supplied box rather than silently guessing.

### Shaded panels, grids, and companion scales

For a shaded plotting theme, the panel is the contiguous rectangle of plotting background behind the
data. Use its color transition as the boundary even when no dark frame is drawn. Keep the regular
grid lines inside that rectangle; they are evidence of the panel extent, not separate plot areas.

Apply these checks to common look-alikes:

- A colorbar is a narrow standalone scale with its own gradient and tick labels. Exclude it even if
  its long edges satisfy the line-frame detector.
- An outer figure border surrounds titles, labels, and captions. Exclude it; use the inner axis or
  panel extent instead.
- A legend is explanatory furniture. Exclude it when it is outside the panel, but do not shrink the
  plot area around a legend placed inside the panel.
- Repeated grid lines are not independent frames. Prefer the contiguous shaded-panel edge or the
  axis endpoints that contain the data and their grid.

If the panel edge is visually clear but the automatic candidate follows one of these look-alikes,
override it and note which object was rejected. If the panel edge itself is unclear, report the
ambiguity and retain a low-confidence or missing result rather than guessing from the data extent.

## When it goes wrong

- **The box hugs the data instead of the frame** — the frame is faint or absent. Check whether the
  line-frame candidate in `plot_area.candidates` is better and override with it.
- **The box covers the whole figure** — the figure's own outer border was taken for the frame.
  Re-run the chart overview so a panel is recorded, or override.
- **The box includes the legend or the colourbar** — override with the plot area alone.
- **Nothing found on a photographed or heavily compressed chart** — expected. Override.

Both candidate boxes are always recorded under `plot_area.candidates`, so you can switch to the
rejected one without re-running anything.

## Reading the scale

Two steps, because **you** read the numbers — there is no OCR here, and your eyes are better than
one.

**First, crop the labels:**

```
uv run .github/skills/analyse-axis/read_axis_scale.py --image images/<figure>.png
```

This finds the tick marks on each edge, finds the text beside each one, and writes a single composite
image per axis into `crops/` pairing every pixel position with its label — vertical-axis labels
rotated so they read horizontally. **Open those crops and read them.**

**Then hand back what you read:**

```
uv run .github/skills/analyse-axis/read_axis_scale.py --image images/<figure>.png \
  --x "86=-4,833=6" --y "19=7,566=-4" --x-title "time" --y-title "voltage" --y-unit "V"
```

Each pair is `pixel=value`. Two per axis is enough; give more and they are fitted by least squares.
The fit extrapolates to the plot-area edges, so the stored minimum and maximum are the axis limits,
not the outermost ticks.

**Log axes are detected, not assumed.** Both a linear and a log fit are tried and the log one wins
only when it is clearly better. Force it with `--x-scale log --x-log-base 10` when a figure has too
few ticks to tell.

**Reversed axes fall out of the fit** and are recorded with a `reversed` flag, so an axis whose
values decrease left to right comes out right instead of mirrored.

**Watch the residual.** It is reported per axis in pixels and in data units. A residual of roughly
zero means your reading was consistent; a large one almost always means a misread digit — a `5` read
as `6`, or a minus sign missed — and raises a warning. Re-read the crop rather than accepting it.

