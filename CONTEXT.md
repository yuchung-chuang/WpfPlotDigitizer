# PlotDigitizer

Turning a picture of a 2-D chart back into the numbers that produced it.

## Language

### The figure

**Chart**:
A single 2-D plot with one horizontal and one vertical axis. A **figure** is the whole image file,
which may contain more than one chart.
_Avoid_: Graph, plot (as a noun for the whole image)

**Plot area**:
The rectangular region of a figure bounded by the axes, inside which data is drawn.
_Avoid_: Axis region, ROI, chart area

**Panel**:
One chart within a figure that contains several.
_Avoid_: Subplot, facet

**Border**:
The frame drawn around the plot area, together with its tick marks.
_Avoid_: Axis lines, spines, frame

**Grid line**:
A thin line spanning the plot area at regular spacing, aligned with a tick position.
_Avoid_: Rule, guide

**Background**:
Area fill within a figure — a solid colour, a gradient, or a shaded or alternating band. Background
is area, never ink.
_Avoid_: Fill, canvas

**Annotation**:
Text or an arrow overlaid on a chart. In-plot labels, fitted-equation text, callouts and
significance markers are all annotations.
_Avoid_: Label (which means a series name), caption

### The data

**Series**:
One set of related data drawn on a chart, distinguished from the others by its style. A series is
either a **point series** or a **line series**.
_Avoid_: Dataset, curve, trace, plot

**Marker**:
The glyph drawn at a single data point — a circle, square, triangle, diamond, cross or star, either
filled or open.
_Avoid_: Symbol, dot, point (which means a coordinate)

**Series style**:
The visual signature that distinguishes one series from another: its colour, marker shape, marker
size, line style and line width.
_Avoid_: Appearance, format, theme

**Legend**:
The key that names each series and shows its style. Inspected during analysis to learn the series
styles, then removed before data is extracted so its swatches are never mistaken for data.
_Avoid_: Key, caption

**Swatch**:
The sample of a series' style shown in one legend entry, as distinct from that entry's name.
_Avoid_: Icon, sample, glyph

**Error bar**:
A whisker drawn through a marker, with or without a perpendicular cap, showing uncertainty. Not data
to be digitized.
_Avoid_: Whisker, uncertainty bar

**Tick label**:
The printed number or category beside a tick mark, giving the data value at that pixel position.
_Avoid_: Axis label (which means the axis title), scale marker

### The work

**Extraction**:
The complete result of digitizing one figure — what was found, what was rejected, how confident each
step was, and the resulting series.
_Avoid_: Result, output, digitization

**Working directory**:
The folder holding everything produced while digitizing one figure: the extraction, its overlays and
its mask layers.
_Avoid_: Temp folder, cache, output folder

**Mask layer**:
A named record of which pixels one operation claimed, kept separately from every other operation's
so that it can be dropped or recomputed on its own.
_Avoid_: Filter, layer, mask

**Protected region**:
An area of a figure that no filter may claim, because a later step still needs to read it.
_Avoid_: Exclusion zone, keep-out

**Overlay**:
An annotated copy of the original figure showing what an operation found, produced so the result can
be judged by eye.
_Avoid_: Preview, debug image, visualization

**Confidence**:
How much an operation trusts its own result, from zero to one.
_Avoid_: Score, certainty, quality

**Diagnostic**:
A structured note from an operation about something worth attention — what happened, how serious it
is, and where in the figure.
_Avoid_: Warning, error, log entry

**Decline**:
To identify a figure as beyond the supported scope and refuse it, with a reason. A decline is a
successful outcome; silently producing wrong numbers is not.
_Avoid_: Fail, reject, skip
