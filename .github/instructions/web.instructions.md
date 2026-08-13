---
description: Conventions and known blockers for the PlotDigitizer ASP.NET Core Razor Pages web frontend.
applyTo: "PlotDigitizer.Web/**"
---

# PlotDigitizer.Web conventions

The web port of the WPF app. It reuses `PlotDigitizer.Core` for all digitization and mirrors the desktop workflow as Razor Pages, in the same order as `PageService.Pages`:

`LoadPage` -> `AxisPage` -> `RangePage` -> `FilterPage` -> `EditPage` -> `DataPage`

All six steps are implemented, including OCR axis detection, the pixel editor with undo/redo, and CSV/TXT export.

## Per-user state is the central design constraint

Core registers the model, setting, and every graph node as singletons through `AddModel()`. That is correct for a single-user desktop app and catastrophic on a server: every visitor would share one image, axis, filter, and dataset. **Never call `AddModel()` from the web project.**

Instead:

- `Services/ServiceCollectionExtensions.AddSessionScopedModel()` re-registers the same graph with `AddScoped`.
- `Services/DigitizerState` owns one DI scope, so it holds a private node graph, `Setting`, and `IEditService` for one browser session. It also owns the editor lifecycle (`EnsureEditorInitialised`, `CommitEdits`) that `EditPageViewModel.Enter`/`Leave` handles on the desktop.
- `Services/DigitizerStateManager` keeps those states in a `MemoryCache` keyed by `HttpContext.Session.Id`, with a sliding idle timeout and a session cap from `DigitizerSessionOptions`. Eviction disposes the state.
- Page models never inject `Model` directly. They inherit `Pages/WorkflowPageModel`, which resolves the caller's state through `IDigitizerStateAccessor` and exposes `Model` and `Setting`.

Each live session holds several full-size `Image<Rgba, byte>` buffers plus an undo history, so memory is bounded deliberately. When you add state to `DigitizerState`, dispose Emgu images in `Dispose` rather than waiting for a finalizer, and read nodes through `.Data` there so disposal never triggers a recomputation.

Do not use `static` fields for user state, and never put `Image<Rgba, byte>` in `TempData`, `ViewData`, or cookies.

## Reuse Core, do not reimplement it

Any digitization behaviour must come from Core. If a page needs logic that only exists in a WPF view model, move that logic into Core first so both frontends share it, then call it from the page model.

Web-only adapters are deliberately thin and must stay that way:

- `Models/Model` extends `UpdatableModel` and adds only render concerns: `DisplayWidth` and versioned image urls.
- `Services/CollectingMessageBoxService` implements Core's `IMessageBoxService` by collecting messages for inline display, because a server cannot show a modal.
- `Services/ImageSourceService` replaces `IFileDialogService`, the clipboard, and drag-and-drop.
- `Services/AxisOcrReader` wraps Core's keyed `IOcrService` instances.

## Workflow and navigation

`Services/WorkflowService` is the web counterpart of `PageService`. It declares the step order, the page names, and which steps the current state allows. `WorkflowPageModel.OnPageHandlerExecuting` redirects a user who deep links into a step they cannot enter yet and sets `TempData["StepBlocked"]`, which the layout renders.

Keep `WorkflowService.Steps` synchronized with `PageService.Pages`. When adding a step, add it in both places and give it a `BlockedReason`.

Guard prerequisites through `WorkflowService.HasAxisLocation`/`HasAxisLimit` rather than testing the raw `Setting` values. A zero-size `AxisLocation` makes `IImageService.CropImage` throw, so it must never reach the graph.

## Page model patterns

Page models inherit `WorkflowPageModel`, override `Step`, and take Core services by constructor injection.

Handlers follow the standard Razor Pages convention, and the verb carries meaning:

- `OnGet` prepares the full page.
- `OnGet<Name>` is for safe, idempotent work: `AxisPage.OnGetDetect`, `DataPage.OnGetExport`.
- `OnPost<Name>` is for anything that mutates session state, which is most partial refreshes: `FilterPage.OnPostFilter`, `EditPage.OnPostUndo`, `DataPage.OnPostDataType`.
- The final `OnPost` advances the workflow through `RedirectToNextStep()`.

Partial refresh handlers return `Partial("_XxxPageView", model)` and the client swaps the result into the page's `#view` panel. Do not leave a partial handler behind once nothing calls it.

Handler routing differs by page and both forms are in use: `@page "{handler?}"` puts the handler in the path, which suits scripts that request `?handler=Name` or `/Page/Name`, while a plain `@page` routes handlers through the `?handler=` query string that `asp-page-handler` generates. Pick one per page and make sure the scripts match it.

Posts are antiforgery protected. `wwwroot/js/site.js` attaches the token to every non-safe jQuery ajax request from the token the layout renders, so page scripts do not repeat that.

## Images over HTTP

Images are served by `Pages/Image.cshtml.cs` at `/image/{kind}`, returning `File(bytes, "image/png")`. Do not go back to inlining base64 `data:` uris; that inflates the payload by roughly a third and defeats caching.

`Models/Model` appends a version token to each url, incremented whenever the corresponding node raises `PropertyChanged` or `PropertyOutdated`. Any new image url must carry a token that changes with its content, otherwise the browser will serve a stale image. Responses are marked `private`, since an image belongs to one session; the live editor and preview kinds are `no-store`.

## Untrusted input

Everything the browser supplies is untrusted, and `ImageSourceService` is the single place that validates it:

- Uploads are checked against a content-type and extension allowlist and `MaxUploadBytes` before decoding, and decoding failures return a message rather than throwing.
- Url fetching resolves the host and refuses loopback, link-local, private, and carrier-grade NAT addresses. Keep that SSRF guard if you touch `FromUrlAsync`.
- `EditPage.OnPostCommit` accepts a canvas image from the client, so it verifies the dimensions match the image being edited before it enters the undo history.

## Front-end

Razor Pages with jQuery, Bootstrap, noUiSlider, and interact.js. Third-party scripts are vendored under `wwwroot/lib` and `wwwroot/nouislider`; do not reference a CDN.

Page behaviour lives in `wwwroot/js/<page>.js`, not in inline `<script>` blocks in the view. Values a script needs from the server are passed through `data-` attributes rather than Razor string interpolation inside JavaScript.

`wwwroot/js/selectionBox.js` is the web equivalent of the WPF `SelectionBox` control, used by both the axis and range pages. It always keeps coordinates in image pixels, so the value posted does not depend on the rendered size. Preserve that when reusing it.

## Platform and dependencies

The project targets `net8.0-windows` and references `Emgu.CV.runtime.windows`, so it only runs on Windows. Deploying to Linux containers requires the corresponding Linux Emgu runtime package and a retarget to `net8.0`.

Keep `Emgu.CV.runtime.windows` on the same version as the `Emgu.CV` reference in `PlotDigitizer.Core`; a mismatch fails at runtime, not at build. Razor runtime compilation is registered only in the Development environment, so keep it out of the production path.

OCR training data (`eng.traineddata`) is copied from Core into the output folder. A web host's working directory is not the output folder, so `AddDigitizerServices` anchors the configured `OCR` path to `AppContext.BaseDirectory`. `AxisOcrReader` also degrades gracefully when Tesseract cannot start, letting the user type axis values by hand instead of failing the request.

```powershell
dotnet build .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
dotnet run --project .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
```

A Core change that breaks the desktop app is not a successful port, so build `PlotDigitizer.WPF` and run the test suite as well.
