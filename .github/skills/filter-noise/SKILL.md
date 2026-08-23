---
name: filter-noise
description: Remove everything in a chart that is not data - the plot border and tick marks, grid lines, background fills, in-plot annotations, and error bars. Use before extracting series from a figure, or when extracted points include ink that is not a data point.
---

# Filtering out what is not data

Every filter here claims pixels into its **own named mask layer**. Nothing modifies a shared image.
That is what lets you drop one filter and keep the rest, which you will want constantly, because the
most likely failure in digitizing is an over-eager filter eating a thin data series.

Run these after locating the plot area and before extracting anything.

## The contract every filter follows

**One layer per filter.** A filter writes `masks/<name>.png` and records it in the extraction under
`mask_layers` with its name, `kind: "noise"`, file and pixel count. Re-running replaces that layer
and nothing else.

**The combined mask is derived.** `masks/plot_mask.png` is the union of every noise layer, rebuilt
whenever a layer is added or dropped. Never edit it directly. Extraction scripts read it
automatically and fall back to the raw plot area when it does not exist.

**Filters are idempotent.** Every layer is computed from the *original* figure, never from an
already-masked one, so running a filter twice changes nothing and running them in a different order
gives the same combined mask.

**Dropping is first-class.** Every filter takes `--drop`, which removes its own layer and rebuilds
the combined mask:

```
uv run .github/skills/filter-noise/clear_border.py --image images/<figure>.png --drop
```

**Protected regions are untouchable.** Anything recorded in `protected_regions` — the legend, most
importantly — is spared by every filter, and the summary reports how many pixels were spared.

**Each layer gets its own overlay colour**, keyed by its position in `mask_layers`, so a layer keeps
its colour between runs and you can see at a glance which filter claimed which pixels.

## The four categories

They must not overlap, or two filters will fight over the same pixels. The boundaries are fixed in
`CONTEXT.md`:

| Filter | Claims | Does not claim |
| --- | --- | --- |
| `clear_border` | The plot frame and its tick marks — ink forming the plot-area boundary | Data points that merely sit near the edge |
| `remove_grid_lines` | Thin lines at regular spacing aligned to the ticks | A thin data series, which is irregular |
| `remove_background` | Area: solid fills, gradients, shaded and alternating bands | Ink of any kind, however large |
| `remove_annotations` | Text and arrows overlaid on the chart | The legend, and dense clusters of small markers |
| `remove_error_bars` | Whiskers and their caps | The marker at the whisker's centre |

## Removing the border

```
uv run .github/skills/filter-noise/clear_border.py --image images/<figure>.png
```

Takes the frame and the tick stubs attached to it, by finding ink connected to the plot-area
boundary — the flood-fill idea from the desktop implementation. `--reach` sets how far from the
boundary still counts as touching; raise it for a thick or doubled frame, lower it when data close
to the axes is being eaten.

**An axis drawn through the origin is not border.** It does not bound the plot area, so
`clear_border` leaves it alone. Treat it as an annotation.

## After running filters

**Look at the overlay** and check that the tinted pixels are all things you wanted gone. Then re-run
extraction — it picks up the new combined mask automatically. If a series got thinner, drop the
layer you suspect and compare.
