---
name: wpf-to-web-port
description: Port PlotDigitizer desktop features from the WPF client to the ASP.NET Core Razor Pages web frontend, covering shared-logic extraction into Core, per-user state scoping, image delivery over HTTP, and WPF-to-web control equivalents. Use when building or reviewing the web version of the app.
---

# Porting PlotDigitizer from WPF to the web

Use this when moving a feature from `PlotDigitizer.WPF` to `PlotDigitizer.Web`, or when reviewing web code for desktop assumptions.

## The governing rule

Behaviour belongs in `PlotDigitizer.Core`; only presentation belongs in a frontend. Before writing any Razor code, check whether the logic lives in a WPF view model or code-behind. If it does, move it into Core first, keep WPF calling the relocated code, and then write the page model against it. Copying logic into a page model forks the two frontends and guarantees they drift.

Core targets `netstandard2.1` and cannot reference WPF or ASP.NET types. Logic using `System.Windows.Point`/`Rect` must be rewritten with Core's `PointD`/`RectangleD` before it can move — that is exactly why those types exist.

## Fix the composition root first

Two defects currently block anything built on the web project:

1. **`IImageService` is never registered.** `Startup.ConfigureServices` calls `AddModel()`, but `CroppedImageNode`, `FilteredImageNode`, `DataPointsNode`, `DataNode`, and `AxisPageModel` all require `IImageService`. Resolving any of them throws at request time.

```csharp
services.AddTransient<IImageService, EmguCvService>();
```

2. **Digitization state is one global singleton.** `AddModel()` registers the model, setting, and every node as singletons, and `Startup` adds `Models.Model` as another singleton. Every visitor therefore shares one image, one axis, one filter, and one dataset.

## Making state per-user

The workflow spans several requests (load → axis → limits → filter → edit → data), so request scoping alone loses state between steps. Prefer session-scoped state:

- Register the nodes, `Setting`, and `Model` as `AddScoped`, and rehydrate the caller's state per request; or
- Hold a per-session state object keyed by session id, with timed eviction to bound memory.

Whichever you choose, budget for memory: each session holds several full-size `Image<Rgba, byte>` buffers plus an undo history. Evict idle sessions and cap upload size.

Never use `static` fields for this, and never put `Image<Rgba, byte>` in `TempData`, `ViewData`, or cookies.

## Control and concept equivalents

| WPF | Web |
| --- | --- |
| `ViewModelBase.Enter()` / `Leave()` | `OnGet` handler / explicit persist on post |
| `PageService` order and `NextPage`/`PrevPage` | Razor Page routes and redirects, same order |
| `RelayCommand` + `CanExecute` | Post handler + server-side validation + disabled control |
| Two-way `Binding` | `[BindProperty]` fields or a JSON fetch handler |
| `PropertyChanged` push refresh | Explicit re-render of a `_XxxPageView` partial |
| `IFileDialogService` | `IFormFile` upload, content-type allowlist, size limit |
| `IClipboardService` / drag-drop | Browser paste and drop events posting to an upload handler |
| `IMessageBoxService` | Inline validation messages, not a modal |
| `IDownloadService` (image from URL) | Server-side fetch, with SSRF protection on the supplied URL |
| `Image` control bound to `Image<Rgba, byte>` | `<img>` pointing at an endpoint returning `File(bytes, "image/png")` |
| WPF `RangeSlider` for RGB filtering | noUiSlider (already vendored in `wwwroot/nouislider`) |
| Editor undo stack via scoped `IEditService` | Per-session history, or client-side edits committed in batches |
| Long-running work on a background thread | Keep off the request path; expose progress via a polling endpoint |

## Established page patterns

Page models take `Model` and Core services by constructor injection and expose `Model` as a property. Partial refresh handlers use the `OnGet<Name>` convention and return `Partial("_XxxPageView", model)`, so the client can update one panel without a full reload. Keep new handlers on that pattern.

## Images over HTTP

`ImageCaster.ToImgSrc` base64-encodes a PNG into a `data:` URI inlined in the HTML. Fine for a prototype; in production it inflates the payload by about a third and defeats browser and CDN caching. When touching image delivery, switch to a dedicated endpoint returning `File(bytes, "image/png")`.

`ToImageAsync` assumes the upload decodes to a `Bitmap` and throws otherwise — validate content type and size before decoding.

## Verify a port

A Core change that breaks the desktop app is not a successful port.

```powershell
dotnet build .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
dotnet build .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj
dotnet test .\PlotDigitizer.Test\PlotDigitizer.Test.csproj --filter "TestCategory!=UI"
dotnet run  --project .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
```

Then request the route and confirm it renders rather than assuming it does.

## Platform notes

The web project targets `net8.0-windows` and references `Emgu.CV.runtime.windows`, so it runs only on Windows. Linux containers require the corresponding Linux Emgu runtime package and a retarget to `net8.0`. Several ASP.NET packages are still on 3.1.x while the app targets .NET 8; align them when you next touch the project file. Razor runtime compilation and `Westwind.AspNetCore.LiveReload` are development conveniences — keep them out of the production path.
