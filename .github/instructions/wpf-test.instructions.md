---
description: Conventions for the PlotDigitizer.WPF.Test project (FlaUI desktop automation and WPF-hosted unit tests).
applyTo: "PlotDigitizer.WPF.Test/**"
---

# PlotDigitizer.WPF.Test conventions

This project holds every test that needs a real WPF runtime. It was split out of
[PlotDigitizer.Core.Test](../../PlotDigitizer.Core.Test) so the main test project stays headless; see
[tests.instructions.md](tests.instructions.md) for MSTest/fake/asset conventions shared with
this project — they are not repeated here.

## Two kinds of test, two folders

- `Ui/` — FlaUI UIA3 desktop-automation tests (`UiTest.cs`, `MainWindowUiTests.cs`). These launch
  `PlotDigitizer.exe` as a separate process and drive it through Windows UI Automation, tagged
  `[TestCategory("UI")]`. They require an interactive Windows desktop session and cannot run
  headless or in most CI agents.
- `Wpf/` — unit tests that construct real WPF types in-process (controls, value converters,
  behaviours, validation rules, editor/selection state machines) via `StaTestContext`, which hosts
  a single background STA thread with a running `Dispatcher` for the whole assembly and loads the
  real `App.xaml` resources (without running `OnStartup`) so `{StaticResource ...}` lookups
  resolve. MSTest runs test methods on an MTA thread, so any test touching a WPF object must
  marshal onto that dispatcher via `StaTestContext.Run(...)`.

Follow [wpf.instructions.md](wpf.instructions.md) for automation-id conventions when writing `Ui/`
tests, and this project's existing `Wpf/Controls`, `Wpf/Utilities`, and `Wpf/Services` tests as the
pattern for `Wpf/` tests.

## Shared fakes and assets, not duplicated

This project does not maintain its own `Fakes/` or `Assets/` — its `.csproj` links them from
`PlotDigitizer.Core.Test` by path instead of copying them:

```xml
<Compile Include="..\PlotDigitizer.Core.Test\Fakes\**\*.cs" Link="Fakes\%(RecursiveDir)%(Filename)%(Extension)" />
<None Include="..\PlotDigitizer.Core.Test\Assets\**\*" Link="Assets\%(RecursiveDir)%(Filename)%(Extension)" CopyToOutputDirectory="PreserveNewest" />
```

Add a new fake or asset to `PlotDigitizer.Core.Test` as usual; it becomes available here automatically
through the link. Do not copy-paste a fake or asset into this project.

## Running

```powershell
# Everything in this project (requires an interactive Windows desktop session)
dotnet test .\PlotDigitizer.WPF.Test\PlotDigitizer.WPF.Test.csproj

# Only the FlaUI automation tests
dotnet test .\PlotDigitizer.WPF.Test\PlotDigitizer.WPF.Test.csproj --filter "TestCategory=UI"

# Only the WPF-hosted unit tests (still needs STA/WPF, but launches no separate process)
dotnet test .\PlotDigitizer.WPF.Test\PlotDigitizer.WPF.Test.csproj --filter "TestCategory!=UI"
```

`PlotDigitizer.Core.Test` itself has no `UI`-category tests and needs no filter — see
[tests.instructions.md](tests.instructions.md).
