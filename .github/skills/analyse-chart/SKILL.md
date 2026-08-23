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
and drops nested rectangles like legend boxes and insets. More than one panel is an automatic
decline.

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
  --declare unsupported --reason "three offset Y axes and two X axes"
```

Until you declare, `supported` stays null and confidence stays at 0.5. That is deliberate: an
undeclared figure is one nobody has looked at.

## In scope

A single panel, one X and one Y axis, linear or logarithmic, carrying any number of scatter and line
series. Error bars are handled by removing them.

Decline anything else — several panels, offset secondary axes, a bar chart or histogram, a pie or
polar plot. **Declining is a success.** The failure this framework exists to prevent is confident
wrong numbers, so refusing a figure beats digitizing the wrong thing in it.

## Reading the result

| Line | Meaning |
| --- | --- |
| `panels` | Frames found. More than one is an automatic decline |
| `axis lines` | Tick-bearing lines, and long lines, around the primary panel |
| `supported` | `yes`, `no`, or `unknown` until you declare |
| `type` | What you saw in the preview, once you pass `--type` |

If the overlay's panel box is wrong, `--min-panel-area` changes the smallest fraction of the figure
a frame must cover to count. Raise it when legend boxes are being counted as panels; lower it when a
small panel is being missed.
