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

**Status:** ready-for-agent

- [ ] Axis readings per figure are stored in a checked-in file, so a scoreboard run needs no agent.
- [ ] One command runs every figure that has ground truth and prints one row per figure: series
      count, median error, 95th percentile, coverage, and pass or fail.
- [ ] A total line reports how many series passed out of how many.
- [ ] Results are written to a file that can be committed, so a `git diff` shows which numbers moved.
- [ ] A figure that crashes is reported as a failure row rather than stopping the run.
- [ ] The scoreboard records which skill versions produced it, so a change can be attributed.
- [ ] Running it twice without changing anything produces the same numbers.
