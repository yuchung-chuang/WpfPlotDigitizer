# 08 — Verify an extraction visually

**What to build:** The closing check of the whole workflow: the extracted data is projected back
onto the original image so both the agent and the researcher can see whether it landed on the actual
markers or somewhere else.

This is the only assertion the framework has. There is no automated suite by decision, so this
overlay *is* the test, and it must be good enough to judge by eye.

**Blocked by:** 07 — Export digitized data.

**Status:** done

- [x] A review skill group is discoverable and states that the agent must view the verification
      overlay before declaring the digitization complete.
- [x] Running verification reads the exported data values, projects them back through the axis
      mapping into pixels, and draws them on the original image.
- [x] Each series is drawn distinctly so they can be told apart.
- [x] A summary reports the point count per series and the mean distance between each reprojected
      point and the nearest ink in the plot area.
- [x] A large reprojection error is reported as a diagnostic identifying the likely stage at fault.
- [x] Deliberately corrupting the axis fit produces a visibly wrong overlay and a raised error
      figure, confirming the check actually detects failure.
- [x] The simple scatter plot from the corpus verifies cleanly. **Milestone: one chart digitized end
      to end.**

**Verified.** `131643.png` was exported and reprojected with distinct overlay points. A normal
run reports the point-to-ink distance; changing the X-axis fit to a deliberately shifted mapping
raises the configured error diagnostic and changes the overlay visibly.
