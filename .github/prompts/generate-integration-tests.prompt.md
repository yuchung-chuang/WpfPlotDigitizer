---
name: generate-integration-tests
description: Generate MSTest integration tests for PlotDigitizer DI composition, graph invalidation, navigation, or Razor page models.
argument-hint: "[area to cover, e.g. graph invalidation or PageService]"
agent: integration-test-generator
---

Generate integration tests for: ${input:target:Area to cover, e.g. dependency-graph invalidation, PageService navigation, or a Razor page model}

Work through this sequence and do the work yourself.

1. **Assemble the container the way production does**, so a missing registration fails the test:

```csharp
var provider = new ServiceCollection()
    .AddTransient<IImageService, EmguCvService>()
    .AddModel()
    .AddViewModels()
    .BuildServiceProvider();
```

2. **Exercise real collaborators.** Substitute a fake only at a true boundary — file system, network, OCR, or a dialog. If most participants are fakes, you are writing a unit test; switch approach.

3. **Cover the behaviour that only appears when components are combined:**
   - Every registered node, view model, `Model`, and `Setting` resolves successfully.
   - Lifetimes hold: `Model` is a singleton across resolves; `EditPageViewModel` differs across scopes.
   - Invalidation propagates along `InputImage` -> `CroppedImage` -> `FilteredImage` -> `EdittedImage` -> `DataPoints` -> `Data`, and along `AxisLocation` -> `AxisTextBox` -> `AxisLimit`.
   - An unrelated change does *not* invalidate a node.
   - `PropertyChanged` and `PropertyOutdated` fire on the facades as the UI expects.
   - `PageService` navigation calls `Leave()` then `Enter()`, respects the `Pages` order and end boundaries, and disposes the Edit scope when `FilteredImage` goes outdated.
   - Web page model handlers return the expected `IActionResult` and mutate the shared `Setting`.

4. **Place tests** in `PlotDigitizer.Test/Integration/` with `[TestCategory("Integration")]`, then run and iterate:

```powershell
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory=Integration"
```

5. **Confirm the wider suite still passes:**

```powershell
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory!=UI"
```

Report the commands you ran with pass/fail counts. If a test exposes a composition defect — for example `PlotDigitizer.Web` never registering `IImageService` — report it plainly instead of hiding it behind an extra registration in test setup.
