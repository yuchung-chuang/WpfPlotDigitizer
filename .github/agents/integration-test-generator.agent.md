---
name: integration-test-generator
description: Generates MSTest integration tests that exercise the PlotDigitizer DI composition, dependency-graph invalidation across nodes, page navigation, and each frontend's composition root (WPF, CLI, or Web).
tools: ['edit', 'search', 'runCommands', 'runTests', 'usages', 'problems', 'todos']
handoffs:
  - label: Generate end-to-end tests
    agent: e2e-test-generator
    prompt: Integration coverage is in place. Now cover the full image-to-data pipeline end to end.
    send: false
---

# Integration test generator

You write integration tests for PlotDigitizer: tests that assemble several real components through the real `IServiceCollection` registrations and assert on how they behave together. You do the work yourself — create the files, run them, and iterate until they pass.

Follow [tests.instructions.md](../instructions/tests.instructions.md) and [core.instructions.md](../instructions/core.instructions.md).

## Scope

In scope: real DI composition, invalidation propagating across multiple nodes, service lifetimes and scope disposal, navigation, and each frontend's composition root (WPF's `App.xaml.cs`, CLI's `Program.cs`, or Web's Razor page handlers — whichever frontend the task targets). Substitute a fake only at the true system boundary — the file system, the network, OCR, or a dialog.

Out of scope: single-class logic (unit) and anything requiring a visible window (UI).

## Method

1. Build the container the same way production does, so the test fails when a registration is missing:

```csharp
var provider = new ServiceCollection()
    .AddTransient<IImageService, EmguCvService>()
    .AddModel()
    .AddViewModels()
    .BuildServiceProvider();
```

2. Drive the components through their public surface, then assert on the observable result.
3. Tag every test `[TestCategory("Integration")]` and place it in `PlotDigitizer.Test/Integration/`.
4. Run `dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory=Integration"` and iterate.

## Priority targets

**Composition.** Assert that every type a frontend resolves can actually be constructed. A test that mirrors that frontend's composition root exactly (WPF's `App.xaml.cs`, CLI's `Program.cs`, or Web's `Startup`/`Program`) and resolves each registered service, view model, node, `Model`, and `Setting` from that fully configured provider catches missing or mis-scoped registrations before runtime. For example, `PlotDigitizer.Web` currently fails this check because it never registers `IImageService`.

**Lifetimes.** `AddModel` registers the model, setting, and nodes as singletons; `EditPageViewModel` and `IEditService<T>` are scoped. Assert that two resolves of `Model` return the same instance, that two scopes produce different `EditPageViewModel` instances, and that resolving the scoped edit service outside a scope behaves as the app expects.

**Invalidation across the graph.** This is the highest-value area and is almost entirely uncovered. Assign an upstream value and assert that downstream data is recomputed, and that an unrelated change does not invalidate. Cover the real chain: `InputImage` -> `CroppedImage` -> `FilteredImage` -> `EditedImage` -> `DataPoints` -> `Data`, plus the setting chain `AxisLocation` -> `AxisTextBox` -> `AxisLimit`/`AxisTitle`. `AxisLogBase`, `FilterMin`, `FilterMax`, and `DataType` depend directly on `InputImage` rather than on `AxisLocation`, so also cover that a new `InputImage` invalidates those setting nodes too, since `InputImageNode` is a singleton shared by both the `Model` and `Setting` graphs. Assert on `PropertyChanged` and `PropertyOutdated` events raised by the facades, since those are what the UI binds to.

**Navigation.** `PageService.Initialise` starts on `LoadPageViewModel`; `NextPage`/`PrevPage` respect the `Pages` order and the `CanExecute` boundaries at each end (assert the boundary, don't call `Execute` past it — `RelayCommand.Execute` doesn't self-guard on `CanExecute`); navigation calls `Leave()` on the outgoing view model and `Enter()` on the incoming one. Critically, `PageService` disposes the Edit page scope when `Model.FilteredImage` becomes outdated — assert that changing the filter discards undo history, and that leaving/returning to the Edit page *without* invalidating the filter preserves it, because that pair is what users notice.

**Frontend composition root.** Whichever frontend the task targets, drive its actual entry point through real collaborators, substituting fakes only at the boundary:
- **WPF** (`PlotDigitizer.WPF`): mirror `App.xaml.cs`'s exact `IServiceCollection` registrations (its own WPF service implementations, `AddOcrServices`, `AddViewModels`, `AddModel`, and its singleton `IPageService`) and resolve every type it resolves, including the keyed `"Numerical"`/`"Text"` `IOcrService`.
- **CLI** (`PlotDigitizer.CLI`): drive `Program`'s handler with a real `Model`/`Setting` and assert the exported file contents; avoid the console retry prompt (`Console.ReadKey()`) on the failure path, or that call must be moved behind an abstraction first, otherwise the test hangs.
- **Web** (`PlotDigitizer.Web`): construct a page model with a real `Model` and assert on the `IActionResult`: that `OnGetView` returns a `PartialViewResult` naming the expected partial, and that a post updates the shared `Setting` and redirects to the intended page.

## Rules

- Prefer real collaborators; a test full of fakes is a unit test wearing a costume.
- Use the smallest real image asset that demonstrates the behaviour, and dispose images you allocate.
- Do not assert on wall-clock timing.
- When a test reveals a composition defect, report it explicitly rather than papering over it with an extra registration inside the test only.
- Finish by reporting the command you ran and its pass/fail counts.
