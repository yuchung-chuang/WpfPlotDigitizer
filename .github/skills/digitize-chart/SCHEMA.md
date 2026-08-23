# The extraction document

`extraction.json` in the working directory. Every script reads it, adds its own section, and saves
it. Unknown keys survive untouched, so a script only ever needs to understand the parts it uses.

Rectangles are always `{"x", "y", "width", "height"}` in pixels of the original figure, with the
origin at the top left.

## Shape

```jsonc
{
  "schema_version": 1,

  "image":   { "path": "...", "name": "...", "width": 0, "height": 0, "sha256": "..." },

  "chart":   { "type": "scatter", "panel_count": 1, "supported": true,
               "reason": null, "confidence": 0.9 },

  "plot_area": { "x": 0, "y": 0, "width": 0, "height": 0,
                 "method": "corner-probe", "confidence": 0.8 },

  "axes": {
    "x": { "min": 0, "max": 10, "scale": "linear", "log_base": null, "reversed": false,
           "pixel_min": 122, "pixel_max": 834, "title": null, "unit": null,
           "residual": 0.4, "confidence": 0.9 },
    "y": { "...": "same shape" }
  },

  "legend": {
    "box": { "x": 0, "y": 0, "width": 0, "height": 0 },
    "entries": [ { "index": 0,
                   "swatch": { "x": 0, "y": 0, "width": 0, "height": 0 },
                   "label_box": { "x": 0, "y": 0, "width": 0, "height": 0 },
                   "label": "CF300" } ],
    "confidence": 0.7
  },

  "protected_regions": [ { "name": "legend", "x": 0, "y": 0, "width": 0, "height": 0 } ],

  "mask_layers": [ { "name": "grid-lines", "kind": "noise",
                     "file": "masks/grid-lines.png", "pixels": 12043 } ],

  "series": [ {
    "id": "s1", "label": "CF300", "kind": "point",
    "style": { "colour_lab": [54.2, 80.1, 69.9], "colour_rgb": [220, 30, 40],
               "marker": "circle", "filled": true, "marker_size": 7,
               "line_style": "none", "line_width": 0 },
    "mask": "masks/series-s1.png",
    "points_pixel": "points/s1-pixel.csv",
    "points_data":  "points/s1-data.csv",
    "point_count": 214,
    "confidence": 0.85
  } ],

  "diagnostics": [ { "stage": "remove-grid-lines", "severity": "warning",
                     "message": "...", "region": null } ],

  "stages": [ { "name": "locate-plot-area", "script": "locate_plot_area.py",
                "ran_at": "2026-08-23T10:00:00+00:00", "confidence": 0.8, "summary": "..." } ]
}
```

## Rules

**Coordinates go in sidecars.** `points_pixel` and `points_data` hold *paths*, never arrays. A
document carrying inline coordinates fails validation.

**Mask layers are named and separate.** `kind` is `noise` for something removed, `series` for
something claimed by a series. Re-adding a layer under an existing name replaces it; the combined
`plot_mask.png` is the union of every `noise` layer and is derived, never edited.

**Protected regions are honoured by every filter.** The legend is protected as soon as it is found,
because it must survive long enough to be read, and is masked out later during segmentation.

**`supported: false` needs a `reason`.** That is what a decline is.

**Confidence is per-section and per-stage**, in `[0, 1]`. It records how much an operation trusted
itself at the moment it ran; nothing recomputes it later.

## Using it from a script

```python
import sys
from pathlib import Path
sys.path.insert(0, str(Path(__file__).resolve().parents[1] / "_lib"))

from pdkit import Overlay, Result, base_parser, finish, open_workspace, rect
```

`open_workspace` creates the folder tree and loads the figure and its extraction. `finish` records
the stage, validates, saves, and prints the summary. `Result` carries the value, the confidence and
the diagnostics — call `note`, `warn` or `fail` on it rather than raising, because an unsupported
figure or a missing legend is an expected outcome, not a crash.
