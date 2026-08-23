# Digitize charts with an agent skill framework, not a fixed pipeline

The existing .NET application walks a hard-coded six-step workflow and models a figure as a single
colour range, so it cannot represent more than one series and degrades to useless on real published
figures — overlapping series separable only by marker shape, error bars, grid lines, shaded panels.
The long tail of chart layouts is effectively infinite, so we are replacing the fixed pipeline with a
set of agent skills: markdown instructions backed by small single-purpose scripts, orchestrated by
whichever coding agent the user already has. The agent brings vision and judgement — reading legends
and tick labels, choosing which filters this figure needs, inspecting every intermediate result —
and the scripts bring the pixel-accurate operations an LLM cannot perform.

## Considered options

**Extend the existing dependency graph to hold multiple series.** Rejected: the graph is a UI state
mechanism built for lazy invalidation during interactive editing, and every new chart feature would
still mean another page, another setting and another node. It makes the common case slightly better
and the long tail no better at all.

**A deterministic analysis library with an optional vision model behind an interface.** Rejected
after discussion. It keeps the fixed-pipeline problem while adding a provider dependency, and the
semantic steps it would delegate — legend parsing, tick reading — are exactly the steps the user's
own agent can already do for free.

**Ship our own model integration.** Rejected: it would force a vendor choice on the user, require
API key handling, and make the tool unusable on a locked-down machine. The consuming agent is the
model, so the framework needs no provider, no credentials and no network access of its own.

## Consequences

The workflow is no longer reproducible run to run, because the agent chooses its own order of
operations. We accept this: correctness is judged by looking at overlays, and the alternative —
constraining the agent back into a fixed sequence — reintroduces the problem this decision exists to
solve.

The existing .NET projects stay untouched and become a reference implementation to port algorithms
from rather than code to extend.
