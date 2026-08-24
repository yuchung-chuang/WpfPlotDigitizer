# 20 — Corpus pass over all images

**What to build:** The verdict. Every image in the repository's corpus is run through the framework
and the outcome recorded, so the researcher can judge whether the approach is worth continuing.

Two outcomes count as success, and conflating them would hide the thing that matters most:
a supported figure that digitizes correctly, and an unsupported figure that is **declined with a
reason**. A figure that produces confident wrong numbers is the only real failure, because that is
precisely the flaw this project exists to fix.

**Blocked by:** 08 — Verify an extraction visually; 14 — Remove error bars; 18 — Extract multiple
point series; 19 — Extract line series; 22 — Corpus scoreboard.

**Status:** ready-for-agent

**Verification fixture order and complete image inventory.** Process the corpus in dependency
order, retaining an outcome, confidence, diagnostics and verification overlay for each image.

1. **Minimal supported baselines.** Start with `images/3-1-freefall.png`,
      `images/A+cleaned+up+scatter+plot.jpg`, `images/rplot.png` and `images/linegraph-3.png`. These
      need only the foundational, axis, export and review path, with the line image additionally using
      ticket 19.
2. **Single-series noise cases.** Then run `images/3-2-freefall-mimimum.png` and
      `images/Screenshot 2021-06-26 231058.png` after ticket 11, followed by
      `images/unnamed-chunk-9-1.png` and `images/zivEp.png` after ticket 12. These are still
      single-panel charts, but their grid or shaded-panel masks must be verified before scoring.
3. **First multi-series cases.** Run `images/Screenshot 2024-09-15 131643.png`,
      `images/Screenshot 2024-09-15 131740.png`, `images/Screenshot 2024-09-15 131805.png`,
      `images/Hysteresis-loop-of-cobalt-ferrite-samples-CF-CF300_30-and-CF600_180.png`,
      `images/rnaseqdedemo_19.png` and `images/Graph-1.jpg` after tickets 15-18. Run
      `images/Screenshot 2021-06-26 230901.png` only after its annotation and series-isolation
      prerequisites, and run `images/Inseam-v-Height-Graph.jpg` and `images/data.png` last as dense
      cloud stress cases.
4. **Full 2024 regression set.** After the appropriate noise, legend/style, segmentation and
      extraction prerequisites, process `images/Screenshot 2024-09-15 131234.png`,
      `images/Screenshot 2024-09-15 131309.png`, `images/Screenshot 2024-09-15 131341.png`,
      `images/Screenshot 2024-09-15 131401.png`, `images/Screenshot 2024-09-15 131423.png`,
      `images/Screenshot 2024-09-15 131439.png`, `images/Screenshot 2024-09-15 131522.png`,
      `images/Screenshot 2024-09-15 131556.png`, `images/Screenshot 2024-09-15 131624.png`,
      `images/Screenshot 2024-09-15 131712.png`, `images/Screenshot 2024-09-15 131643.png`,
      `images/Screenshot 2024-09-15 131740.png` and `images/Screenshot 2024-09-15 131805.png`.
      Use tickets 14 and 13 before the error-bar and annotation cases, and ticket 19 for the mixed
      line figures. The first three entries in this list are repeated from step 3 intentionally as
      part of the small-to-large regression progression.
5. **Expected declines.** Finally process `images/scatter_and_hist_border.png`,
      `images/Offset_Multiple_Y_Axes_Plot_of_YBCO_Superconductor_Growth_Study.png` and
      `images/VLObject-2561-031201081203.png`. Record the first as a multi-panel decline and the last
      two as multiple-offset-axis declines; do not emit digitized data for them.

This inventory covers all 30 image files in `images/`. The 2024 screenshot set in step 4 is
intentionally listed after the minimal cases even though several files are individually supported:
they combine legends, multiple series, log axes, annotations, error bars or dense linework and are
not cheap first checks.

- [ ] Every image in the corpus is processed and its outcome recorded.
- [ ] Each record states the outcome, the series found, the confidence, and any diagnostics raised.
- [ ] Every figure with ground truth beside it carries a measured median error and a pass or fail,
      not an opinion.
- [ ] Verification overlays are kept for every image so the results can be reviewed by eye.
- [ ] Figures that were declined are listed separately from figures that failed, with the reason for
      each decline.
- [ ] The multi-panel composite and the multiple-offset-axis figure are confirmed as declined rather
      than mangled.
- [ ] No figure produces high-confidence output that the overlay or the score shows to be wrong.
- [ ] Recurring failure patterns are written up as candidate follow-up work rather than fixed
      ad hoc inside this ticket.
- [ ] The findings note whether the deferred work — the review interface and the synthetic corpus —
      is now justified by the results.
