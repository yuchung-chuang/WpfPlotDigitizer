---
name: port-wpf-page-to-web
description: Port a PlotDigitizer WPF page to the ASP.NET Core Razor Pages frontend, relocating shared logic into Core.
argument-hint: "[page to port, e.g. FilterPage]"
agent: web-port-architect
---

Port this WPF page to the web frontend: ${input:page:Page to port, e.g. AxisPage, FilterPage, EditPage, or DataPage}

Do the work yourself: move the code, wire it up, build both frontends, and verify the page renders.

1. **Read both sides first** — the WPF view model in `PlotDigitizer.Core/ViewModels/`, its view in `PlotDigitizer.WPF/View/`, and any existing Razor page and `_XxxPageView` partial for the same step.

2. **Separate behaviour from presentation.** List what is genuine digitization logic and what is WPF presentation. Anything in the first group that still lives in WPF code-behind must move into Core, with WPF updated to call the relocated code. **Never copy logic from a WPF view model into a page model** — a fork is how the frontends drift apart. Core targets `netstandard2.1`, so express relocated logic with `PointD`/`RectangleD` rather than `System.Windows` types.

3. **Confirm the composition root is sound before building on it.** The web project does not register `IImageService`, so `AddModel()`'s nodes and `AxisPageModel` cannot be constructed; add `services.AddTransient<IImageService, EmguCvService>()`. And `Model` is a singleton shared by every visitor — if this page introduces per-user state, make the graph session-scoped rather than adding another shared singleton.

4. **Translate the interaction model:**
   - `Enter()` becomes the `OnGet` handler; `Leave()` becomes an explicit persist on post.
   - `RelayCommand` + `CanExecute` becomes a post handler plus server-side validation.
   - Two-way bindings become `[BindProperty]` fields or a JSON fetch handler.
   - Partial refreshes follow the existing convention: `OnGet<Name>` returning `Partial("_XxxPageView", model)`.
   - Images should come from an endpoint returning `File(bytes, "image/png")` rather than a base64 `data:` URI when you are touching image delivery.

5. **Verify — a Core change that breaks the desktop app is not a successful port:**

```powershell
dotnet build .\PlotDigitizer.Web\PlotDigitizer.Web.csproj
dotnet build .\PlotDigitizer.WPF\PlotDigitizer.WPF.csproj
dotnet test .\PlotDigitizer.Core.Test\PlotDigitizer.Core.Test.csproj
```

Then run the web app and request the route to confirm it actually renders.

Report what moved into Core, what stayed frontend-specific, how per-user state is handled, and anything deliberately deferred.
