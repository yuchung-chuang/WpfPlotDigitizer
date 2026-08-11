---
name: generate-e2e-tests
description: Generate end-to-end MSTest coverage for the full PlotDigitizer digitization pipeline, from chart image to exported data.
argument-hint: "[surface: core pipeline, CLI, or web]"
agent: e2e-test-generator
---

Generate end-to-end tests for: ${input:target:Surface to cover - Core pipeline, CLI process, or web request pipeline}

Work through this sequence and do the work yourself.

1. **Pick a fixture** from `PlotDigitizer.Test/Assets/`. Only add a new image if the existing ones cannot express the case — and if you do, register it with `CopyToOutputDirectory` in the test `csproj`, or it will never reach the output folder.

2. **Drive the whole pipeline with nothing stubbed.** For the Core surface, compose the real container, assign `InputImage`, load a fully populated `Setting`, and read `Model.Data` — reading is what triggers the lazy computation. `ModelFacadeTests.ModelTest` is the reference example.

3. **Assert on properties that survive refactoring:**
   - Point count, and stability across repeated runs on the same input.
   - Data falling within the configured `AxisLimit`.
   - Ordering or monotonicity where the chart guarantees it.
   - Landmark points compared with `MathHelpers.ApproxEqual`, never exact `double` equality.

   Pin an exact count only for a small, deliberately chosen fixture. If an assertion needs a wide tolerance to pass, report that rather than widening it until it proves nothing.

4. **Cover the variations that matter:** `DataType.Discrete` and `DataType.Continuous`, and a log-scale axis through `AxisLogBase`. Give every sample `Setting` an explicit value for every field, because `Setting.Load` ignores `default` values.

5. **For the CLI surface**, run the built executable with `-i`, `-s`, and `-o`, then assert on the output file: the `X`/`Y` header, comma separator for `.csv` and tab for `.txt`, and the row count. **Do not drive the CLI down its failure path** — its error handler calls `Console.ReadKey()` and the test will hang.

6. **For the web surface**, note that `Microsoft.AspNetCore.Mvc.Testing` is not referenced. Propose that dependency change and wait for approval before adding it.

7. **Place tests** in `PlotDigitizer.Test/EndToEnd/` with `[TestCategory("EndToEnd")]`, clean up temporary files and images in `[TestCleanup]`, then run:

```powershell
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory=EndToEnd"
```

Report the fixtures used, the assertions chosen and why they are robust, the commands you ran with pass/fail counts, and the runtime.
