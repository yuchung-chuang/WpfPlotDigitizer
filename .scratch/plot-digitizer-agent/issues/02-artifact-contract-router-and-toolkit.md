# 02 — Artifact contract, router skill, and shared toolkit

**What to build:** The foundation every other skill stands on: an agent can read one router
document, understand how the whole framework passes state around, and run a self-check that proves
the contract works on a real image.

The router document teaches the agent three things and deliberately not a fourth: the working
directory layout, the extraction document that every script reads and progressively fills, and the
rule that every script emits an annotated overlay which the agent must look at before continuing.
It must *not* prescribe an order of operations — choosing the sequence is the agent's job.

The shared toolkit is the code every later script imports: resolving and creating a working
directory, loading and saving the extraction document with validation, reading and writing mask
layers, rendering overlays onto a copy of the original image, and the common conventions for
command-line arguments and the short summary each script prints.

The self-check is what makes this ticket verifiable on its own: run it against a generated minimal
image and it initialises a working directory, writes a valid extraction document recording the image
identity, renders a trivial overlay, and prints a one-line summary.

**Blocked by:** 01 — Domain glossary and architecture decisions.

**Status:** fully completed and tested

**Contract self-check.**

`test_check_setup.py` creates a 16x12 generated PNG and invokes `check_setup.py` twice in a temporary
working directory. This verifies working-directory initialization, extraction-document creation,
image identity, trivial overlay rendering, dependency imports, and repeat-safe state handling. No
corpus image is required; ticket 02 has no chart-analysis or accuracy verification requirement.

- [x] A router skill is discoverable by the editor and describes the working directory layout, the
      extraction document, and the mandatory overlay-viewing rule.
- [x] The router explicitly states that the agent chooses its own order of operations.
- [x] The router documents how to check the script runner is installed and what to do if it is not.
- [x] The extraction document schema is written down, covering image identity, chart classification,
      plot area, axes, legend, protected regions, mask layers, series, diagnostics and a stage log.
- [x] The schema records that bulk coordinate arrays live in per-series sidecar files and the
      document holds only counts and references.
- [x] A shared toolkit provides working-directory resolution, extraction document load, save and
      validation, mask layer input and output, and overlay rendering.
- [x] Every script that will ever be written can bootstrap the toolkit without a virtual environment
      or an install step, and all scripts declare an identical dependency set so one cached
      environment serves them all.
- [x] The toolkit's failure convention is in place: operations return a value, a confidence between
      zero and one, and a list of structured diagnostics carrying severity, stage, message and an
      optional pixel region. Expected conditions never raise.
- [x] A self-check script runs against a generated minimal image, creates the working directory,
      writes a valid extraction document, renders an overlay, and prints a short summary.
- [x] `test_check_setup.py` runs the self-check twice on the generated image and confirms that the
      extraction document and overlay remain valid.
- [x] The skill folder is self-contained and imports nothing from outside the skills directory.
