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

To perform the visual closing check after export, use:

```
uv run .github/skills/review-extraction/verify_extraction.py --image images/<figure>.png
```

Verification reads each `points_data` sidecar, projects it through the recorded axis fit, measures
the mean distance to visible ink inside the plot area, and writes a distinct-colour overlay. A
large distance is reported as an axis-fit or extraction diagnostic; view the overlay before
declaring the extraction complete.

To run the reproducible baseline over every figure with checked-in ground truth, use:

```
uv run .github/skills/review-extraction/score_corpus.py
```

The command replays the axis readings in `corpus-axis-readings.json`, creates a fresh generated
workspace for each figure, and writes the commit-friendly report to
`.scratch/plot-digitizer-agent/corpus-scoreboard.md`. It sorts figures by filename, records the
source hash of every pipeline skill, and reports a crashed figure as a failed row so one bad image
does not hide the rest of the corpus.

**Fit the axis scale with `--x-title` and `--y-title` first.** The truth file's columns are matched
to the chart's axes **by header**, so the titles you read off the figure are what tells the scorer
which column is X. There is no rule that the first column is X — several of these files put Y
first. Without titles the scorer falls back to guessing from value ranges and says so.

## What the score means

Two things are measured, and they fail differently.

**Localisation** — how far the extracted points sit from the truth. Errors are in **axis units
normalised by the axis range**, so a figure spanning millions and one spanning 0 to 1 are judged on
the same scale. For each ground-truth point the nearest extracted point is found, and that distance
is the error.

Scoring uses each series' exported `points_data` sidecar, which contains the extracted centroids or
line samples. The optional `support_points_data` sidecar contains cleaned mask pixels for visual
support and is not the extracted series; it must not be used to judge point counts or localisation.

**Segmentation** — whether the series were separated correctly. Reported as how many series were
found against how many the truth holds, plus **purity**: the fraction of an extracted series' points
whose nearest truth point actually belongs to the series it was matched to. A series can be
perfectly located and still be wrong if it is carrying another series' points, and purity is what
catches that.

| Column | Meaning |
| --- | --- |
| `median` | Half the truth points are closer than this. The headline number |
| `p95` | The tail. A good median with a bad p95 means a few points went badly wrong |
| `covered` | Fraction of truth points with an extracted point inside tolerance |
| `pure` | Fraction of this series' points that belong to it. Low means series were merged |
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

These files are spreadsheets a person laid out, not a designed format. Two layouts appear:
**blocks** of columns separated by blank columns, and **long format** where a text column names the
series and two numeric columns hold the values. Within a block one X column may be shared by several
Y columns, uncertainty columns are mixed in, and the X column is not always first.

The loader works this out and **reports every judgement it made** — read those notes:

- *"long format: N series named by the 'Group' column"* — expected for a tidy file.
- *"one X shared by N Y columns"* — expected for a table of several series against a common X.
- *"blocks disagree about which column is X"* — you did not supply axis titles, so it guessed from
   monotonicity and the guess was inconsistent. Fit the axis scale with titles and re-run.
- *"'...' is named in the file but carries no data points"* — a series the author described some
   other way, such as a fitted line given only as its equation. It cannot be scored.
- *"N series share the label"* — two blocks named the same thing.

Point at a differently-named file with `--truth <path>`, and override the column matching with
`--swap-truth-axes yes|no` if the headers are too different from the axis titles to match.

## When there is no ground truth

Most figures have none, and the script says so and stops. Fall back to the overlay: look at whether
the extracted points sit on the markers. That is a weaker check but it catches gross failures.
