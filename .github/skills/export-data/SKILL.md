---
name: export-data
description: Turn a digitized chart's pixel coordinates into data values and write them out as CSV. Use as the last step of digitizing a figure, or when the extracted points need to be handed to an analysis tool.
---

# Writing the data out

```
uv run .github/skills/export-data/export_data.py --image images/<figure>.png
```

Needs a fitted axis scale and at least one extracted series. Without an axis fit it refuses rather
than emitting numbers that would look plausible and be meaningless.

## What it writes

Everything lands in `export/` inside the working directory:

| File | Contents |
| --- | --- |
| `data.csv` | Every series in one tidy table with a `series` column |
| `<series>-data.csv` | One file per series, for taking a single series without filtering |
| `extraction.json` | The full extraction, including confidences and diagnostics |

Column headings carry the axis titles and units when they were supplied to the axis fit, so the
export is self-describing. Coordinates stay in files; the extraction document holds only counts and
paths.

Projection handles linear and logarithmic axes and reversed directions, matching the axis fit — it
never re-derives the mapping.

## Check the numbers

Spot-check a couple of points against the figure's printed tick labels before trusting an export.
Better, if the figure has a ground-truth CSV beside it, score the export — see the review skill.

`--decimals` controls the digits kept; the default of 6 is well beyond what a pixel measurement
justifies and exists so rounding never becomes the error you are chasing.
