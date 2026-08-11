---
description: Conventions and known blockers for the PlotDigitizer ASP.NET Core Razor Pages web frontend.
applyTo: "PlotDigitizer.Web/**"
---

# PlotDigitizer.Web conventions

The in-progress web port of the WPF app. It reuses `PlotDigitizer.Core` for all digitization and mirrors the desktop workflow as Razor Pages: `LoadPage` -> `AxisPage` -> `AxisLimitPage` -> `FilterPage` -> `EditPage`.

## Fix these before adding features

Both are real defects in the current composition root, not stylistic preferences.

**1. `IImageService` is never registered.** `Startup.ConfigureServices` calls `AddModel()`, but the nodes it registers (`CroppedImageNode`, `FilteredImageNode`, `DataPointsNode`, `DataNode`) and `AxisPageModel` all require `IImageService`. Resolving any of them throws at request time. The desktop app registers `EmguCvService`; the web app must too:

```csharp
services.AddTransient<IImageService, EmguCvService>();
```

**2. Digitization state is a global singleton.** `services.AddModel().AddSingleton<Models.Model>()` gives every visitor the *same* image, axis, filter, and data. This is correct for a single-user desktop app and wrong for a server. Before this is usable by more than one person, the model, setting, and every graph node must become per-user state.

Prefer scoping the whole graph to the user session rather than to the request, because the workflow spans several requests:

- Register the nodes, `Setting`, and `Model` as `AddScoped`, and resolve them per request from state rehydrated for the caller.
- Or keep a per-session container/state object keyed by session id, and evict it on a timer.

Do not solve this by adding `static` fields, and do not carry `Image<Rgba, byte>` instances in `TempData` or cookies.

## Reuse Core, do not reimplement it

Any digitization behaviour must come from Core. If a page needs logic that only exists in a WPF view model, move that logic into Core first so both frontends share it, then call it from the page model. `PlotDigitizer.Web/Models/Model.cs` is only an adapter: it extends `UpdatableModel` and exposes `...ImageSource` string properties for rendering.

## Page model patterns

Page models take `Model` (and any Core service) via constructor injection and expose it as a property for the view. Partial-update handlers follow the `OnGet<Name>` convention and return `Partial("_XxxPageView", model)`; the `_XxxPageView.cshtml` partials exist so the client can refresh a single panel without a full page load. Keep new AJAX handlers on that pattern.

Watch for unreachable code left from prototyping — `FilterPageModel.OnPost` currently has a `return RedirectToPage("EditPage")` after `return Page()`. Decide the intended navigation instead of leaving both.

## Images over HTTP

`ImageCaster.ToImgSrc` base64-encodes a PNG into a `data:` URI and inlines it in the HTML. That is fine for prototyping and expensive for real images: it inflates the payload by roughly a third and defeats browser and CDN caching. When you touch image delivery, prefer a handler that returns `File(bytes, "image/png")` from a dedicated endpoint and reference it with a normal `<img src>`.

Validate uploads in `ToImageAsync`: it currently assumes the stream decodes to a `Bitmap` and will throw on a non-image upload. Enforce a content-type allowlist and a maximum request size.

## Platform and dependencies

The project targets `net8.0-windows` and references `Emgu.CV.runtime.windows`, so it only runs on Windows. Deploying to Linux containers requires swapping in the corresponding Linux Emgu runtime package and retargeting to `net8.0`.

Several packages are still on 3.1.x (`Microsoft.AspNetCore.Mvc.Razor.RuntimeCompilation`, `Microsoft.VisualStudio.Web.CodeGeneration.Design`) while the app targets .NET 8. Align them with the 8.x line when you next touch the project file. Runtime compilation and `Westwind.AspNetCore.LiveReload` are development conveniences — keep them out of the production path.

```powershell
dotnet run --project .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
```
