---
name: updatable-node
description: Add or modify a node in the PlotDigitizer lazy dependency graph that backs Model and Setting, wiring dependencies, DI registration, facade properties, and invalidation correctly. Use when adding derived digitization state or debugging stale or missing recomputation.
---

# Working with the PlotDigitizer dependency graph

`Model` and `Setting` are facades. The real state lives in `UpdatableNode` instances that recompute lazily and invalidate downstream when anything upstream changes. Use this skill when adding derived state or when a value is stale, missing, or recomputing too often.

## How it works

- `UpdatableNode<TData>.Data` setter assigns and calls `OnUpdated()`, raising `Updated`.
- `DependsOn(node)` subscribes to that node's `Updated` **and** `Outdated` events; either one marks this node outdated and re-broadcasts, so invalidation cascades transitively.
- `GetUpdatedData()` calls `CheckUpdate()`, which runs `Update()` only when `IsUpdated` is false.
- `IsAllDependenciesUpdated()` calls `CheckUpdate()` on each dependency, which is what pulls the whole upstream chain into a computed state on demand.
- `UpdatableModel`/`UpdatableSetting` relay each node's `Updated` to `PropertyChanged` and `Outdated` to `PropertyOutdated`.

There is no explicit "run the pipeline" call anywhere. Reading `model.Data` is what performs the digitization.

## Current graph

```
InputImage ─┬─> CroppedImage ──> FilteredImage ──> EdittedImage ─┬─> DataPoints ──> Data
            │        ^                 ^                         │                  ^
AxisLocation┘        │        FilterMin/FilterMax                │      AxisLimit ──┤
     │               │                                    DataType                  │
     └──> AxisTextBox ──> AxisLimit / AxisTitle / AxisLogBase        AxisLogBase ────┘
```

`InputImageNode` overrides `IsUpdated => Data != null`, so it is the only source node — it never computes, it just reports whether an image was supplied.

## Adding a node

1. **Create the node** in `Models/UpdatableModelNodes/` or `Models/UpdatableSettingNodes/`:

```csharp
public class MyDerivedNode : UpdatableNode<MyData>
{
    private readonly CroppedImageNode croppedImage;
    private readonly IImageService imageService;

    public MyDerivedNode(CroppedImageNode croppedImage, IImageService imageService)
    {
        this.croppedImage = croppedImage;
        this.imageService = imageService;
        DependsOn(croppedImage);            // one call per upstream node
    }

    protected override void Update()
    {
        if (!IsAllDependenciesUpdated())    // always first
            return;
        if (croppedImage.Data is null)      // guard null upstream data
            return;
        Data = imageService.DoSomething(croppedImage.Data);   // setter raises Updated
    }
}
```

2. **Register it** in `ServiceExtensions.AddModel()` as a singleton, alongside the existing nodes.

3. **Expose it** on the facade with an `override` property, and relay its events:

```csharp
public override MyData MyDerived
{
    get => myDerived.GetUpdatedData();
    set => myDerived.Data = value;
}
// in the constructor:
RelayEvents(myDerived, nameof(MyDerived));
```

Add the matching `virtual` property to the base `Model` or `Setting` so all frontends see it.

4. **Test the invalidation**, not just the computation. Change an upstream value and assert the derived value is recomputed; change an unrelated value and assert it is not.

## Rules

- `DependsOn` must list **every** input. A missing edge produces stale results that only appear after a specific edit order — the hardest class of bug in this codebase.
- Never cache derived data in a plain field or property outside a node.
- `Update()` must be safe to call repeatedly and must return early rather than throw when upstream data is absent.
- Do not call `OnUpdated()` manually after assigning `Data`; the setter already does.
- Keep nodes singletons. Two instances of the same node mean two disconnected graphs.

## Debugging

| Symptom | Likely cause |
| --- | --- |
| Value never refreshes after an upstream edit | Missing `DependsOn` for that input |
| Value recomputes constantly | A dependency raises `Updated` on every read, usually because `Update()` assigns `Data` unconditionally |
| `NullReferenceException` inside `Update()` | Missing null guard on upstream `Data` |
| UI does not refresh though data is correct | Node not passed to `RelayEvents`, or no facade property |
| Undo history vanishes unexpectedly | `PageService` disposes the Edit scope when `FilteredImage` goes outdated — expected behaviour |
| Setting value silently ignored on load | `Setting.Load` skips properties whose value is `default` |
