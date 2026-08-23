# /// script
# requires-python = ">=3.11"
# dependencies = [
#   "numpy>=2.0",
#   "opencv-python-headless>=4.10",
#   "scipy>=1.14",
#   "scikit-image>=0.24",
#   "scikit-learn>=1.5",
#   "pillow>=10.4",
# ]
# ///
"""Crop the tick labels for the agent to read, then fit the pixel-to-value mapping it reads back.

With no values supplied this finds the ticks and the text beside them and writes one upscaled
composite per axis. Given `--x`/`--y` pixel=value pairs it fits the axis, decides linear against
logarithmic on the residual, and writes the axes section of the extraction.
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path

sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

import cv2
import numpy as np

from pdkit import Overlay, Result, base_parser, finish, open_workspace, palette_colour, rect
from pdkit.imageio import write_image
from pdkit.ink import ink_mask

STAGE = "read-axis-scale"
UPSCALE = 3.0
TICK_REACH_FRACTION = 0.012
TICK_REACH_LIMIT = 12
TICK_WIDTH_FRACTION = 0.02
FRAME_COVERAGE = 0.7
LABEL_BAND_FRACTION = 0.16
JOIN_KERNEL_FRACTION = 0.004
TITLE_EXTENT_FRACTION = 0.6
LINE_THICKNESS = 5
LINE_ASPECT = 12
MIN_LABELS = 3
MAX_INTERIOR_LINES = 4
ROTATED_ASPECT = 1.8
LOG_MARGIN = 0.5
RESIDUAL_FRACTION = 0.01
RESIDUAL_FLOOR = 2.0
MAX_LISTED_TICKS = 16
CAPTION_WIDTH = 132


def main() -> int:
    parser = base_parser("Crop the tick labels, then fit the axis scale from the values read.")
    for name in ("x", "y"):
        parser.add_argument(
            f"--{name}",
            type=_pairs_argument,
            default=None,
            metavar="PIXEL=VALUE,...",
            help=f"the {name} labels you read, as pixel=value pairs (two or more)",
        )
        parser.add_argument(
            f"--{name}-scale",
            choices=("auto", "linear", "log"),
            default="auto",
            help=f"force the {name} axis scale instead of choosing on residual (default auto)",
        )
        parser.add_argument(
            f"--{name}-log-base",
            type=float,
            default=10.0,
            help=f"the base to use if the {name} axis is logarithmic (default %(default)s)",
        )
        parser.add_argument(f"--{name}-title", default=None, help=f"the {name} axis title")
        parser.add_argument(f"--{name}-unit", default=None, help=f"the {name} axis unit")
    parser.add_argument(
        "--crop",
        action="store_true",
        help="crop the labels even though values were supplied",
    )
    parser.add_argument(
        "--upscale",
        type=float,
        default=UPSCALE,
        help="how much to enlarge each label crop (default %(default)s)",
    )
    args = parser.parse_args()

    workdir, extraction, image = open_workspace(args)
    result = Result(stage=STAGE)

    area = extraction.plot_area
    if not all(key in area for key in ("x", "y", "width", "height")):
        result.fail("no plot area recorded; run locate_plot_area.py first")
        return finish(workdir, extraction, STAGE, Path(__file__).name, result, [], None)

    box = rect(area["x"], area["y"], area["width"], area["height"])
    chart = extraction.data.get("chart", {})
    ink, text = ink_mask(image), _text_boxes(image)
    axes = {name: _survey(ink, text, box, name, chart, result) for name in ("x", "y")}
    fitting = args.x is not None or args.y is not None

    lines = []
    fits: dict[str, dict] = {}
    if fitting:
        fits = _fit_axes(args, box, axes, extraction, result, lines)
    if args.crop or not fitting:
        lines.extend(_crop_axes(workdir, image, axes, args.upscale, result))
        if not fitting:
            lines.append('next        --x "PIXEL=VALUE,..." --y "PIXEL=VALUE,..." from the crops')

    _store_labels(args, extraction, result)
    result.confidence = (
        min(fit["confidence"] for fit in fits.values())
        if fits
        else 0.4 * sum(1 for survey in axes.values() if survey["anchors"])
    )
    overlay = _draw(image, box, axes, fits).save(workdir.overlay_path(STAGE))
    return finish(workdir, extraction, STAGE, Path(__file__).name, result, lines, overlay)


# -- arguments -------------------------------------------------------------


def _pairs_argument(text: str) -> list[tuple[float, float]]:
    pairs = []
    for chunk in text.replace(" ", "").split(","):
        if not chunk:
            continue
        if chunk.count("=") != 1:
            raise argparse.ArgumentTypeError(f"expected PIXEL=VALUE, got {chunk!r}")
        pixel, value = chunk.split("=")
        try:
            pairs.append((float(pixel), float(value)))
        except ValueError as error:
            raise argparse.ArgumentTypeError(f"{chunk!r} is not two numbers") from error
    if len(pairs) < 2:
        raise argparse.ArgumentTypeError("an axis needs at least two pixel=value pairs")
    return pairs


# -- finding the ticks and the text beside them ----------------------------


def _survey(
    ink: np.ndarray, boxes: list[dict], box: dict, name: str, chart: dict, result: Result
) -> dict:
    """Ticks and tick-label boxes for one axis, taken from whichever line carries them.

    Usually that is an edge of the plot area, but a chart drawn with its axis through the origin
    hangs its labels off a line in the middle of the frame instead, so long lines inside the box
    are tried too once the edges have come up short.
    """
    shape = ink.shape
    sides = ("bottom", "top") if name == "x" else ("left", "right")

    edges = [_survey_line(ink, boxes, box, side, _edge(box, side), shape) for side in sides]
    chosen = max(edges, key=_carried)
    if len(chosen["labels"]) < MIN_LABELS:
        interior = [
            _survey_line(ink, boxes, box, side, line, shape)
            for line in _interior_lines(box, chart, name)
            for side in sides
        ]
        better = max(interior, key=_carried, default=chosen)
        if _carried(better) > _carried(chosen):
            result.note(
                f"the {name} labels hang off a line at {better['line']:.0f} px inside the plot "
                "area, not off its edge; the axis is drawn through the origin"
            )
            chosen = better

    if not chosen["labels"]:
        result.warn(
            f"no {name} tick labels found beside the plot area; "
            f"read the pixel positions off the overlay instead"
        )
    chosen["anchors"] = _pair(chosen["ticks"], chosen["labels"], name)
    chosen["name"] = name
    return chosen


def _survey_line(
    ink: np.ndarray, boxes: list[dict], box: dict, side: str, line: float, shape: tuple[int, int]
) -> dict:
    labels = _labels(boxes, box, side, line, shape)
    ticks = _ticks(ink, box, side, line, _clearance(labels, line, side))
    return {"side": side, "line": line, "ticks": ticks, "labels": labels}


def _carried(survey: dict) -> tuple[int, int]:
    return len(survey["labels"]), len(survey["ticks"])


def _interior_lines(box: dict, chart: dict, name: str) -> list[float]:
    """Long lines the chart overview saw running across the plot area, clear of its edges."""
    key = "horizontal_candidates" if name == "x" else "vertical_candidates"
    low = box["y"] if name == "x" else box["x"]
    high = low + (box["height"] if name == "x" else box["width"])
    candidates = chart.get("axis_lines", {}).get(key) or []
    return [line for line in sorted(candidates) if low + 6 < line < high - 6][:MAX_INTERIOR_LINES]


def _ticks(
    ink: np.ndarray, box: dict, side: str, line: float, clearance: float | None
) -> list[float]:
    """Short stubs attached to an axis line, on whichever face of it carries them.

    Read as runs of ink in a narrow band that starts just past the line itself and stops just
    short of the tick labels, kept only when they are too thin to be data crossing the band. Minor
    ticks come through too; pairing against the labels drops them again.
    """
    field = ink if side in ("left", "right") else ink.T
    height, width = ink.shape
    reach = min(TICK_REACH_LIMIT, max(4, int(round(TICK_REACH_FRACTION * min(height, width)))))

    start, span = _along(box, side in ("top", "bottom"))
    outward = -1 if side in ("left", "top") else 1
    lo, hi = max(0, int(start)), min(field.shape[0], int(start + span) + 1)
    if hi - lo < 4:
        return []

    found: list[float] = []
    for sign in (outward, -outward):
        base = _frame_edge(field, line, sign, lo, hi)
        room = int(clearance) - base if sign == outward and clearance is not None else reach
        depth = max(3, min(reach, room))
        band = _band(field, line, sign, base, base + depth)
        if band.shape[1] < 2:
            continue
        groups = _consecutive(np.flatnonzero(band[lo:hi].any(axis=1)))
        limit = _tick_width(groups, span)
        centres = [float(group.mean() + lo) for group in groups if group.size <= limit]
        if len(centres) > len(found):
            found = centres
    return found


def _tick_width(groups: list[np.ndarray], span: float) -> int:
    """How wide a run may be and still be a tick: twice the typical run, never a sizeable bar.

    A fixed width throws away the fatter ticks of an anti-aliased figure and keeps the frame line
    of a badly placed band, so the band's own runs set the scale and an absolute cap guards it.
    """
    if not groups:
        return 0
    typical = int(np.median([group.size for group in groups]))
    return max(3, min(2 * typical, int(round(TICK_WIDTH_FRACTION * span))))


def _edge(box: dict, side: str) -> float:
    """Where the named edge of the plot area sits."""
    return {
        "left": box["x"],
        "right": box["x"] + box["width"],
        "top": box["y"],
        "bottom": box["y"] + box["height"],
    }[side]


def _along(box: dict, horizontal: bool) -> tuple[float, float]:
    """Where the axis starts and how far it runs, along its own direction."""
    return (box["x"], box["width"]) if horizontal else (box["y"], box["height"])


def _band(field: np.ndarray, line: float, sign: int, inner: int, outer: int) -> np.ndarray:
    low, high = sorted((int(line) + sign * inner, int(line) + sign * outer))
    return field[:, max(0, low) : min(field.shape[1], high)]


def _frame_edge(field: np.ndarray, line: float, sign: int, lo: int, hi: int) -> int:
    """How far past the recorded edge the frame line itself still runs.

    The plot area is recorded at the frame's centre, near enough, so a band opened at a fixed
    offset lands back on a thick frame and reads it as one enormous tick.
    """
    span = max(hi - lo, 1)
    past = 1
    for step in range(-3, 7):
        column = int(line) + sign * step
        if 0 <= column < field.shape[1] and field[lo:hi, column].sum() >= FRAME_COVERAGE * span:
            past = step + 1
    return max(past, 1)


def _clearance(labels: list[dict], line: float, side: str) -> float | None:
    """How much room there is between the axis line and the nearest tick label."""
    if not labels:
        return None
    horizontal = side in ("top", "bottom")
    outward = -1 if side in ("left", "top") else 1
    return min(outward * (_faces(label, horizontal, outward)[0] - line) for label in labels)



def _consecutive(indices: np.ndarray, gap: int = 1) -> list[np.ndarray]:
    if indices.size == 0:
        return []
    return np.split(indices, np.flatnonzero(np.diff(indices) > gap) + 1)


def _text_boxes(image: np.ndarray) -> list[dict]:
    """Word-sized boxes over the whole figure.

    Ported from the desktop client's axis-text search: a morphological gradient lifts every glyph
    edge, Otsu binarises it, and a wide closing joins the characters of one word into one blob.
    Overlapping blobs are then merged, as the text prototype did, so a minus sign and its digits
    do not arrive as two labels.
    """
    grey = cv2.cvtColor(image, cv2.COLOR_BGR2GRAY)
    gradient = cv2.morphologyEx(
        grey, cv2.MORPH_GRADIENT, cv2.getStructuringElement(cv2.MORPH_ELLIPSE, (3, 3))
    )
    _, binary = cv2.threshold(gradient, 0, 255, cv2.THRESH_BINARY | cv2.THRESH_OTSU)

    join = max(5, int(round(JOIN_KERNEL_FRACTION * max(image.shape[:2]))))
    closed = cv2.morphologyEx(
        binary, cv2.MORPH_CLOSE, cv2.getStructuringElement(cv2.MORPH_RECT, (join, 1))
    )
    contours, _ = cv2.findContours(closed, cv2.RETR_EXTERNAL, cv2.CHAIN_APPROX_SIMPLE)
    glyphs = [rect(*cv2.boundingRect(contour)) for contour in contours]
    return _merge([glyph for glyph in glyphs if not _line_like(glyph)], join)


def _line_like(glyph: dict) -> bool:
    """A grid line or a frame edge, which would otherwise swallow the label it runs up against."""
    short, long = sorted((glyph["width"], glyph["height"]))
    return short <= LINE_THICKNESS and long >= LINE_ASPECT * max(short, 1)


def _merge(boxes: list[dict], slack: int) -> list[dict]:
    """Fold every pair of touching boxes into their union until none touch.

    `slack` closes the sideways gap the closing kernel could not reach across, which is what keeps
    a minus sign attached to the digits it belongs to.
    """
    merged = True
    while merged and len(boxes) > 1:
        merged = False
        kept: list[dict] = []
        for box in boxes:
            for index, other in enumerate(kept):
                if _overlaps(box, other, slack):
                    kept[index] = _union(box, other)
                    merged = True
                    break
            else:
                kept.append(box)
        boxes = kept
    return boxes


def _overlaps(a: dict, b: dict, slack: int) -> bool:
    return (
        a["x"] - slack < b["x"] + b["width"]
        and b["x"] - slack < a["x"] + a["width"]
        and a["y"] < b["y"] + b["height"]
        and b["y"] < a["y"] + a["height"]
    )


def _union(a: dict, b: dict) -> dict:
    x, y = min(a["x"], b["x"]), min(a["y"], b["y"])
    right = max(a["x"] + a["width"], b["x"] + b["width"])
    bottom = max(a["y"] + a["height"], b["y"] + b["height"])
    return rect(x, y, right - x, bottom - y)


def _labels(
    boxes: list[dict], box: dict, side: str, line: float, shape: tuple[int, int]
) -> list[dict]:
    """The one row or column of text sitting nearest the given axis line.

    Anything further out is the axis title or the figure caption, so only the nearest band is kept.
    """
    height, width = shape
    horizontal = side in ("top", "bottom")
    band = LABEL_BAND_FRACTION * (height if horizontal else width)
    outward = -1 if side in ("left", "top") else 1

    beside = []
    for candidate in boxes:
        near, far = _faces(candidate, horizontal, outward)
        distance = outward * (near - line)
        if not 0 <= distance <= band or outward * (far - line) < 0:
            continue
        if _extent(candidate, horizontal) > TITLE_EXTENT_FRACTION * _extent(box, horizontal):
            continue  # a box spanning the axis is its title, not one tick's label
        if _extent(candidate, not horizontal) > band:
            continue  # too deep to be one line of text beside the axis
        if not _straddles(candidate, box, horizontal):
            continue
        beside.append((distance, candidate))
    if not beside:
        return []

    nearest = min(distance for distance, _ in beside)
    depth = max(2.0, np.median([_extent(c, not horizontal) for _, c in beside]))
    row = [candidate for distance, candidate in beside if distance - nearest <= depth]
    return sorted(row, key=lambda candidate: _centre(candidate, horizontal))


def _faces(candidate: dict, horizontal: bool, outward: int) -> tuple[float, float]:
    low = candidate["y"] if horizontal else candidate["x"]
    high = low + (candidate["height"] if horizontal else candidate["width"])
    return (low, high) if outward > 0 else (high, low)


def _extent(candidate: dict, horizontal: bool) -> float:
    return candidate["width"] if horizontal else candidate["height"]


def _centre(candidate: dict, horizontal: bool) -> float:
    key = "x" if horizontal else "y"
    return candidate[key] + _extent(candidate, horizontal) / 2


def _straddles(candidate: dict, box: dict, horizontal: bool) -> bool:
    """Whether a label sits within the axis's own span, with a label's width of slack at each end."""
    slack = _extent(candidate, horizontal)
    low = (box["x"] if horizontal else box["y"]) - slack
    high = low + _extent(box, horizontal) + 2 * slack
    return low <= _centre(candidate, horizontal) <= high


def _pair(ticks: list[float], labels: list[dict], name: str) -> list[dict]:
    """Give each label the tick it belongs to, falling back to its own centre when there is none."""
    horizontal = name == "x"
    centres = [_centre(label, horizontal) for label in labels]
    spacing = float(np.median(np.diff(centres))) if len(centres) > 1 else 0.0
    tolerance = max(8.0, 0.4 * abs(spacing))

    anchors = []
    for label, centre in zip(labels, centres):
        nearest = min(ticks, key=lambda tick: abs(tick - centre)) if ticks else None
        ticked = nearest is not None and abs(nearest - centre) <= tolerance
        anchors.append(
            {
                "pixel": round(nearest if ticked else centre, 1),
                "from": "tick" if ticked else "label",
                "box": label,
            }
        )
    return anchors


# -- the crops the agent reads ---------------------------------------------


def _crop_axes(workdir, image: np.ndarray, axes: dict, upscale: float, result: Result) -> list[str]:
    lines = []
    for name, survey in axes.items():
        ticks = " ".join(f"{tick:.0f}" for tick in survey["ticks"][:MAX_LISTED_TICKS])
        extra = len(survey["ticks"]) - MAX_LISTED_TICKS
        lines.append(
            f"{name} axis      {survey['side']:<6} edge   "
            f"{len(survey['ticks'])} ticks   {len(survey['labels'])} labels"
        )
        lines.append(f"{name} ticks     {ticks}" + (f" +{extra} more" if extra > 0 else ""))
        if not survey["anchors"]:
            continue
        path = _composite(workdir, image, survey, upscale)
        lines.append(f"{name} labels    {workdir.display(path)}   <-- read the labels here")
        loose = sum(1 for anchor in survey["anchors"] if anchor["from"] == "label")
        if loose:
            result.note(
                f"{loose} of {len(survey['anchors'])} {name} labels had no tick under them; "
                "their anchor is the centre of the text instead"
            )
    return lines


def _composite(workdir, image: np.ndarray, survey: dict, upscale: float) -> Path:
    """One strip per axis: every label, enlarged, against the pixel position it belongs to."""
    name = survey["name"]
    rotate = _mostly_rotated([anchor["box"] for anchor in survey["anchors"]])
    tiles = [_tile(image, anchor["box"], upscale, rotate) for anchor in survey["anchors"]]

    heading = f"{name} axis, {survey['side']} edge - pixel position, then the label"
    width = max(CAPTION_WIDTH + max(tile.shape[1] for tile in tiles) + 12, 9 * len(heading) + 16)
    rows = [_header(heading, width)]
    for anchor, tile in zip(survey["anchors"], tiles):
        rows.append(_row(f"{name}={anchor['pixel']:.0f}", tile, width))

    strip = np.vstack(rows)
    path = workdir.crops / f"axis-{name}-labels.png"
    write_image(path, strip)
    return path


def _mostly_rotated(boxes: list[dict]) -> bool:
    """Whether this axis's labels are drawn turned on their side rather than lying flat."""
    tall = sum(1 for box in boxes if box["height"] > ROTATED_ASPECT * box["width"])
    return tall * 2 > len(boxes)


