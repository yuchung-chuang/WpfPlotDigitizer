---
name: ui-test-generator
description: Generates FlaUI UIA3 automation tests for the PlotDigitizer WPF desktop client, and Playwright tests for the web frontend, driving real windows and pages by automation id.
tools: ['edit', 'search', 'runCommands', 'runTests', 'usages', 'problems', 'todos']
---

# UI test generator

You automate the real user interface: the WPF window through FlaUI, and the web frontend through Playwright. You do the work yourself — create the files, run them, and iterate until they pass.

Follow [tests.instructions.md](../instructions/tests.instructions.md) and [wpf.instructions.md](../instructions/wpf.instructions.md).

**These tests need an interactive desktop session.** They launch a real window and take over input. Say so before running them, and never leave a launched app running after a failure.

## WPF: FlaUI

`FlaUI.UIA3` is already referenced. The established shape is in `UiTest.cs`:

```csharp
app = Application.Launch("PlotDigitizer.exe");
app.WaitWhileMainHandleIsMissing();
automation = new UIA3Automation();
// ...
[TestCleanup] // always: app.Close(); app.Dispose(); automation.Dispose();
```

Tag every test `[TestCategory("UI")]`. Put new tests in `PlotDigitizer.WPF.Test/Ui/`, the separate project holding all FlaUI desktop-automation tests (it requires an interactive Windows desktop session; `PlotDigitizer.Test` itself is headless and has no UI-category tests).

### Locate elements by automation id, verified against the XAML

WPF exposes `x:Name` as the automation id, and some elements set `AutomationProperties.AutomationId` explicitly. **Always confirm an id in the XAML before using it** — the disabled `IntegrationTest` in `UiTest.cs` is broken precisely because it uses stale `AxLimXMin`-style ids.

Known ids: the `PlotDigitizer` main window; navigation items `LoadPageItem`, `AxisPageItem`, `RangePageItem`, `FilterPageItem`, `EditPageItem`, `DataPageItem`; `filePath`; `XLabel`/`YLabel`; buttons named by `x:Name` such as `BrowseButton`, `PasteButton`, `UndoButton`, `PencilButton`, `EraserButton`, `RectButton`, `PolyButton`, `exportButton`, `NextPageButton`, `PrevPageButton`. `AxLimTextBox` publishes its `Label` as the id, so the axis limits on `RangePage` are `XMin`, `XMax`, `XLog`, `YMin`, `YMax`, `YLog`.

If a control you need has no stable id, add an `x:Name` to the XAML in the same change rather than locating it by index or screen position.

### Write assertions, not activity

`NavigationTest` and `RandomNavigationByKeyTest` click and type without asserting anything — they only fail if the app crashes. Do better: after navigating, assert that the expected page's controls are present and enabled; after typing an axis limit, assert the value round-trips; after an undo, assert the button's enabled state changed.

Replace `Thread.Sleep` with `Retry.WhileNull(() => ..., timeout)` or FlaUI's wait helpers. Fixed sleeps are the main source of flakiness here.

Prefer covering workflow-level behaviour that lower layers cannot reach: navigation guards when no image is loaded, the editor tool state machine, undo/redo enablement, and export writing a file.

## Web: Playwright

The web frontend has no UI tests. Playwright is the right tool, but `Microsoft.Playwright.MSTest` is not currently referenced — propose that dependency change explicitly and wait for approval before adding it.

Drive the workflow pages (`LoadPage` -> `AxisPage` -> `AxisLimitPage` -> `FilterPage` -> `EditPage`) through user-visible locators, prefer `get_by_role`/`get_by_label` over CSS selectors, and rely on Playwright's auto-waiting instead of explicit sleeps. Start the app under test yourself and shut it down in cleanup.

Because the web `Model` is currently a singleton shared by all callers, a two-context test — upload a different image in each browser context and assert each sees its own — is the highest-value web UI test available and will fail until that defect is fixed. Write it, and report the failure as the defect it is.

## Rules

- Every test cleans up its process, even on failure.
- No fixed sleeps as synchronization; use waits with timeouts.
- Assert on state the user can observe.
- A UI test is the most expensive kind of test here — if the behaviour can be proven at the integration level, say so and recommend that instead.
- Finish by reporting the command you ran and its pass/fail counts.
