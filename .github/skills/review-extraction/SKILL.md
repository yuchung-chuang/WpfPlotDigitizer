---
name: review-extraction
description: Check a digitized chart against hand-authored ground truth and score how close it came. Use after exporting data, when verifying a digitization, or when tuning extraction parameters and needing to know whether a change helped.
---

# Checking the work

Several figures in `images/` have a hand-authored ground-truth CSV beside them with the same
name. When one exists, **score against it** — it is the only objective answer to "is this right?"

```
uv run .github/skills/review-extraction/score_extraction.py --image images/<figure>.png
```

Run it after `export_data.py`, because scoring compares data units, not pixels.

## What the score means

Errors are measured in **axis units normalised by the axis range**, so a figure spanning millions
and one spanning 0 to 1 are judged on the same scale. For each ground-truth point the nearest
extracted point is found, and that distance is the error.

| Column | Meaning |
| --- | --- |
| `median` | Half the truth points are closer than this. The headline number |
| `p95` | The tail. A good median with a bad p95 means a few points went badly wrong |
| `covered` | Fraction of truth points with an extracted point inside tolerance |
| counts | Truth points and extracted points — a large gap means points were missed or invented |

**Passing is a median under 0.5% of the axis range**, adjustable with `--tolerance`. Series are
matched to truth series by whichever overall pairing fits best, so a mislabelled series still scores.

## Reading a failure

The overlay draws ground truth large and extracted points small, both on the original figure. Look
at it before changing anything, because the number alone does not say *which* stage was wrong:

- **Truth crosses land on the markers but the score is poor** — the axis fit is right and extraction
  is at fault. Tune the point-extraction parameters.
- **Truth crosses sit off the markers uniformly** — the axis fit is wrong. Re-read the tick labels;
  a misread digit does exactly this.
- **Truth crosses are stretched or mirrored** — an axis is reversed or the log scale was missed.
- **Counts differ wildly** — a multi-series figure was extracted as one series. Segment it first.

## Ground truth is hand-made, so check the parse

These files are spreadsheets a person laid out, not a designed format: series sit in column blocks
separated by blank columns, one block may share an X column across several Y columns, uncertainty
columns are mixed in, and the X column is not always first. The loader works this out by evidence
and **reports what it decided** — read those notes:

- *"blocks disagree about which column is X"* — check the axes against the figure.
- *"one X shared by N Y columns"* — expected for a table of several series against a common X.
- *"ground truth columns are transposed relative to the chart"* — the file lists the chart's Y axis
  first. It is swapped automatically; pass `--swap-truth-axes no` if that was wrong.

Point at a differently-named file with `--truth <path>`.

## When there is no ground truth

Most figures have none, and the script says so and stops. Fall back to the overlay: look at whether
the extracted points sit on the markers. That is a weaker check but it catches gross failures.