def _tile(image: np.ndarray, box: dict, upscale: float, rotate: bool) -> np.ndarray:
    pad = 3
    height, width = image.shape[:2]
    x0 = max(0, int(box["x"]) - pad)
    y0 = max(0, int(box["y"]) - pad)
    x1 = min(width, int(box["x"] + box["width"]) + pad)
    y1 = min(height, int(box["y"] + box["height"]) + pad)
    crop = image[y0:y1, x0:x1]
    if crop.size == 0:
        return np.full((8, 8, 3), 255, np.uint8)
    if rotate:
        crop = cv2.rotate(crop, cv2.ROTATE_90_CLOCKWISE)
    return cv2.resize(crop, None, fx=upscale, fy=upscale, interpolation=cv2.INTER_LANCZOS4)


def _header(text: str, width: int) -> np.ndarray:
    band = np.full((28, width, 3), 240, np.uint8)
    cv2.putText(band, text, (8, 19), cv2.FONT_HERSHEY_SIMPLEX, 0.5, (40, 40, 40), 1)
    return band


def _row(caption: str, tile: np.ndarray, width: int) -> np.ndarray:
    height = max(tile.shape[0], 26) + 6
    canvas = np.full((height, width, 3), 255, np.uint8)
    top = (height - tile.shape[0]) // 2
    canvas[top : top + tile.shape[0], CAPTION_WIDTH : CAPTION_WIDTH + tile.shape[1]] = tile
    cv2.putText(
        canvas, caption, (8, height // 2 + 6), cv2.FONT_HERSHEY_SIMPLEX, 0.6, (150, 60, 20), 2
    )
    cv2.line(canvas, (0, height - 1), (width - 1, height - 1), (210, 210, 210), 1)
    return canvas


# -- fitting ---------------------------------------------------------------


def _fit_axes(
    args, box: dict, axes: dict, extraction, result: Result, lines: list[str]
) -> dict[str, dict]:
    fits = {}
    for name in ("x", "y"):
        pairs = getattr(args, name)
        if pairs is None:
            continue
        span = (box["x"], box["x"] + box["width"]) if name == "x" else (
            box["y"] + box["height"],
            box["y"],
        )
        fit = _fit(pairs, span, name, args, result)
        fit["title"] = getattr(args, f"{name}_title")
        fit["unit"] = getattr(args, f"{name}_unit")
        extraction.data["axes"][name] = fit
        fits[name] = fit
        lines.extend(_describe(name, fit))
    missing = [name for name in ("x", "y") if getattr(args, name) is None]
    if missing:
        result.warn(f"no values given for the {' and '.join(missing)} axis; it was left unfitted")
    return fits


def _fit(pairs: list[tuple[float, float]], span: tuple[float, float], name: str, args, result):
    """Fit pixel against value both ways round and keep the scale that predicts pixels better.

    Both residuals are in pixels, which is the only footing on which a linear and a logarithmic
    fit can be compared at all: a residual in data units is dominated by the largest value on a
    log axis and says nothing.
    """
    pixels = np.array([pair[0] for pair in pairs], float)
    values = np.array([pair[1] for pair in pairs], float)
    requested = getattr(args, f"{name}_scale")
    base = getattr(args, f"{name}_log_base")

    linear = _least_squares(values, pixels)
    logarithmic = None
    if np.all(values > 0) and base > 1:
        logarithmic = _least_squares(np.log(values) / np.log(base), pixels)

    scale, chosen = _choose(linear, logarithmic, requested, len(pairs), name, base, result)
    inverse = (lambda t: base**t) if scale == "log" else (lambda t: t)

    if abs(chosen["slope"]) < 1e-12:
        result.fail(f"the {name} values you gave are all the same; the axis cannot be fitted")
        chosen = {"slope": 1.0, "intercept": 0.0, "residual": float("nan")}

    ends = [float(inverse((pixel - chosen["intercept"]) / chosen["slope"])) for pixel in span]
    low, high = (0, 1) if ends[0] <= ends[1] else (1, 0)
    pixel_span = abs(span[1] - span[0]) or 1.0
    confidence = _confidence(chosen["residual"], pixel_span, len(pairs))

    fit = {
        "min": round(ends[low], 6),
        "max": round(ends[high], 6),
        "scale": scale,
        "log_base": base if scale == "log" else None,
        "reversed": bool(chosen["slope"] < 0) if name == "x" else bool(chosen["slope"] > 0),
        "pixel_min": round(span[low], 1),
        "pixel_max": round(span[high], 1),
        "title": None,
        "unit": None,
        "residual": round(chosen["residual"], 3),
        "residual_data": round(_data_residual(pixels, values, chosen, inverse), 6),
        "anchors": [[round(pixel, 1), value] for pixel, value in pairs],
        "confidence": confidence,
    }
    _check(fit, chosen["residual"], pixel_span, name, result)
    return fit


def _least_squares(transformed: np.ndarray, pixels: np.ndarray) -> dict:
    """Pixel as a straight line in the transformed value, with its residual in pixels."""
    design = np.vstack([transformed, np.ones_like(transformed)]).T
    (slope, intercept), *_ = np.linalg.lstsq(design, pixels, rcond=None)
    residual = float(np.sqrt(np.mean((design @ [slope, intercept] - pixels) ** 2)))
    return {"slope": float(slope), "intercept": float(intercept), "residual": residual}


def _choose(linear, logarithmic, requested, count, name, base, result):
    if requested == "log":
        if logarithmic is None:
            result.fail(f"a log {name} axis needs every value positive and a base above 1")
            return "linear", linear
        result.note(f"the {name} axis was declared logarithmic, base {base:g}")
        return "log", logarithmic
    if requested == "linear":
        return "linear", linear
    if logarithmic is None or count < 3:
        if logarithmic is not None:
            result.note(
                f"two {name} pairs fit either scale exactly, so linear was assumed; "
                f"give a third label, or --{name}-scale log, to settle it"
            )
        return "linear", linear
    if logarithmic["residual"] < LOG_MARGIN * linear["residual"]:
        result.note(
            f"the {name} axis is logarithmic: {logarithmic['residual']:.2f} px residual "
            f"against {linear['residual']:.2f} px linear"
        )
        return "log", logarithmic
    return "linear", linear


def _data_residual(pixels, values, chosen, inverse) -> float:
    predicted = inverse((pixels - chosen["intercept"]) / chosen["slope"])
    return float(np.sqrt(np.mean((np.asarray(predicted, float) - values) ** 2)))


def _confidence(residual: float, pixel_span: float, count: int) -> float:
    if not np.isfinite(residual):
        return 0.0
    tolerance = max(RESIDUAL_FLOOR, RESIDUAL_FRACTION * pixel_span)
    fit = max(0.0, 1.0 - residual / tolerance)
    return round(min(0.95, (0.75 if count < 3 else 0.95) * (0.4 + 0.6 * fit)), 3)


def _check(fit: dict, residual: float, pixel_span: float, name: str, result: Result) -> None:
    tolerance = max(RESIDUAL_FLOOR, RESIDUAL_FRACTION * pixel_span)
    if np.isfinite(residual) and residual > tolerance:
        result.warn(
            f"the {name} fit is {residual:.1f} px out against a {tolerance:.1f} px tolerance, "
            "which usually means a digit was misread; check the crops and resubmit",
        )
    if fit["reversed"]:
        result.note(f"the {name} axis runs backwards: the value falls as the pixel rises")
    if fit["scale"] == "log" and fit["min"] <= 0:
        result.warn(f"the {name} axis is logarithmic but its fitted minimum is not positive")


def _describe(name: str, fit: dict) -> list[str]:
    scale = fit["scale"] + (f" base {fit['log_base']:g}" if fit["log_base"] else "")
    titled = " ".join(part for part in (fit["title"], f"[{fit['unit']}]" if fit["unit"] else "") if part)
    return [
        f"{name} axis      {fit['min']:g} .. {fit['max']:g} over px "
        f"{fit['pixel_min']:.0f} .. {fit['pixel_max']:.0f}   {scale}"
        + ("   reversed" if fit["reversed"] else ""),
        f"{name} fit       {len(fit['anchors'])} anchors   residual {fit['residual']:.2f} px "
        f"({fit['residual_data']:g} in data units)   confidence {fit['confidence']:.2f}"
        + (f"   {titled}" if titled else ""),
    ]


def _store_labels(args, extraction, result: Result) -> None:
    """Titles and units survive a run that only re-fits, and a crop-only run that supplies them."""
    for name in ("x", "y"):
        entry = extraction.data["axes"].get(name)
        for field in ("title", "unit"):
            value = getattr(args, f"{name}_{field}")
            if value is None:
                continue
            if entry is None:
                result.note(f"no {name} axis to hang the {field} on; supply it with --{name}")
                continue
            entry[field] = value


# -- overlay ---------------------------------------------------------------


def _draw(image: np.ndarray, box: dict, axes: dict, fits: dict[str, dict]) -> Overlay:
    overlay = Overlay(image).rectangle(box, palette_colour(6), thickness=1)
    for index, (name, survey) in enumerate(axes.items()):
        colour = palette_colour(index)
        for label in survey["labels"]:
            overlay.rectangle(label, colour, thickness=1)
        overlay.points(_tick_points(survey, box), colour, size=11)
        overlay.key(f"{name} axis: {len(survey['ticks'])} ticks, {len(survey['labels'])} labels", colour)
        if name in fits:
            _annotate(overlay, fits[name], name, box, colour)
    return overlay


def _tick_points(survey: dict, box: dict) -> list[tuple[float, float]]:
    line = survey["line"]
    if survey["side"] in ("left", "right"):
        return [(line, tick) for tick in survey["ticks"]]
    return [(tick, line) for tick in survey["ticks"]]


def _annotate(overlay: Overlay, fit: dict, name: str, box: dict, colour) -> None:
    """Write the fitted value at each end of the axis, where a mirrored fit is obvious at a glance.

    Drawn just inside the plot area rather than out beside the tick labels, which is the one place
    on the figure guaranteed not to be already covered in text.
    """
    inside = 6
    for key, pixel in (("min", fit["pixel_min"]), ("max", fit["pixel_max"])):
        if name == "x":
            origin = (int(pixel) + inside, int(box["y"] + box["height"]) - inside)
        else:
            origin = (int(box["x"]) + inside, int(pixel) - inside)
        _text(overlay.canvas, f"{fit[key]:.4g}", origin, colour)


def _text(canvas: np.ndarray, text: str, origin: tuple[int, int], colour) -> None:
    (width, height), _ = cv2.getTextSize(text, cv2.FONT_HERSHEY_SIMPLEX, 0.45, 1)
    x = min(max(origin[0], 2), canvas.shape[1] - width - 2)
    y = min(max(origin[1], height + 2), canvas.shape[0] - 2)
    cv2.putText(canvas, text, (x, y), cv2.FONT_HERSHEY_SIMPLEX, 0.45, (255, 255, 255), 3)
    cv2.putText(canvas, text, (x, y), cv2.FONT_HERSHEY_SIMPLEX, 0.45, colour, 1)



if __name__ == "__main__":
    raise SystemExit(main())
