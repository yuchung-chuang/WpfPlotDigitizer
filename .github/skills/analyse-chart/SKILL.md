---
name: analyse-chart
description: Take the first look at a chart image before digitizing it - how many panels it holds, what kind of plot it is, and whether it is in scope. Use at the start of digitizing a figure, or when deciding whether a figure can be digitized at all.
---

# Taking the first look

Run this before anything else on a new figure. It measures what it can and hands you the rest,
because deciding whether a figure is in scope needs eyes.

```
uv run .github/skills/analyse-chart/inspect_chart.py --image images/<figure>.png
```

Then **read the preview it writes** and **look at the overlay**. The preview is downscaled so it
costs little context; the overlay shows the panel boxes it found drawn on the figure.

## What it measures, and what you decide

**Panel count is reliable** — it finds each panel's frame, ignores the figure's own outer border,
and drops nested rectangles like legend boxes and insets. Multiple panels are reported as separate
panel contexts; each one needs its own plot-area and axis analysis before extraction.

**Axis lines are evidence, not a verdict.** It reports long lines around the primary panel and which
of them carry tick marks. Tick detection misses thin and inward-drawn ticks, so treat a low count as
"not proven" rather than "not there".

**The support verdict is yours.** On a single-panel figure the script leaves `supported` unset and
asks you to look. Decide from the preview whether the figure has one X and one Y axis and nothing
exotic, then record it:

```
uv run .github/skills/analyse-chart/inspect_chart.py --image images/<figure>.png \
  --type scatter --declare supported
```

```
uv run .github/skills/analyse-chart/inspect_chart.py --image images/<figure>.png \
  --declare supported --unresolved-axis right-y \
  --reason "right-y series association remains unresolved"
```

Until you declare, `supported` stays null and confidence stays at 0.5. That is deliberate: an
undeclared figure is one nobody has looked at. For a multi-panel figure, declaring support means
the figure is suitable for per-panel processing; downstream stages must not reuse one panel's axes
for another panel.

## In scope

A single panel or independently resolvable panel context, with one or more linear or logarithmic
axis mappings. Multiple horizontal or vertical axes are in scope when their colours or other
stable visual styles provide a best-effort association between each data series and the correct
axis. Use colour, line or marker style, labels and spatial alignment as evidence, record the
association and confidence, and carry it through plot-area detection, scale fitting, extraction and
export. If one association remains unresolved, record that axis with `--unresolved-axis` and
continue with the resolved series; do not discard the whole figure. Error bars are handled by
removing them.

Decline only the unresolved axis or associated series. Decline the whole figure for a bar chart or
histogram, a pie or polar plot, or when the main axes cannot be established. A multi-panel figure is
only declined when its panels cannot be independently resolved. The failure this framework exists
to prevent is confident wrong numbers, so omit an ambiguous subset rather than digitizing it with
the wrong mapping.

## Reading the result

| Line | Meaning |
| --- | --- |
| `panels` | Frames found; each panel gets an independent processing context |
| `axis lines` | Tick-bearing lines, and long lines, around the primary panel |
| `supported` | `yes`, `no`, or `unknown` until you declare |
| `type` | What you saw in the preview, once you pass `--type` |

If the overlay's panel box is wrong, `--min-panel-area` changes the smallest fraction of the figure
a frame must cover to count. Raise it when legend boxes are being counted as panels; lower it when a
small panel is being missed.
