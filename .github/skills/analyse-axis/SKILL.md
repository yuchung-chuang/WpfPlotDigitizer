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

## When it goes wrong

- **The box hugs the data instead of the frame** — the frame is faint or absent. Check whether the
  line-frame candidate in `plot_area.candidates` is better and override with it.
- **The box covers the whole figure** — the figure's own outer border was taken for the frame.
  Re-run the chart overview so a panel is recorded, or override.
- **The box includes the legend or the colourbar** — override with the plot area alone.
- **Nothing found on a photographed or heavily compressed chart** — expected. Override.

Both candidate boxes are always recorded under `plot_area.candidates`, so you can switch to the
rejected one without re-running anything.
