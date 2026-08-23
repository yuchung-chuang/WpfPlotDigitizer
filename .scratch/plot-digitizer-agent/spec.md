# Spec: agent-first plot digitizer skills

Status: ready-for-agent
Tracker: local (`.scratch/plot-digitizer-agent/issues/`)

## Problem Statement

Digitizing a chart with PlotDigitizer today is a manual, single-series chore. The user opens the
WPF app, walks a fixed six-page workflow, and at the Filter step picks **one** global RGB range.
That range is the only mechanism for saying "this ink is my data", so a figure with three
overlapping series has to be digitized three times, once per pass, with the user hand-tuning colour
tolerances each time. Nothing in the application knows what a *series* is.

Everything the user actually cares about is invisible to the app:

- A **legend** tells a human exactly how many series there are and how to tell them apart. The app
  never looks at it.
- **Marker shape** and **size** separate series that share a colour. The app only sees colour.
- **Error bars**, **grid lines**, **in-plot annotations** and **shaded backgrounds** are ink that is
  not data. The app cannot distinguish them, so they silently corrupt the extracted centroids.
- Axis detection is a brittle heuristic (Otsu threshold, largest contour, corner probing) that
  assumes a white background and a clean rectangular frame. It has no fallback when it is wrong, and
  no way to say that it is unsure.

The result is that the app works on tidy, single-series charts and degrades to "useless" on the
kind of real published figure the user actually has — the multi-series hysteresis loops, the
multi-axis superconductor plots, the multi-panel scatter-and-histogram composites sitting in
`images/`. Worse, when it fails it fails *silently*, emitting plausible-looking numbers that are
wrong.

The deeper problem is architectural. The workflow is hard-coded — `Load → Axis → Range → Filter →
Edit → Data` — so every new chart feature means a new page, a new setting, and a new node in the
dependency graph. The long tail of chart layouts is effectively infinite, and a fixed pipeline can
never cover it.

## Solution

Stop building a fixed pipeline and start building a **toolkit for an agent**.

Ship a set of agent **skills** — markdown instructions backed by small, single-purpose Python
scripts — that give any coding agent the pixel-accurate computer-vision operations it cannot
perform by eye. The agent supplies what the scripts cannot: judgement, vision, and the freedom to
choose its own order of operations.

The division of labour is the core idea:

- **The agent** looks at the chart, decides what kind of figure it is, reads the legend entries and
  the tick labels with its own vision, decides which noise filters this particular figure needs,
  inspects every intermediate result, and retries anything that looks wrong.
- **The scripts** do what an LLM provably cannot: locate an axis frame to the pixel, cluster colours
  in perceptually-uniform space, template-match marker shapes, skeletonize a line into an ordered
  polyline, and compute sub-pixel centroids.

Nobody hard-codes the workflow. There is no `PageService`, no fixed order, no six pages. The agent
reads a router skill that explains the artifact contract and the available operations, then works
the problem.

Three properties make this actually function rather than merely sound good:

1. **Every script emits an annotated overlay image, and the agent is required to look at it before
   continuing.** This is the feedback loop. Without it the agent orchestrates blind and can only
   trust numbers it has no way to check. With it, the agent has the same loop a human has: "that
   mask caught the gridlines as well as my data — retry with a tighter tolerance."
2. **State lives in files, not in the agent's context.** A per-image working directory holds a
   progressively-filled extraction document plus per-stage images. Scripts read and write those
   files and print only a short summary. Coordinate arrays never pass through the context window,
   where they would burn the budget and get silently truncated or rounded.
3. **Noise filtering is non-destructive.** Each filter contributes a named mask layer rather than
   mutating a working image, so any layer can be dropped or re-run without starting over, and the
   overlay can render each layer in a different colour to show *which* filter ate the data.

There is no LLM provider in the product. The user's own agent is the LLM, so the framework works
with any model the user prefers and requires no API keys, no configuration and no network calls of
its own.

