# 21 — Score an extraction against ground truth

**What to build:** When a figure has hand-authored ground truth beside it, the agent can measure how
close its digitization came instead of judging by eye — and can tell whether a parameter change
helped or hurt.

Ground truth arrived after this work started, which reverses the project's original position that
correctness could only be judged visually. It is the oracle the accuracy metric always needed.

The measurement must compare fairly across figures whose axes span wildly different magnitudes, and
it must survive ground-truth files that a person laid out by hand rather than a program designed.

**Blocked by:** 07 — Export digitized data.

**Status:** done

- [x] Ground truth is found automatically beside the figure, sharing the figure's name, and a
      different file can be pointed at explicitly.
- [x] A figure with no ground truth is reported as such and the run stops cleanly, since most of the
      corpus has none.
- [x] Series in the ground truth are read correctly from column blocks separated by blank columns.
- [x] A block sharing one X column across several Y columns yields one series per Y column, each
      named from its own column.
- [x] Uncertainty columns are recognised and kept out of the series.
- [x] The independent variable is identified by evidence rather than by column position, because in
      these files it is not always first.
- [x] Every judgement the loader made is reported so the agent can overrule it.
- [x] Ground truth whose columns are transposed relative to the chart is detected and corrected, and
      the correction is reported and overridable.
- [x] Extracted series are matched to truth series by best overall pairing, not by label.
- [x] Error is measured in axis units divided by the axis range, so figures compare on equal terms.
- [x] Median, 95th percentile and coverage are reported per series, with the point counts.
- [x] A pass threshold is applied and adjustable, defaulting to a median under 0.5% of the range.
- [x] The score is written into the extraction, kept distinct from confidence.
- [x] The overlay draws ground truth and extracted points together on the figure so the agent can
      see which stage a bad score came from.

**Verified.** All 13 ground-truth files parse, yielding 53 series in total, with the label, point
count and ranges reported for each. Two genuine ambiguities are flagged rather than hidden: one file
whose blocks disagree about which column is the independent variable, and one whose columns are
transposed relative to the chart. A full chain on `Screenshot 2024-09-15 131643.png` scored median
1.47% of range against a 0.5% tolerance — a fail, and a useful one: the overlay showed the
reprojected truth landing squarely on the markers, proving the axis fit was right and the fault lay
in extraction, which is exactly the diagnostic this ticket exists to provide.
