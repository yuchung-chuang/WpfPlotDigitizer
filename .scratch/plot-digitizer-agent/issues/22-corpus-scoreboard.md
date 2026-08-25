# 22 — Corpus scoreboard

**What to build:** One command that digitizes every ground-truthed figure and prints a table of
scores, so a change to any skill can be judged against the whole corpus rather than the one figure
it was tuned on.

This is the regression net the project deliberately went without while there was no oracle. Now
there is one, and a silent regression in a shared primitive — the ink threshold, the plot-area
search, the projection — would otherwise only surface by accident.

It must not need an agent in the loop, so the axis readings each figure needs are recorded once and
replayed. Reading tick labels is the only step that genuinely requires eyes, and re-reading them on
every run would make the scoreboard unusable.

**Blocked by:** 21 — Score an extraction against ground truth.

**Status:** in progress — the latest corpus scoreboard fails 14 figures

**Latest verification failure:** `score_corpus.py` completed with exit code 1 and reported 29 of
69 series passed across 14 failed figures. Failures include missing axis readings for the older
2021/clean fixtures and accuracy or coverage failures in multiple 2024 multi-series fixtures. The
report was written to `.digitize/verification-scoreboard-20260824/corpus-scoreboard.md`; no
implementation changes were made.

**Verification fixture order.** The scoreboard should be checked from the smallest scoreable case to
the full truth-bearing corpus, while the generated report still includes every exact image/CSV pair.

1. `images/3-1-freefall.png` with `images/3-1-freefall.csv`, then
      `images/A+cleaned+up+scatter+plot.jpg` with `images/A+cleaned+up+scatter+plot.CSV` - minimal
      point-extraction and axis-fit regressions.
2. `images/Screenshot 2021-06-26 230901.png` and `images/Screenshot 2021-06-26 231058.png` with
      their same-stem CSV files - the first requires annotation/series handling and the second grid
      filtering, so they follow the foundational rows.
3. The 13 2024 pairs, in filename order: `images/Screenshot 2024-09-15 131234.png`,
      `images/Screenshot 2024-09-15 131309.png`, `images/Screenshot 2024-09-15 131341.png`,
      `images/Screenshot 2024-09-15 131401.png`, `images/Screenshot 2024-09-15 131423.png`,
      `images/Screenshot 2024-09-15 131439.png`, `images/Screenshot 2024-09-15 131522.png`,
      `images/Screenshot 2024-09-15 131556.png`, `images/Screenshot 2024-09-15 131624.png`,
      `images/Screenshot 2024-09-15 131643.png`, `images/Screenshot 2024-09-15 131712.png`,
      `images/Screenshot 2024-09-15 131740.png` and `images/Screenshot 2024-09-15 131805.png`.
      Run these only after tickets 14, 17, 18 and 19 cover the features present in each figure.

The alternate `images/3-1-freefall-minimum.csv` is intentionally excluded from the automatic
scoreboard until an image with the exact same stem exists. Non-truth images, including
`images/3-2-freefall-mimimum.png`, remain corpus-pass fixtures but do not produce scoreboard rows.

- [x] Axis readings per figure are stored in a checked-in file, so a scoreboard run needs no agent.
- [x] One command runs every figure that has ground truth and prints one row per figure: series
      count, median error, 95th percentile, coverage, and pass or fail.
- [x] A total line reports how many series passed out of how many.
- [x] Results are written to a file that can be committed, so a `git diff` shows which numbers moved.
- [x] A figure that crashes is reported as a failure row rather than stopping the run.
- [x] The scoreboard records which skill versions produced it, so a change can be attributed.
- [x] Running it twice without changing anything produces the same numbers.

**Verified.** `score_corpus.py` runs all 13 truth-bearing figures, writes
`corpus-scoreboard.md`, records SHA-256 prefixes for the baseline skill scripts, and isolates the
`131556` scorer crash as one failed row. The baseline currently passes 0 of 69 parsed series; the
report is deterministic across repeated runs.
