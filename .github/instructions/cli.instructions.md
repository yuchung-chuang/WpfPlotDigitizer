---
description: Conventions for the PlotDigitizer command-line frontend.
applyTo: "PlotDigitizer.CLI/**"
---

# PlotDigitizer.CLI conventions

A `System.CommandLine` frontend that digitizes one image headlessly. It is the only frontend that exercises the Core dependency graph with no user interaction, which makes it the natural target for end-to-end tests.

## Contract

```
PlotDigitizer -i <image> -s <setting.json> -o <output.csv|.txt>
```

All three options are required. `-i` and `-s` use `ExistingOnly()`, so a missing input file fails during parsing rather than in the handler. Output format is chosen from the extension: `.csv` writes comma-separated, `.txt` writes tab-separated, and anything else throws `FormatException`. Both formats write an `X`/`Y` header row.

Keep this contract stable, or update the end-to-end tests and the sample `Assets/PlotDigitizer Setting.json` in the same change.

## How the digitization actually happens

`Run` only assigns `model.InputImage` and calls `setting.Load(...)`. There is no explicit processing call — reading `model.Data` walks the Core dependency graph and computes the result on demand. Do not "fix" this by adding a manual pipeline invocation.

Because `Setting.Load` skips properties whose value is `default`, a setting file that omits a field, or that specifies `DataType: 0` (`Continuous`), leaves the in-memory default in place. Sample setting files used by tests should set every field explicitly.

## Constraints

- The CLI registers `Model`/`Setting` explicitly and then calls `AddModel()`, which registers them again. The last registration wins for a single resolve, so leave the ordering alone unless you are deliberately cleaning up the composition root.
- Error handling currently prompts on the console for a retry (`try again? (y/n)`) via `Console.ReadKey()`. Any automated test must avoid the failure path, or that call must be moved behind an abstraction first, otherwise the test will hang waiting for input.

```powershell
dotnet run --project .\PlotDigitizer.CLI\PlotDigitizer.CLI.csproj -- -i .\Assets\test.png -s ".\Assets\PlotDigitizer Setting.json" -o .\out.csv
```
