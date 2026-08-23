# 09 — Cross-agent instructions and agent definition

**What to build:** The framework becomes usable outside this editor. An agent arriving cold — in
another tool, or in a fresh session with no history — can read one root document and know what this
repository offers, how to invoke it, and what the rules are.

Deliberately sequenced after the tracer bullet, because writing the instructions before the
workflow's shape is known produces instructions that are wrong.

**Blocked by:** 08 — Verify an extraction visually.

**Status:** ready-for-agent

- [ ] A root instructions document readable by any agent describes the digitization framework, the
      working directory layout, the artifact contract and the mandatory overlay rule.
- [ ] It states that the agent chooses its own order of operations.
- [ ] It names the prerequisite tooling and how to check for it.
- [ ] A dedicated agent definition exists for chart digitization work.
- [ ] The repository's existing editor instructions point at the new skills alongside the current
      customization list.
- [ ] Following only the root document, an agent with no prior context can digitize a simple chart
      from the corpus end to end.
- [ ] No existing .NET project is modified.