The legend is a **hint, not the mechanism**. Unsupervised segmentation always runs; the legend, when
found, names and validates the clusters it discovered and supplies the expected series count.
Disagreement between the two is a confidence signal. If the legend were load-bearing, a third of the
target corpus would be dead on arrival, because it has no legend at all.

## User Stories

**Getting started**

1. As a researcher, I want to point my coding agent at a chart image and say "digitize this", so
   that I get data without learning a six-page desktop workflow.
2. As a researcher, I want the framework to work with whichever LLM I already pay for, so that I am
   not forced onto a vendor the tool chose for me.
3. As a researcher, I want no API keys, accounts or service configuration, so that I can use the
   tool on an air-gapped or locked-down machine.
4. As a researcher, I want the scripts to bootstrap their own dependencies on first run, so that I
   do not have to manage a virtual environment before I can try anything.
5. As an agent, I want a single router skill that explains the artifact contract and the available
   operations, so that I can plan my own approach instead of following a script.
6. As an agent, I want each skill group's instructions to load only when I actually need them, so
   that I spend my context window on the image rather than on documentation.

**Understanding the figure**

7. As an agent, I want a downscaled preview of the input image, so that I can look at the chart
   without spending my whole context budget on one high-resolution PNG.
8. As an agent, I want to know how many panels a figure contains, so that I can tell a single plot
   from a composite before I waste effort on the wrong region.
9. As an agent, I want the framework to flag figures that are out of scope, so that I decline them
   loudly instead of emitting confident nonsense.
10. As a researcher, I want to be told "this is a three-panel figure and I only handle single
    panels" rather than receiving silently wrong numbers, so that I can trust the output I do get.

**Finding the axes**

11. As an agent, I want the plot area located in pixel coordinates, so that I can restrict every
    later operation to the region that contains data.
12. As an agent, I want two independent axis-detection strategies whose results I can compare, so
    that agreement raises my confidence and disagreement warns me to look closer.
13. As an agent, I want an overlay showing the detected axis rectangle drawn on the original image,
    so that I can see at a glance whether it latched onto the frame or onto something else.
14. As an agent, I want to override the detected plot area with coordinates I choose, so that I can
    recover when detection fails on an unusual layout.
15. As a researcher, I want axis detection to work on figures without a full rectangular frame, so
    that open-axis and spine-only plots are not automatically lost.

**Reading the scale**

16. As an agent, I want the tick-label regions cropped and upscaled into a small image, so that I
    can read the numbers with my own vision instead of relying on OCR.
17. As an agent, I want to hand back the label values I read together with their pixel positions,
    so that the script can fit the mapping from pixels to data units.
18. As an agent, I want the fit to detect logarithmic axes from the spacing of the tick positions,
    so that log plots are not linearly mis-projected.
19. As an agent, I want reversed axes handled, so that plots whose values decrease left-to-right
    come out correct rather than mirrored.
20. As an agent, I want the residual of the axis fit reported, so that a mis-read digit shows up as
    a bad fit rather than as a plausible wrong answer.
21. As a researcher, I want axis units and labels captured alongside the numbers, so that the
    exported data is self-describing.

**Removing what is not data**

22. As an agent, I want to remove the plot frame and tick marks, so that the border does not become
    a spurious data series.
23. As an agent, I want to remove grid lines, so that regularly-spaced rules do not pollute the
    extracted points.
24. As an agent, I want to remove background fills and shaded bands, so that a coloured panel does
    not swamp colour-based segmentation.
25. As an agent, I want to remove in-plot text and arrows, so that annotations are not mistaken for
    markers.
26. As an agent, I want to remove error bars and their caps, so that whiskers do not drag centroids
    away from the true data point.
27. As an agent, I want each filter to contribute a separate named mask layer rather than modifying
    a shared image, so that I can drop one filter without redoing all the others.
28. As an agent, I want an overlay that renders each mask layer in its own colour, so that I can see
    exactly which filter removed which pixels.
29. As an agent, I want to re-run a single filter with different parameters, so that I can tune an
    over-aggressive filter in isolation.
