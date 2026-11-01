# Contributing

Contributions are welcome. This repository is a deliberately narrow Codebelt-maintained Microsoft Testing Platform variant of Coverlet, so changes should stay close to upstream Coverlet internals unless the fork objective requires a different package, framework, dependency, or release shape.

## Requirements

Use the .NET SDK pinned by `global.json`. The retained production matrix is `netstandard2.0`, `net10.0`, and `net11.0`; tests execute on `net10.0` and `net11.0`.

## Building, testing, and packing

Clone this repository:

```bash
git clone https://github.com/codebeltnet/coverlet.git
cd coverlet
```

Use the standard .NET commands:

```bash
dotnet restore
dotnet build -c Release --no-restore
dotnet pack src/coverlet.MTP/coverlet.MTP.csproj -c Release --no-build
dotnet test -c Release --no-build
```

The MTP package-validation project automatically packs `Codebelt.Coverlet.MTP` when built locally, including builds started by Visual Studio Test Explorer. No manual packing or package-folder cleanup is required. Each configuration and test framework uses an isolated directory under `artifacts/validation`, so older packages in `artifacts/package` do not affect local validation. The first build takes longer because it builds the package for all supported frameworks; subsequent package builds reuse their build outputs.

In GitHub Actions, automatic packing is disabled so validation consumes the downloaded release package from `artifacts/package/release`. Outside GitHub Actions, use `-p:PrepareValidationPackage=false` to validate an existing package from `artifacts/package/<configuration>` instead. That directory must contain exactly one `Codebelt.Coverlet.MTP.*.nupkg`. As usual, `dotnet test --no-build` reuses the package prepared by the previous build.

## Focused test runs

For this repository's xUnit v3 Microsoft Testing Platform test apps, use xUnit-specific filters such as `--filter-method`, `--filter-class`, or `--filter-query`; VSTest-style `--filter` is unsupported.

Examples:

```bash
dotnet test test/coverlet.MTP.tests/coverlet.MTP.tests.csproj -c Release -f net10.0 -- --filter-class Coverlet.MTP.Collector.Tests.CollectorExtensionTests
dotnet test test/coverlet.core.tests/coverlet.core.tests.csproj -c Release -f net11.0 -- --filter-method PatternMatchingOr_Should_Report_100_Percent_Branch_Coverage
```

## Debugging isolated coverage tests

`test/coverlet.core.coverage.tests` uses an isolated child-process path for instrumentation tests that require full process separation. Set `COVERLET_DEBUG_CHILD_PROCESS=1` before running a focused test if you need the child process to wait for debugging.

```pwsh
$env:COVERLET_DEBUG_CHILD_PROCESS = "1"
dotnet test test/coverlet.core.coverage.tests/coverlet.core.coverage.tests.csproj -c Release -f net10.0 -- --filter-query "/[method]=Coverage_SkippedInstrumentedMethod"
$env:COVERLET_DEBUG_CHILD_PROCESS = $null
```
