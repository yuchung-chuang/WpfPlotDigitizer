---
name: mstest-authoring
description: Write and run MSTest tests for the PlotDigitizer solution, covering hand-rolled fakes, Emgu CV image assets, test categories, and targeted dotnet test filters. Use when adding or debugging tests in PlotDigitizer.Core.Test.
---

# Authoring MSTest tests for PlotDigitizer

Use this when adding, running, or debugging tests in `PlotDigitizer.Core.Test`.

## Before you start

- MSTest 2.1, `net8.0-windows`, with `coverlet.collector`.
- **No mocking library is available.** Hand-roll fakes; see [fake-service-template.cs](./fake-service-template.cs).
- `PlotDigitizer.Core` targets `netstandard2.1`, so Core code cannot use .NET 8-only APIs.
- `ModelTests.cs` is excluded from compilation by `<Compile Remove>`. Editing it changes nothing.
- Emgu CV natives arrive transitively via the `PlotDigitizer.WPF` project reference. Do not remove it.

## Procedure

1. **Locate the behaviour.** Read the production code and its call sites. Tests must pin down what the code does, including quirks — several behaviours in this codebase are deliberate and surprising (see below).

2. **Choose the level and category.** Every test in `PlotDigitizer.Core.Test` gets exactly one:

   | Category | Use for | Folder |
   | --- | --- | --- |
   | `Unit` | One class, in memory, fakes at the boundary | mirror of the source folder |
   | `Integration` | Real DI container, multiple nodes, navigation | `Integration/` |
   | `EndToEnd` | Whole pipeline: image in, data out | `EndToEnd/` |

   FlaUI window automation (`UI` category) lives in the separate `PlotDigitizer.WPF.Test` project instead, which requires an interactive desktop session — see that project's own instructions rather than this skill.

3. **Write the test.** Start from [test-class-template.cs](./test-class-template.cs). Name tests `Method_Scenario_ExpectedOutcome`. Use `[DataTestMethod]` + `[DataRow]` for tables — the adapter also honours `[DataRow]` on a plain `[TestMethod]`, but `[DataTestMethod]` states the intent and matches the existing `ImageServiceTests`.

4. **Run the narrowest useful filter, then widen:**

```powershell
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj --filter "FullyQualifiedName=PlotDigitizer.Core.Tests.QuickTest.TempFolderTest"
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj --filter "ClassName~EditServiceTests"
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj --filter "TestCategory=Unit"
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj --collect:"XPlat Code Coverage"
```

`PlotDigitizer.Core.Test` has no UI-category tests to filter out — it's already headless, so the full run is the standard local loop.

## Loading image assets

Prefer the direct form — no WPF dependency, no STA requirement:

```csharp
using var image = new Image<Rgba, byte>("Assets/data.png");
```

Use the pack-URI form only when the test genuinely exercises WPF types, and keep the guard, or the URI scheme is unregistered and the test throws:

```csharp
if (!UriParser.IsKnownScheme("pack"))
    new System.Windows.Application();
var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/data.png")).ToBitmap().ToImage<Rgba, byte>();
```

**A new asset only reaches the output directory once you add a `CopyToOutputDirectory` entry for it in `PlotDigitizer.Core.Test.csproj`.** `Image<Rgba, byte>` holds unmanaged memory — dispose images you allocate, especially inside data-driven loops.

## Dependencies in tests

Pass `null` for every `ILogger<T>`; production code calls `logger?.Log...`.

Construct view models directly — their DI constructor chains to a parameterless constructor that wires up the `RelayCommand` instances, so no container is needed for a unit test:

```csharp
var viewModel = new AxisPageViewModel(model, setting, fakeImageService, null);
```

Build a real container only for integration tests:

```csharp
var provider = new ServiceCollection()
    .AddTransient<IImageService, EmguCvService>()
    .AddModel()
    .AddViewModels()
    .BuildServiceProvider();
```

## Behaviours that are easy to get wrong

Assert current behaviour; if you believe it is a bug, report it rather than silently changing production code.

- `Setting.Load` copies only non-`default` properties, so `DataType.Continuous` (enum value 0) and zeroed rectangles do not overwrite the destination.
- `EditService.Edit` discards the redo branch after `Index` before appending.
- `EditService.CurrentObject` throws before `Initialise`; `IsInitialised` is the guard.
- `MathHelpers.Clamp`/`IsIn` take `Max` before `Min` and swap reversed arguments.
- `RelayCommand<TParam>.Execute` does nothing when the parameter is not a `TParam`.
- `PageService` disposes the Edit scope when `Model.FilteredImage` goes outdated, discarding undo history.
- Reading a `Model`/`Setting` property triggers lazy recomputation; the pipeline has no explicit "run" call.

## Do not

- Add a mocking package without explicit approval.
- Write a test with no assertion. `QuickTest.TempFolderTest` is a placeholder, not a pattern.
- Use `Thread.Sleep` for synchronization in UI tests; use `Retry.WhileNull` with a timeout.
- Compare `double` values with exact equality; use `MathHelpers.ApproxEqual`.