30. As a researcher, I want to see that the gridline filter did not also erase my thin line series,
    so that I catch the most likely failure mode immediately.

**Identifying the series**

31. As an agent, I want the legend box located, so that I can read what each series is called.
32. As an agent, I want each legend entry's swatch and label cropped separately, so that I can pair
    a visual style with a name.
33. As an agent, I want legends outside the plot area found as well as legends inside it, so that
    figures with a legend in the margin are handled.
34. As an agent, I want the legend recorded as a protected region during analysis, so that noise
    filters do not destroy it before I have read it.
35. As an agent, I want the legend excluded from the plot region during extraction, so that its
    marker swatches are not digitized as data points.
36. As an agent, I want each swatch profiled into a colour, marker shape, marker size, line style
    and line width, so that I have a concrete signature to search for in the plot.
37. As a researcher, I want series distinguished by marker shape even when they share a colour, so
    that monochrome and print-oriented figures work.
38. As a researcher, I want series distinguished by colour even when they share a marker shape, so
    that colour-coded scatter plots work.
39. As an agent, I want unsupervised clustering to run whether or not a legend was found, so that
    figures with no legend still produce separated series.
40. As an agent, I want to be told when the number of clusters I found disagrees with the number of
    legend entries, so that I can investigate rather than silently mislabel.
41. As an agent, I want to merge or split discovered clusters, so that I can correct a segmentation
    that split one series in two or fused two into one.

**Extracting the data**

42. As an agent, I want a per-series mask produced for each identified series, so that extraction
    operates on one series at a time.
43. As an agent, I want sub-pixel centroids from a marker mask, so that scatter points are as
    accurate as the rendering allows.
44. As an agent, I want blobs whose area is a wild outlier discarded, so that merged or clipped
    markers do not produce phantom points.
45. As an agent, I want overlapping markers detected and reported, so that I know the point count is
    an undercount rather than assuming it is complete.
46. As an agent, I want a line mask reduced to an ordered polyline, so that curves come out as a
    traversable sequence rather than an unordered pixel cloud.
47. As an agent, I want gaps in a dashed line bridged, so that a dashed series is traced as one line
    rather than dozens of fragments.
48. As an agent, I want line crossings resolved by direction continuity, so that two intersecting
    curves are not swapped at the crossing point.
49. As an agent, I want to choose the sampling density of a traced line, so that I can trade file
    size against fidelity.
50. As a researcher, I want both scatter and line series extracted from the same figure, so that
    mixed plots work in one pass.

**Reviewing and exporting**

51. As an agent, I want the extracted data re-rendered over the original image, so that I can
    visually confirm the result before declaring success.
52. As a researcher, I want a verification overlay to look at, so that I can judge quality myself
    rather than taking the agent's word for it.
53. As a researcher, I want the extracted data as a tidy CSV with a series column, so that I can
    load it straight into my analysis tool.
54. As a researcher, I want the full extraction document including confidences and diagnostics, so
    that I can see how much to trust each series.
55. As a researcher, I want bulk coordinate arrays kept out of the summary document, so that the
    document stays readable and the agent's context stays intact.
56. As a researcher, I want per-series files as well as a combined export, so that I can take one
    series without filtering the rest out.
57. As an agent, I want a running record of which operations I performed, so that I can explain what
    I did and resume after an interruption.

**When things go wrong**

58. As an agent, I want every script to report a confidence rather than throwing on an expected
    condition, so that a weak result is something I can act on rather than a crash.
59. As an agent, I want structured diagnostics naming the stage and the affected pixel region, so
    that I know where to look rather than guessing.
60. As an agent, I want the working directory preserved on failure, so that I can inspect what the
    last successful stage produced.
61. As a researcher, I want to delete the working directory and start over cleanly, so that a bad
    run does not poison the next one.

## Implementation Decisions

### Shape of the deliverable

- The deliverable is an **agentic framework**, not a library or an application. It consists of skill
  documents, an agent definition, cross-agent instructions, and Python scripts.
