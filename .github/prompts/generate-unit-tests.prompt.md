---
name: generate-unit-tests
description: Generate MSTest unit tests for a PlotDigitizer.Core class using hand-rolled fakes.
argument-hint: "[class or file to cover, e.g. EditService]"
agent: unit-test-generator
---

Generate unit tests for: ${input:target:Class or file to cover, e.g. PlotDigitizer.Core/Services/EditService.cs}

Work through this sequence and do the work yourself — do not just propose a plan.

1. **Read the target and its collaborators.** Use #tool:usages to find every call site so the tests reflect how the type is actually used. Pin down what the code *does*, not what its name suggests it does.

2. **List the behaviours you will cover** before writing any test: each guard clause, early return, boundary, null path, and state transition. Call out any behaviour that looks like a bug — cover it as-is and report it.

3. **Write the tests** in `PlotDigitizer.Test/`, mirroring the source folder. Requirements:
   - `[TestCategory("Unit")]` on every test.
   - Names in `Method_Scenario_ExpectedOutcome` form.
   - `[DataTestMethod]` + `[DataRow]` for value tables (preferred over `[DataRow]` on a plain `[TestMethod]`, which also runs but hides the intent).
   - No mocking library. Hand-write fakes implementing the Core interface in `PlotDigitizer.Test/Fakes/`.
   - Pass `null` for `ILogger<T>`.
   - Load image assets as `new Image<Rgba, byte>("Assets/<name>.png")` rather than through WPF pack URIs.

4. **Run them and iterate until green:**

```powershell
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "ClassName~<YourTestClass>"
```

5. **Confirm you did not break the suite:**

```powershell
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory!=UI"
```

Report the behaviours covered, the commands you ran with pass/fail counts, and any defect you found. Do not change production code to make a test pass unless the change is the point of the task — report the defect instead.
