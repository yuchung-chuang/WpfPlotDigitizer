# PlotDigitizer.WPF.Setup

This project creates framework-dependent WiX v4 MSI packages for the WPF desktop application and a portable x64 folder.

## Prerequisites

- .NET 8 SDK
- WiX Toolset v4 SDK package restore access
- .NET 8 Windows Desktop Runtime installed on the target computer

The installers and portable build do not install .NET. The portable build is a folder containing the executable and its dependencies; copy the complete folder before launching `PlotDigitizer.exe`.

## Build

Run these commands from the repository root:

```powershell
# x86 MSI
dotnet build .\PlotDigitizer.WPF.Setup\PlotDigitizer.WPF.Setup.wixproj --configuration Release -p:Platform=x86

# x64 MSI + portable x64 folder
dotnet build .\PlotDigitizer.WPF.Setup\PlotDigitizer.WPF.Setup.wixproj --configuration Release -p:Platform=x64

# everything in one pass
dotnet msbuild .\PlotDigitizer.WPF.Setup\PlotDigitizer.WPF.Setup.wixproj -target:BuildAllArtifacts -property:Configuration=Release
```

## Outputs

| Artifact | Path |
| --- | --- |
| x86 installer | `bin\x86\Release\PlotDigitizer-x86.msi` |
| x64 installer | `bin\x64\Release\PlotDigitizer-x64.msi` |
| Portable x64 | `bin\Release\portable-x64\` |

The portable folder is produced automatically by any x64 build. It is plain `dotnet publish` output, so it needs no installer — copy the whole folder and run `PlotDigitizer.exe` from it.