- **No LLM provider is integrated.** The consuming agent is the LLM. Nothing in the framework makes
  a network call, reads an API key, or depends on a particular model.
- **The agent orchestrates freely.** There is no fixed stage order. The router skill documents the
  contract and the available operations; the agent chooses the sequence, repeats steps and
  backtracks as it sees fit.
- The existing .NET projects — Core, WPF, Web, CLI, Blazor and their tests — are **out of scope and
  untouched**. They serve only as a reference implementation to port algorithms *from*.

### Skill organisation

- Skills live under the repository's existing skills directory, so the editor discovers them
  automatically and they sit alongside the current repo skills.
- **Six discoverable skill groups plus one router.** Groups: chart overview, axis analysis, noise
  filtering, data segmentation, review, and export. Fifteen separately-discoverable skills would
  produce fifteen near-identical descriptions and the agent would pick wrong; six coherent phases
  keep the descriptions distinguishable.
- Each group folder holds a `SKILL.md` describing the group's workflow and a companion reference
  document holding per-script detail that the agent loads only when it runs that script. This is
  progressive disclosure: the router is cheap, the group is moderate, the reference is expensive and
  loaded last.
- The router's job is the **artifact contract, the working-directory layout, and the
  overlay-viewing rule** — explicitly *not* sequencing, which belongs to the agent.
- A cross-agent instructions file at the repository root carries the same contract, so agents other
  than the editor's own can use the framework.
- Skill folders are **self-contained**: no imports from repository paths outside the skills
  directory, so the whole set can later be copied into another repository without refactoring.

### Runtime and invocation

- Scripts are **Python**, run as **single-file scripts with inline dependency metadata** via `uv`.
  No virtual environment management, no requirements file, no install step.
- Algorithms are **ported, not bound**. The C# and MATLAB code is read for its ideas; nothing links
  against Emgu CV or the .NET assemblies. The genuinely good parts to carry over are the corner-probe
  axis search, the contour-moment centroid with its degenerate-moment fallback, the linear-and-log
  projection, the flood-fill border clearing, the morphological-opening trick that erases thin lines
  while leaving markers intact, the area z-score outlier rejection, the Hough-lines-constrained-to-
  axis-aligned frame detection, and the overlapping-text-box merge.
- **The agent invokes scripts through the terminal.** Not MCP — that adds a server lifecycle and a
  per-agent install step to something that should be drop-in, and not every agent speaks it.
- Every script shares an **identical dependency block** so the package resolver produces and caches a
  single environment rather than one per script. The dependency set is OpenCV (headless), NumPy,
  SciPy, scikit-image, scikit-learn and Pillow.
- A small shared helper package sits beside the skill groups and is imported by path bootstrap from
  each script. It owns working-directory resolution, extraction-document load/save and validation,
  mask-layer input and output, overlay rendering, and the common argument and summary conventions.

### Artifact contract

- All state for one image lives in a **per-image working directory**, defaulting to a
  conventionally-named folder under the current directory and overridable by argument. It is
  ignored by version control.
- The directory holds one **extraction document** — a JSON file that every script reads and
  progressively fills — plus per-stage overlay images and mask layers.
- **Scripts print a short human-readable summary to standard output, never bulk data.** The
  extraction document is the transport; the console is for the agent's attention.
- **Bulk coordinate arrays are stored in per-series sidecar files.** The extraction document holds
  counts and file references only. This is the rule that protects the agent's context window and it
  is not negotiable — a two-thousand-point array in the document defeats the entire design.
- The extraction document's sections are: image identity; chart classification and support verdict;
  plot area with confidence and detection method; per-axis limits, scale type, log base, reversal
  flag, pixel anchors and fit residual; legend box and per-entry swatch and label boxes; protected
  regions; mask layers with their kind and pixel counts; series with identity, label, style profile,
  kind, mask reference and point-file references; diagnostics; and a stage log.
- **Nothing throws for an expected condition.** Every operation returns a value plus a confidence in
  the unit interval plus a list of structured diagnostics carrying severity, stage, message and an
  optional pixel region. This contract is load-bearing: it feeds the agent's retry decisions, the
  "unsupported, here is why" report, and the content of the overlays.

