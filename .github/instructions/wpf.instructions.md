---
description: Conventions for the PlotDigitizer WPF desktop client, including automation-id requirements.
applyTo: "PlotDigitizer.WPF/**"
---

# PlotDigitizer.WPF conventions

This project is the composition root for the desktop app. `App.xaml.cs` builds the `ServiceCollection`, registers the WPF implementations of the Core service interfaces, configures the two keyed OCR services from `appsettings.json`, and adds the debug/file logging providers.

## Keep logic out of this project

Business logic belongs in `PlotDigitizer.Core` so the CLI and Web frontends inherit it. This project should contain only WPF concerns: views, controls, value converters, validation rules, behaviours, and the service implementations that wrap WPF APIs (`ClipboardService`, `FileDialogService`, `MessageBoxService`, `WindowService`, `AwaitTaskService`). Code-behind should bind and delegate to a Core view model rather than compute results.

When you add a service that Core needs, define the interface in `PlotDigitizer.Core/Services/Interfaces/` and register the WPF implementation in `App.OnStartup`.

## PropertyChanged.Fody

`PropertyChanged.Fody` weaves `INotifyPropertyChanged` into properties at build time. Do not hand-write raise-on-set boilerplate for woven types, and do not add a `static` `On<PropertyName>Changed` method — Fody emits a build warning for that pattern.

## Editor and selection state machines

`Controls/Editor/States` and `Controls/SelectionBox` are explicit state machines (`EditorMode`, `EdittingState`, `SelectionBoxState`). Add a new behaviour by adding a state class and its transitions rather than by adding conditionals to the control. Keep transition logic in the state classes so it stays reviewable.

## Automation ids are a test contract

The FlaUI tests in [PlotDigitizer.Core.Test](../../PlotDigitizer.Core.Test) locate elements by automation id, so ids are public API for tests. WPF exposes `x:Name` as the automation id automatically, and some elements set `AutomationProperties.AutomationId` explicitly.

Currently addressable ids include the `PlotDigitizer` main window; the navigation items `LoadPageItem`, `AxisPageItem`, `RangePageItem`, `FilterPageItem`, `EditPageItem`, `DataPageItem`; `filePath`; `XLabel`/`YLabel`; and the editor/toolbar buttons named with `x:Name` such as `BrowseButton`, `PasteButton`, `UndoButton`, `PencilButton`, `EraserButton`, `RectButton`, `PolyButton`, `exportButton`, `NextPageButton`, and `PrevPageButton`.

`AxLimTextBox` binds its automation id to its `Label` property, so the axis-limit inputs on `RangePage` are addressed as `XMin`, `XMax`, `XLog`, `YMin`, `YMax`, and `YLog` — **not** `AxLim`-prefixed names. The disabled `IntegrationTest` in `UiTest.cs` still uses the stale `AxLimXMin`-style ids and must be updated before it is re-enabled.

When you rename or remove an `x:Name` or `Label` that a UI test depends on, update the test in the same change. When you add a control that a UI test will need to reach, give it a stable `x:Name`.

## Running the app

```powershell
dotnet run --project .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj
```

Setting `"RunTest": true` in `appsettings.json` makes `App.OnStartup` run a scripted smoke path after the main window opens. Leave it `false` in committed code.
