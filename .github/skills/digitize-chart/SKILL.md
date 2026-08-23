---
name: digitize-chart
description: Turn a picture of a 2-D chart back into numbers. Use when digitizing or extracting data from a plot image, reading values off a figure, recovering a dataset from a published graph, or working with anything under a .digitize working directory.
---

# Digitizing a chart

You are the orchestrator. The scripts do what you cannot — locate an axis to the pixel, cluster
colours, template-match markers, trace a line into an ordered polyline. You do what they cannot —
look at the figure, read the legend and the tick labels, judge whether a result is right, and decide
what to do next.

**Choose your own order.** Nothing here is a pipeline. Skip what a figure does not need, repeat what
comes out wrong, and go back a step whenever the overlay tells you to.

Read [SCHEMA.md](SCHEMA.md) before writing to the extraction document or when you need to know what
a field means.

## Three rules

**Look at every overlay.** Each script writes an annotated copy of the figure and prints its path.
View it before you run the next thing. It is the only check this framework has: numbers alone cannot
tell you the axis box latched onto a colourbar instead of the plot, but one glance can.

**Leave bulk data in its sidecar.** Coordinates live in files under the working directory. Read a
count, read a path, and move on. A figure can hold thousands of points; pulling them into your
context burns the budget and you will silently round or truncate them.

**Decline rather than guess.** A figure beyond the supported scope — several panels, offset
secondary axes, a bar chart — gets reported as declined, with the reason. Confident wrong numbers
are the failure this framework exists to prevent. Producing none is a success; producing bad ones is
not.

**Score against ground truth whenever it exists.** Some figures in `images/` have a hand-authored
CSV of the real numbers beside them, with the same name. When one is there, finish by scoring the
export against it — a measured error beats an opinion about an overlay, and it is the only way to
tell whether a parameter change helped or hurt. Check before you start:

```
Test-Path ("images/<figure>.csv")
```

## Running a script

Every script takes `--image` and an optional `--workdir`, and is run with `uv`, which fetches its own
dependencies on first use:

```
uv run .github/skills/<group>/<script>.py --image images/<figure>.png
```

Check the toolkit works before anything else on a new machine:

```
uv run .github/skills/digitize-chart/check_setup.py --image images/data.png
```

If `uv` is missing, install it from https://docs.astral.sh/uv/ and re-run. The first run takes a
minute while dependencies download; later runs start immediately.

## The working directory

One figure, one folder — `.digitize/<figure-name>/` unless you pass `--workdir`:

| Path | What it holds |
| --- | --- |
| `extraction.json` | The extraction — every script reads it and adds to it |
| `overlays/` | One annotated figure per operation. **Look at these** |
| `masks/` | One file per mask layer, plus the combined `plot_mask.png` |
| `points/` | Coordinate sidecars, one per series |
| `crops/` | Regions cut out for you to read, such as tick labels and legend entries |

State lives on disk, so a failed run leaves everything the last good step produced and you can re-run
one step without repeating the others. To start clean, delete the folder.

## Typical shape

A rough order, not a required one:

1. **Understand the figure** — what kind of chart, how many panels, is it in scope.
2. **Analyse the axes** — find the plot area, read the tick labels, fit pixels to data values.
3. **Filter the noise** — border, grid lines, background, annotations, error bars. Each contributes
   a mask layer; drop or re-run any one of them alone.
4. **Segment the data** — find the legend, profile each series style, split the plot into one mask
   per series.
5. **Extract** — markers into points, curves into ordered polylines.
6. **Export** — project into data units and write the table.
7. **Review** — reproject over the figure and check by eye, and score against ground truth when the
   figure has any.

Each group is its own discoverable skill; read the one whose job matches what you need next.

## Tuning against a score

When a figure has ground truth, you have a real feedback loop rather than an impression. Change one
parameter, re-run extraction and export, re-score, and keep the change only if the median error
fell. Work on a ground-truthed figure first, settle the parameters there, then carry them to figures
that have none.

## When a script reports low confidence

Confidence is the script telling you how much it trusts itself. Low confidence plus a good-looking
overlay means proceed. Low confidence plus a bad overlay means act:

- Re-run with different parameters — most scripts expose the threshold that matters.
- Override the result. Plot-area detection and series style both accept values you supply.
- Try the operation on a cropped region instead of the whole figure.
- Record what you saw as a diagnostic and carry on with the reduced confidence, so the final
  extraction says how much to trust it.