### Feedback loop

- **Every script emits an annotated overlay image, and every skill's contract requires the agent to
  view it before proceeding.** Mandatory, not optional. This is what makes free-form orchestration
  viable rather than blind.
- Overlays draw on a copy of the original image so the agent always sees results in context.
- Mask-layer overlays render each layer in its own colour, so an over-aggressive filter is visible
  rather than merely suspected.

### Noise filtering

- Filters are **non-destructive named mask layers**, unioned into a derived plot mask. Any layer can
  be dropped or re-run independently. Destructive chaining would make "undo one step" impossible,
  and with a non-deterministic orchestrator that capability is needed constantly.
- Vocabulary, fixed to stop two filters fighting over the same pixels:
  - **Background** — area fills, gradients and shaded or alternating bands. Area, not ink.
  - **Grid lines** — thin straight lines at regular spacing aligned to the ticks.
  - **Annotation** — any text or arrow overlaid on the chart.
  - **Border** — the plot frame itself plus tick marks; ink touching the plot-area boundary.
  - **Error bar** — whisker segments with or without caps, attached to a data marker.
- **The legend is protected during analysis and removed before extraction.** It is inspected to
  determine series styles, its bounding box is recorded early so filters leave it alone, and it is
  then masked out of the plot region so its swatches are never digitized as data.

### Segmentation and extraction

- **Unsupervised segmentation always runs**; the legend names and validates the clusters it found
  and supplies an expected count. A count mismatch lowers confidence and is reported as a
  diagnostic rather than silently reconciled.
- Colour clustering operates in a **perceptually-uniform colour space**, not RGB, because RGB
  distance does not match human judgement of "same colour" and the whole point is to reproduce what
  a reader sees.
- Series identity is the tuple of colour, marker shape, marker size, line style and line width. Any
  one of these alone is insufficient on real figures.
- Scatter extraction is contour centroids with area-outlier rejection, preceded by morphological
  opening to strip connecting lines.
- Line extraction is skeletonization followed by ordered traversal, with gap bridging for dashed
  strokes and direction-continuity heuristics at crossings.
- Pixel-to-data projection supports linear and logarithmic axes and reversed directions.

### Scope of chart support

- **In scope:** single-panel figures with one X/Y axis pair, linear or logarithmic, carrying any
  number of scatter and/or line series, with error bars detected and excluded.
- **Detected and declined, never silently mangled:** multi-panel composites, multiple offset Y axes,
  and secondary axes.
- **Best effort only:** photographed or heavily JPEG-compressed charts.

### Delivery shape

- Work proceeds in **vertical slices, each independently runnable by the user on a named image from
  the corpus**. The user evaluates each increment personally and may stop at any point, so "usable
  at every step" outranks "complete".
- The first slice is a single-series chart end to end — overview, axis location, axis scale, point
  extraction, export, verification — deliberately skipping noise filtering and legend handling.

## Testing Decisions

There is **no automated test suite for this work, by explicit decision.** The user will evaluate
each increment personally against real images and may halt the project if quality is inadequate.
Building a harness before knowing whether the approach works at all would be effort spent on the
wrong thing. This is recorded as a deliberate trade, not an oversight, and it carries a known cost:
a change to one script can silently break another and nothing will catch it.

What replaces a suite:

- **The seam is the script command line.** Every operation is exercised the same way the agent
  exercises it: run the script against a real image with a working directory, then inspect the files
  it produced. This is the highest available seam and the only one — there is deliberately no second
  seam at the helper-package level, because a helper that is only reachable through a script is not
  worth a separate contract.
- **A good check at this seam is a behavioural one**: given this image, did the script write a valid
  extraction document, produce an overlay, and report a confidence consistent with what the overlay
  shows? Internal structure — which function computed the mask, how the clustering is implemented —
  is explicitly not checked, because all of it will change during tuning.
