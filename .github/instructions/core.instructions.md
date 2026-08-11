---
description: Conventions and testability rules for the shared PlotDigitizer.Core domain layer.
applyTo: "PlotDigitizer.Core/**"
---

# PlotDigitizer.Core conventions

Core is the only project shared by the WPF, CLI, and Web frontends. It targets `netstandard2.1`, so it must not use .NET 8-only APIs and must not reference WPF, ASP.NET, or `System.Windows.*` types. `PointD` and `RectangleD` exist precisely because Core cannot depend on `System.Windows.Point` and `System.Windows.Rect`.

## The dependency graph is the architecture

`Model` and `Setting` are thin facades. The real state lives in `UpdatableNode` instances registered by `ServiceExtensions.AddModel()`. Reading a facade property calls `GetUpdatedData()`, which recomputes only when the node is stale; writing a facade property assigns `Data` and raises `Updated`, which invalidates every downstream node.

When adding derived state:

1. Create an `UpdatableNode<T>` subclass, injecting the upstream nodes it needs.
2. Call `DependsOn(...)` in the constructor for every upstream node.
3. Override `Update()`, return early when `!IsAllDependenciesUpdated()`, guard null upstream data, then assign `Data` (the setter calls `OnUpdated()` for you).
4. Register the node in `AddModel()`.
5. Expose it via an `override` property on `UpdatableModel`/`UpdatableSetting` and relay its events with `RelayEvents`.

Never cache derived values in a plain field or property. That bypasses invalidation and produces stale digitization output that only reproduces after a specific edit sequence.

## Dependency injection

`AddModel()` registers the model, setting, and every node as **singletons**; `AddViewModels()` registers page view models as transient except `EditPageViewModel` and `IEditService<T>`, which are **scoped** so undo history is discarded with the scope. Preserve these lifetimes. Frontends supply their own `IImageService`, `IOcrService`, and dialog/clipboard implementations, so Core code must depend on the interfaces in `Services/Interfaces/`, never on a concrete frontend type.

## View models

Derive from `ViewModelBase`. Put page-entry work in `Enter()` and persistence of user edits back to `Setting` in `Leave()`; `PageService` calls these on navigation. Keep a parameterless constructor that wires up `RelayCommand` instances and have the DI constructor chain to it with `: this()` — this is what makes view models unit-testable without a container. Treat `ILogger<T>` as optional and always call it as `logger?.Log...`.

## Behaviour worth preserving (and testing)

These are load-bearing and subtle; changing them silently breaks callers.

- `Setting.Load` copies only properties whose value is not `default`. A source `Setting` with `DataType.Continuous` (enum value 0) or a zeroed `RectangleD` leaves the destination unchanged.
- `EditService.Edit` truncates the redo branch: everything after `Index` is dropped before the new entry is appended.
- `EditService.CurrentObject`/`CurrentTag` throw when the service has not been initialised; `IsInitialised` is the guard.
- `MathHelpers.Clamp` and `IsIn` take `Max` before `Min` and swap them when they arrive reversed.
- `RelayCommand<TParam>.Execute` silently does nothing when the parameter is not a `TParam`.
- `PageService` disposes the Edit page scope when `Model.FilteredImage` becomes outdated, which is what discards undo history when the filter changes.

Add or update a test in [PlotDigitizer.Test](../../PlotDigitizer.Test) whenever you change one of these.
