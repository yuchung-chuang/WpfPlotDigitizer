# Python scripts passing state through a working directory

The skills are Python single-file scripts with inline dependency metadata, run through `uv`, and they
pass state to each other through files in a per-figure working directory rather than through the
agent's context. Python won over reusing the existing C# because the agent's iteration loop matters
more than code reuse — a script the agent can edit and re-run in seconds beats one behind a compile
step, and free-form orchestration means constant parameter fiddling — and because perceptually
uniform colour clustering, skeletonization, density clustering and template matching are all
off-the-shelf in Python and roughly half-present in the current image library.

The file-based contract exists for one specific reason: **bulk coordinate data must never travel
through the agent's context.** A figure can hold thousands of points. Passing that array through the
context window burns the budget, and models silently truncate or round long numeric sequences.
Coordinates therefore live in sidecar files; the extraction document holds counts and references;
scripts print a short summary and nothing more.

## Consequences

Every operation must be expressible as a command that reads files and writes files, which rules out
returning rich objects between steps and makes the command line the only seam the framework has.

Because state is on disk rather than in memory, a failed run leaves everything the last successful
step produced, and any single step can be re-run without repeating the ones before it.

Algorithms are ported as ideas rather than bound to. The corner-probe axis search, the
contour-moment centroid with its degenerate-moment fallback, the linear-and-log projection, the
flood-fill border clearing, the morphological opening that strips lines while leaving markers, and
the area outlier rejection are all worth carrying over — but as new Python, linking against nothing.
