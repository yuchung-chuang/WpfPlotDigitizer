---
name: test-coverage-audit
description: Audit PlotDigitizer test coverage, rank the untested code by risk, and produce a prioritized test backlog.
argument-hint: "[optional: project or folder to scope the audit]"
agent: agent
tools: ['search', 'runCommands', 'usages', 'problems', 'todos']
---

Audit test coverage for: ${input:scope:Project or folder to audit, or leave blank for the whole solution}

This is an analysis task. Do not write tests — produce the backlog that test generation will work from.

1. **Measure the current state.** Run the suite and collect coverage:

```powershell
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj --collect:"XPlat Code Coverage"
```

Report the real numbers, and count how many "tests" assert nothing — `QuickTest.TempFolderTest` asserts nothing, so headline counts overstate the true coverage. FlaUI UI-category tests live in the separate `PlotDigitizer.WPF.Test` project and require an interactive desktop session to run.

2. **Map what exists to what is tested.** Enumerate public types in `PlotDigitizer.Core`, `PlotDigitizer.WPF`, `PlotDigitizer.CLI`, and `PlotDigitizer.Web`, and mark each as covered, partially covered, or untested. Remember that `ModelTests.cs` is excluded from compilation via `<Compile Remove>`, so anything it appears to cover is not actually running.

3. **Rank by risk, not by line count.** Weight each gap by:
   - Blast radius — Core is shared by three frontends.
   - Silent-failure potential — stale cached data and skipped `default` values produce wrong numbers rather than exceptions.
   - Subtlety — reversed `Max`/`Min` arguments, redo-branch truncation, scope disposal.
   - Change frequency, from `git log`.

4. **Assign the right test level to each gap**, and justify it: unit for single-class logic, integration for DI composition and cross-node invalidation, end-to-end for the whole pipeline, UI only for behaviour that cannot be reached below the window.

5. **Produce a prioritized backlog.** For each item give the target type, the level, the specific behaviours to assert, and the prompt to run (`/generate-unit-tests`, `/generate-integration-tests`, `/generate-e2e-tests`, `/generate-ui-tests`). Record it with the #tool:todos so the work can be executed incrementally.

6. **Flag defects separately from gaps.** Anything you find that is already broken — an unregistered service, unreachable code, a stale automation id — belongs in its own list, because a test would just pin the bug in place.

Keep the output to a ranked table plus the defect list. Do not create a Markdown report file in the repository unless asked.
