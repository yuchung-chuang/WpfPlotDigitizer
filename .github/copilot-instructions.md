# PlotDigitizer repository instructions

## Toolchain and commands

- Use the .NET SDK selected by [global.json](../global.json): `8.0.0` with roll-forward enabled. The WPF application and test project target `net8.0-windows`; work from Windows.
- The solution file references `WpfAppTemplate1\WpfAppTemplate1.csproj`, which is not present. Build the individual project you are changing rather than `PlotDigitizer.sln`.

```powershell
# Restore and build the main WPF application
dotnet restore .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj
dotnet build .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj --no-restore

# Run the complete MSTest project
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj

# Run one test by fully qualified name
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "FullyQualifiedName=PlotDigitizer.Core.Tests.QuickTest.TempFolderTest"
```

`PlotDigitizer.Test` uses MSTest. Its FlaUI tests launch `PlotDigitizer.exe` and automate the WPF window, so run the complete suite only in an interactive Windows desktop session. Test image assets are copied to the test output directory and should be used for image-processing coverage.

Tag new tests with `[TestCategory("Unit")]`, `"Integration"`, `"EndToEnd"`, or `"UI"` so the fast loop can skip desktop automation with `--filter "TestCategory!=UI"`.

## Testing and web-port assets

Coverage is currently thin: only `EmguCvService`, `Setting.Load`, and one model-facade path are meaningfully tested, and `CroppedImageNodeTests` is a failing `Assert.Fail()` scaffold. Use these customizations rather than inventing an approach:

- Agents: [unit-test-generator](agents/unit-test-generator.agent.md), [integration-test-generator](agents/integration-test-generator.agent.md), [e2e-test-generator](agents/e2e-test-generator.agent.md), [ui-test-generator](agents/ui-test-generator.agent.md), [web-port-architect](agents/web-port-architect.agent.md).
- Prompts: `/generate-unit-tests`, `/generate-integration-tests`, `/generate-e2e-tests`, `/generate-ui-tests`, `/test-coverage-audit`, `/port-wpf-page-to-web`.
- Skills: [mstest-authoring](skills/mstest-authoring/SKILL.md), [updatable-node](skills/updatable-node/SKILL.md), [wpf-to-web-port](skills/wpf-to-web-port/SKILL.md).
- Scoped instructions live in [.github/instructions](instructions) and apply automatically per project.

There is no mocking library in the test project; write hand-rolled fakes for the Core interfaces.

## Architecture

- `PlotDigitizer.Core` is the shared image-digitizing domain layer. It contains the image/OCR services, models, MVVM view models, commands, geometry helpers, and service interfaces. Images are Emgu CV `Image<Rgba, byte>` values.
- `Model` and `Setting` expose application state through a lazy dependency graph. The normal data path is `InputImage` -> cropped axis region -> color-filtered image -> editable image -> discrete/continuous pixel points -> axis-transformed data. `UpdatableNode` dependencies invalidate downstream nodes; reading a model/setting property calls `GetUpdatedData()` and computes it only when stale.
- `UpdatableModel` and `UpdatableSetting` relay node updates and invalidations as property notifications. `ServiceExtensions.AddModel()` registers the model, setting, and every graph node as singletons.
- `PlotDigitizer.WPF` is the main client. `App.xaml.cs` composes Core with WPF implementations of service interfaces, OCR configuration from `appsettings.json`, and logging. `PageService` owns the fixed workflow `Load -> Axis -> Range -> Filter -> Edit -> Data`; the Edit view model and its undo/redo state are scoped and recreated when `FilteredImage` becomes outdated.
- `PlotDigitizer.CLI` is a separate command-line frontend that reads an image and serialized setting, then exports CSV or tab-delimited data. `PlotDigitizer.Web` is a Razor Pages frontend that is still under development.

## Repository-specific conventions

- Resolve shared state and services through the existing `IServiceCollection` registrations. Preserve the declared lifetimes: model/setting/nodes are singletons, most page view models are transient, and `EditPageViewModel` plus `IEditService` are scoped.
- When adding a derived model or setting value, represent it as an `UpdatableNode<T>`, declare every upstream dependency with `DependsOn`, call `OnUpdated()` after recomputing, register it in `AddModel()`, and relay its events through `UpdatableModel` or `UpdatableSetting`. Directly caching derived data bypasses invalidation and produces stale digitization results.
- Keep the `PageService.Pages` order synchronized with WPF navigation. Page transitions call `Leave()` on the old view model and `Enter()` on the new one; put page-specific subscriptions and cleanup in those lifecycle methods.
- Keep WPF-specific APIs in `PlotDigitizer.WPF` service implementations and depend on Core interfaces (`IImageService`, `IFileDialogService`, `IClipboardService`, and similar) from shared view models and graph nodes.
- Follow [.editorconfig](../.editorconfig): namespaces do not need to match folders, braces are preferred for multiline blocks, and expression-bodied methods are allowed only when they fit on one line.
