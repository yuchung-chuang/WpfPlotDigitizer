# PlotDigitizer repository instructions

## Global rules

- `rg` is unavailable in this Windows environment; use built-in PowerShell commands for search.
- Use the .NET SDK selected by [global.json](../global.json) for .NET projects. There is no solution
  file, so build or test the individual project being changed.
- Preserve existing project lifetimes, public contracts, and user changes. Follow [.editorconfig](../.editorconfig).

## Scoped instructions

Use the nearest matching file in [.github/instructions](instructions) for project-specific rules:

- `PlotDigitizer.Core/**` -> [core.instructions.md](instructions/core.instructions.md)
- `PlotDigitizer.Core.Test/**` -> [tests.instructions.md](instructions/tests.instructions.md)
- `PlotDigitizer.CLI/**` -> [cli.instructions.md](instructions/cli.instructions.md)
- `PlotDigitizer.WPF/**` -> [wpf.instructions.md](instructions/wpf.instructions.md)
- `PlotDigitizer.WPF.Test/**` -> [wpf-test.instructions.md](instructions/wpf-test.instructions.md)
- `PlotDigitizer.Web/**` -> [web.instructions.md](instructions/web.instructions.md)
- `.scratch/plot-digitizer-agent/**` and `.github/skills/**` -> [agent-framework.instructions.md](instructions/agent-framework.instructions.md)

When a change spans projects, apply every matching scoped instruction. Keep shared behavior in Core
and keep frontend-specific APIs in the frontend that owns them.
