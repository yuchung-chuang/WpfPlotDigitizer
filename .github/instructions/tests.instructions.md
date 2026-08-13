---
description: Testing conventions for the PlotDigitizer MSTest project.
applyTo: "PlotDigitizer.Test/**"
---

# PlotDigitizer.Test conventions

## Framework and available packages

MSTest 2.1 (`Microsoft.VisualStudio.TestTools.UnitTesting`) with `Microsoft.NET.Test.Sdk`, `FlaUI.UIA3`, and `coverlet.collector`. Target framework is `net8.0-windows`.

**There is no mocking library.** Do not write `Mock<T>`, `Substitute.For<T>`, or similar. Write hand-rolled fakes that implement the Core interface and place shared ones in `Fakes/`. The Core interfaces (`IImageService`, `IEditService<T>`, `IOcrService`, `IDownloadService`, `IFileDialogService`, `IMessageBoxService`, `IClipboardService`, `IAwaitTaskService`, `IWindowService`, `IPageService`) are small enough to implement directly. Only add a mocking package if the user explicitly asks for a dependency change.

Use `[TestClass]`, `[TestMethod]`, `[TestInitialize]`, `[TestCleanup]`, and `[DataTestMethod]` + `[DataRow]` for table-driven cases. `Assert.ThrowsException<T>` is the exception assertion. Prefer `[DataTestMethod]` for parameterized tests: the adapter also executes `[DataRow]` on a plain `[TestMethod]`, but `[DataTestMethod]` states the intent and matches the existing `ImageServiceTests`.

## Categorize every new test

Always add one category so the suite can be filtered. The desktop-automation tests cannot run headless, so this is what keeps the fast suite runnable.

```csharp
[TestMethod]
[TestCategory("Unit")]        // or "Integration", "EndToEnd", "UI"
public void Undo_AfterSingleEdit_RestoresPreviousObject() { }
```

```powershell
# Fast feedback loop: everything except desktop automation
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory!=UI"

# One category, one class, or one test
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory=Unit"
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "ClassName~EditServiceTests"
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "FullyQualifiedName=PlotDigitizer.Core.Tests.QuickTest.TempFolderTest"

# Coverage via the already-referenced coverlet collector
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --collect:"XPlat Code Coverage"
```

## Naming and layout

Name tests `Method_Scenario_ExpectedOutcome`. Mirror the source layout: `Services/`, `Models/UpdatableModelNodes/`, `Models/UpdatableSettingNodes/`, `ViewModels/`, `Utilities/`, plus `Integration/`, `EndToEnd/`, `Ui/`, and `Fakes/`. Keep the existing `PlotDigitizer.Core.Tests` namespace for non-UI tests and `PlotDigitizer.WPF.Tests.UI` for FlaUI tests.

## Loading image assets

Assets live in `Assets/` and are copied to the output directory by explicit `csproj` entries. **A new asset does not reach the output directory until you add a `CopyToOutputDirectory` entry for it in [PlotDigitizer.Test.csproj](../../PlotDigitizer.Test/PlotDigitizer.Test.csproj).**

Existing tests load images through WPF pack URIs, which forces every test to construct a `System.Windows.Application` first:

```csharp
if (!UriParser.IsKnownScheme("pack"))
    new System.Windows.Application();
var image = new BitmapImage(new Uri(@"pack://siteoforigin:,,,/Assets/data.png")).ToBitmap().ToImage<Rgba, byte>();
```

For new tests, prefer loading straight from the output directory so the test has no WPF or STA dependency:

```csharp
var image = new Image<Rgba, byte>("Assets/data.png");
```

Reserve the pack-URI form for tests that genuinely exercise WPF types.

`Image<Rgba, byte>` wraps unmanaged memory; dispose images you allocate inside loops.

## Known traps

- `ModelTests.cs` is excluded from compilation by `<Compile Remove="Models\ModelTests.cs" />`. Editing it has no effect. Put new model tests in a different file, or remove that exclusion deliberately and fix the resulting failures.
- The Emgu CV native binaries reach this project transitively through the `PlotDigitizer.WPF` project reference, which is the only source of `Emgu.CV.runtime.windows`. Do not drop that reference to "decouple" the tests.
- `PlotDigitizer.Core` targets `netstandard2.1`, so Core code cannot use .NET 8-only APIs even though the test project is `net8.0-windows`.
- Services and view models take `ILogger<T>` and call it as `logger?.Log...`, so pass `null` for the logger instead of building a logging stack.
- Every view model has a parameterless constructor that the DI constructor chains to with `: this()`. Construct view models directly with fakes; do not build a `ServiceProvider` for a unit test.
- A test that asserts nothing is not a test. The existing `TempFolderTest` and the `Assert.Fail()` node stub are placeholders, not patterns to copy.
