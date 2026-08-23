---
name: segment-data
description: Separate a chart's data into series and turn each one into coordinates - marker centroids for scatter plots and ordered polylines for curves. Use when extracting the actual data points from a figure being digitized.
---

# Turning ink into coordinates

Run this after locating the plot area, and after the noise filters if the figure needs them.
Extraction reads `masks/plot_mask.png` automatically when it exists and falls back to the raw plot
area when it does not — so filtering first improves the result without changing how you call this.

## Extracting marker positions

```
uv run .github/skills/segment-data/extract_point_series.py --image images/<figure>.png
```

**Look at the overlay.** A cross should sit on every marker. Crosses in empty space or markers with
no cross both mean the parameters need moving.

How it works, and therefore what to tune:

1. **Opening erases connecting lines while markers survive** — a line is thinner than a marker, so a
   morphological opening removes one and keeps the other. `--opening-radius` is the disk radius.
   Raise it when line segments are becoming points; lower it when small markers are vanishing.
2. **Contour centroids give sub-pixel positions**, with a fallback for single-pixel blobs whose
   moments are degenerate.
3. **Area outliers are discarded** — a blob far from the median marker area is a merge or a
   fragment, not a data point. `--outlier-z` is the threshold. Raise it to keep more, lower it to be
   stricter. The summary reports how many were discarded.

## Reading the result

| Line | Meaning |
| --- | --- |
| `points` | Kept, and discarded as area outliers |
| `area` | Median marker area and the kept range — a sanity check on `--outlier-z` |
| `opening` | The radius used and how much ink it erased |
| `ink source` | Whether a noise mask was used or the raw plot area |

Coordinates go to `points/<series>-pixel.csv`, and the extraction holds only the count and the path.
They are still **pixels** — projecting them into data units is the export step's job.

## Dense and overlapping scatter

When markers overlap heavily the count is an **undercount**, because merged markers form one blob.
The script raises a warning when it sees blobs far above the median area, and that warning is the
signal to check the overlay rather than trust the number.

If large parts of a dense cloud come out empty:

- Lower `--opening-radius` to 0 or 1, since opening thins an already-merged mass further.
- Raise `--outlier-z` substantially, because in a merged cloud the *merges* dominate and the
  z-score rejects the real data.
- Accept that a saturated region cannot be recovered marker by marker, and say so in the extraction
  rather than reporting a confident count.
