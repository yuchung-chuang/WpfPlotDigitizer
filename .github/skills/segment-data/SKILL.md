---
name: segment-data
description: Separate a chart's data into series and turn each one into coordinates - marker centroids for scatter plots and ordered polylines for curves. Use when extracting the actual data points from a figure being digitized.
---

# Turning ink into coordinates

Run this after locating the plot area, and after the noise filters if the figure needs them.
Extraction reads `masks/plot_mask.png` automatically when it exists and falls back to the raw plot
area when it does not — so filtering first improves the result without changing how you call this.

## Choose the extraction path from the figure

Use `extract_point_series.py` for a **single series of discrete, individually visible data-point
markers**. Ticket 06's full point-oriented fixture set is:

- `images/3-1-freefall.png`
- `images/Screenshot 2021-06-26 230901.png`
- `images/Screenshot 2021-06-26 231058.png`
- `images/A+cleaned+up+scatter+plot.jpg`

For the current ticket 06 validation pass, use only the clean fixtures
`3-1-freefall.png` and `A+cleaned+up+scatter+plot.jpg`. Defer
`Screenshot 2021-06-26 230901.png` until the regression/connecting-line noise is removed and
`Screenshot 2021-06-26 231058.png` until the grid-line noise is removed. Those filters must be
verified before their screenshots are used to judge point-extraction quality.

Inspect the chart image first and choose the operation that matches what is drawn. Use
`extract_line_series.py` for continuous line series, `segment_series.py` followed by per-series
extraction for multiple series, and the relevant filtering or decline workflow for bars,
histograms, multi-panel figures, or other unsupported layouts. Do not use the ticket 06 point
fixtures as a blanket test set for every image in `images/`; a different chart type needs its own
operation and acceptance check.

## Finding the legend

Before extracting a multi-series figure, locate its boxed legend and preserve its entries:

```
uv run .github/skills/segment-data/find_legend.py --image images/<figure>.png
```

After reading the label crops, record the names explicitly, for example
`--label 0=CF --label 1=CF300_30 --label 2=CF600_180`.

The script searches the full figure, so legends inside the plot area and in the margins are both
eligible. It records the box and one swatch/label pair per entry, writes upscaled crops under
`crops/`, and protects the legend from noise filters. A missing legend is an ordinary low-confidence
result; unsupervised segmentation does not depend on it.

## Segmenting multiple series

Once the plot area, legend, and styles are ready, write one mask per discovered series:

```
uv run .github/skills/segment-data/segment_series.py --image images/<figure>.png
```

Clustering uses CIELAB pixels in the plot area, excludes protected regions and noise layers, and
matches discovered colours to profiled legend entries. Use `--clusters N` when the legend is absent
or its count is not the expected number. Record a human correction with `--merge A,B` or
`--split A`; the operation is retained in the stage history and lowers confidence when the result
does not agree with the legend count.

## Extracting marker positions

```
uv run .github/skills/segment-data/extract_point_series.py --image images/<figure>.png
```

**Look at the overlay.** A cross should sit on every marker. Crosses in empty space or markers with
no cross both mean the parameters need moving.

How it works, and therefore what to tune:

1. **Opening erases connecting lines while markers survive** — a line is thinner than a marker, so a
   morphological opening removes one and keeps the other. `--radius` is the disk radius.
   Raise it when line segments are becoming points; lower it when small markers are vanishing.
2. **Contour centroids give sub-pixel positions**, with a fallback for single-pixel blobs whose
   moments are degenerate.
3. **Area outliers are discarded** — a blob far from the median marker area is a merge or a
   fragment, not a data point. `--z-threshold` is the threshold. Raise it to keep more, lower it to
   be stricter. The summary reports how many were discarded.
4. **Oversized components are split with distance-transform watershed** — the most common blob
   area estimates one marker, then local distance peaks divide touching markers before the area
   check. The raw oversized-component warning remains, because a saturated cloud is still an
   undercount risk.

## Reading the result

| Line | Meaning |
| --- | --- |
| `points` | Kept, and discarded as area outliers |
| `area` | Median marker area and the kept range — a sanity check on `--z-threshold` |
| `opening` | The radius used and how much ink it erased |
| `ink source` | Whether a noise mask was used or the raw plot area |

Coordinates go to `points/<series>-pixel.csv`, and the extraction holds only the count and the path.
They are still **pixels** — projecting them into data units is the export step's job.

## Dense and overlapping scatter

When markers overlap heavily the count can still be an **undercount**, because merged markers may
have no separable distance peaks. The script raises a warning when raw blobs remain far above the
modal marker area, and that is the signal to check the overlay rather than trust the number.

If large parts of a dense cloud come out empty:

- Lower `--radius` to 0 or 1, since opening thins an already-merged mass further.
- Raise `--z-threshold` substantially, because in a merged cloud the *merges* dominate and the
  z-score rejects the real data.
- Accept that a saturated region cannot be recovered marker by marker, and say so in the extraction
  rather than reporting a confident count.