- **The overlay is the assertion.** Correctness at this stage is a human looking at an annotated
  image and agreeing. Every ticket's acceptance criterion is phrased that way: named image, expected
  visible result.
- **The corpus is the test set.** The repository's image folder holds the target figures, spanning
  trivial single-series plots through multi-series overlapping scatter to unsupported multi-panel
  composites. A final ticket runs every image and records per-image findings.
- **Prior art is limited and mostly a warning.** The existing MSTest project tests the .NET pipeline
  and is irrelevant here — different language, different seam, and its subject is out of scope. Its
  one transferable habit is that image assets are copied next to the tests and referenced by
  relative path; the Python scripts should likewise reference corpus images by repository-relative
  path rather than absolute.

Deferred, and expected to become the real suite later: a **synthetic corpus** — charts rendered from
known input arrays, so ground truth is exact and free — scored on median and 95th-percentile
absolute error in axis units normalised by axis range, plus series count and point recall. Only
once the skills exist and the user has judged the approach worth continuing.

## Out of Scope

- **The review and approval UI.** Results must eventually be presented to the user for review and
  approval, but nothing is built for it in this work. The verification overlay is the interim
  review surface.
- **The synthetic corpus, the accuracy metric, and any regression harness.** Deferred by decision.
- **Hand-labelled ground truth** for any image.
- **All existing .NET projects.** Core, WPF, Web, CLI, Blazor and their tests are untouched. No
  integration between the framework and the desktop or web applications; no consumption of the
  extraction document by the existing editor.
- **Bar charts, histograms and stacked bars.**
- **Extraction from multi-panel figures and figures with multiple or offset Y axes.** These are
  detected and declined only.
- **Pie charts, ternary plots, polar plots, 3-D plots and any non-Cartesian chart.**
- **Any LLM provider integration**, API key handling, response caching or cost management.
- **Packaging the skills for distribution** to other repositories. They are built self-contained so
  this stays cheap later, but no packaging work is done now.
- **OCR.** Tick labels and legend text are read by the consuming agent's vision, not by a text
  recognition engine.

## Further Notes

**On the choice of Python over reusing the C# code.** Code reuse was the weaker consideration. What
decided it was the agent's iteration loop — a script the agent can edit and re-run in seconds beats
one behind a compile step, and free-form orchestration means constant parameter fiddling. Behind
that, the library gap is real: perceptually-uniform colour clustering, skeletonization, density
clustering, template matching and curve fitting are all off-the-shelf in the Python ecosystem and
about half-present in Emgu. Third, a framework should run anywhere, and the existing command-line
project is pinned to Windows through its image loading.

**On what was inverted from the original idea.** The initial framing was legend-driven: read the
legend, then segment by what it says. That was inverted deliberately. Several images in the corpus
have no legend at all and one has it outside the plot area, so a legend-load-bearing design would
lose a third of the target set immediately. Making segmentation unsupervised and the legend a
corroborating signal keeps everything working and turns legend disagreement into useful information.

**On what the existing code got right.** The .NET implementation and the MATLAB spikes are not being
discarded for lack of quality. The corner-probe axis search degrades more gracefully than naive
Hough detection; the centroid routine's fallback for degenerate moments is a real bug fix worth
carrying; the morphological-opening trick for separating markers from lines is elegant; and the
area z-score outlier rejection is a cheap, effective guard. These should be ported as ideas rather
than reimplemented from scratch.

**On the known weakness of this plan.** Free-form agent orchestration plus no regression net plus no
ground truth means results are not reproducible run to run and quality is judged by eye. That is
acceptable while establishing whether the approach works at all, and it is the first thing that
should change if the user decides to continue past the initial evaluation.

**On the corpus.** The repository's image folder is the target set: roughly twenty figures ranging
from a simple two-variable scatter, through a hysteresis loop whose three series are separable only
by marker shape and colour under heavy overlap, to a superconductor plot with three offset Y axes,
two reversed X axes, a log axis and error bars, and a three-panel scatter-and-histogram composite
with the legend outside the axes. The last two are expected to be declined rather than extracted.
