# 20 — Corpus pass over all images

**What to build:** The verdict. Every image in the repository's corpus is run through the framework
and the outcome recorded, so the researcher can judge whether the approach is worth continuing.

Two outcomes count as success, and conflating them would hide the thing that matters most:
a supported figure that digitizes correctly, and an unsupported figure that is **declined with a
reason**. A figure that produces confident wrong numbers is the only real failure, because that is
precisely the flaw this project exists to fix.

**Blocked by:** 08 — Verify an extraction visually; 14 — Remove error bars; 18 — Extract multiple
point series; 19 — Extract line series.

**Status:** ready-for-agent

- [ ] Every image in the corpus is processed and its outcome recorded.
- [ ] Each record states the outcome, the series found, the confidence, and any diagnostics raised.
- [ ] Verification overlays are kept for every image so the results can be reviewed by eye.
- [ ] Figures that were declined are listed separately from figures that failed, with the reason for
      each decline.
- [ ] The multi-panel composite and the multiple-offset-axis figure are confirmed as declined rather
      than mangled.
- [ ] No figure produces high-confidence output that the overlay shows to be wrong.
- [ ] Recurring failure patterns are written up as candidate follow-up work rather than fixed
      ad hoc inside this ticket.
- [ ] The findings note which parts of the deferred work — the review interface, the synthetic
      corpus and its accuracy metric — the results now justify building.
