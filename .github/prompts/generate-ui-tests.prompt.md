---
name: generate-ui-tests
description: Generate FlaUI automation tests for the PlotDigitizer WPF window, or Playwright tests for the web frontend.
argument-hint: "[workflow to automate, e.g. load image then navigate to filter]"
agent: ui-test-generator
---

Generate UI tests for: ${input:target:Workflow to automate, e.g. loading an image and navigating to the filter page}

These tests launch a real application and take over keyboard and mouse. Confirm an interactive desktop session is available before running them, and make sure every launched process is closed afterwards even when a test fails.

Work through this sequence and do the work yourself.

1. **Verify every automation id against the XAML before using it.** WPF exposes `x:Name` as the automation id, and some elements set `AutomationProperties.AutomationId` explicitly. The disabled `IntegrationTest` in `UiTest.cs` is broken because it uses stale `AxLimXMin`-style ids; the real ids on `RangePage` are `XMin`, `XMax`, `XLog`, `YMin`, `YMax`, `YLog`. If a control you need has no stable id, add an `x:Name` to the XAML in the same change rather than locating it positionally.

2. **Follow the established FlaUI shape** from `PlotDigitizer.WPF.Test\Ui\UiTest.cs`: `Application.Launch("PlotDigitizer.exe")`, `WaitWhileMainHandleIsMissing()`, a `UIA3Automation` instance, and a `[TestCleanup]` that closes and disposes both.

3. **Assert on observable state — do not just click.** `NavigationTest` and `RandomNavigationByKeyTest` perform actions without assertions and only fail if the app crashes; do not copy that pattern. After navigating, assert the expected page's controls are present and enabled. After typing an axis limit, assert the value round-trips. After an undo, assert the button's enabled state changed.

4. **Never use `Thread.Sleep` for synchronization.** Use `Retry.WhileNull(() => ..., timeout)` or FlaUI's wait helpers. Fixed sleeps are the main source of flakiness in this suite.

5. **Prefer workflow behaviour that lower test layers cannot reach:** navigation guards with no image loaded, the editor tool state machine, undo/redo enablement, and export writing a file.

6. **Place tests** in `PlotDigitizer.WPF.Test\Ui\`, the separate project holding FlaUI desktop-automation tests, with `[TestCategory("UI")]` so they stay separable from the WPF-hosted unit tests in `PlotDigitizer.WPF.Test\Wpf\`, then run:

```powershell
dotnet build .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj
dotnet test .\PlotDigitizer.WPF.Test\PlotDigitizer.WPF.Test.csproj --filter "TestCategory=UI"
```

For the web frontend, use Playwright with role- and label-based locators and its auto-waiting. `Microsoft.Playwright.MSTest` is not currently referenced — propose that dependency change and wait for approval before adding it.

Report the commands you ran with pass/fail counts. If the behaviour could be proven more cheaply at the integration level, say so and recommend that instead of adding a UI test.
