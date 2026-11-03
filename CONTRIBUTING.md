# Contributing

Contributions are welcome. This repository is a deliberately narrow Codebelt-maintained Microsoft Testing Platform variant of Coverlet, so changes should stay close to upstream Coverlet internals unless the fork objective requires a different package, framework, dependency, or release shape.

## Requirements

Use the .NET SDK pinned by `global.json`. The retained production matrix is `netstandard2.0`, `net9.0`, and `net10.0`; tests execute on `net9.0` and `net10.0`.

## Release branches

Target `releases/v10` for changes to the 10.x maintenance line, following the maintenance-branch naming in the [GitHub Actions versioning example](https://github.com/actions/toolkit/blob/main/docs/action-versioning.md#sample-workflow). Its starting point is the `v10.1.1` release commit, and the 10.1.2 patch retains that release's SDK, dependency versions, and `netstandard2.0;net9.0;net10.0` package targets. CI accepts pull requests targeting either `releases/v10` or `master`.

`master` continues development toward 11.0.0, including its .NET 11 changes and recorded upstream integrations. Keep changes for that release on `master` and the existing 11.0.0 branches. Forward-port maintenance fixes separately and validate them against the destination branch's framework matrix. Merging `master` into a maintenance hotfix would also bring its later product and framework changes into the patch release.

After integrating a maintenance pull request and completing its checks, create the fixed full-version tag, such as `v10.1.2`, on the exact commit to release before starting the manual CI/release workflow. MinVer otherwise calculates a prerelease version above the latest release tag. Select `releases/v10` in the workflow's branch dropdown while that tagged commit is still its tip, or use `gh workflow run ci-pipeline.yml --ref v10.1.2` to select the tagged commit explicitly. The existing workflow builds, tests, packs, and validates that source before requesting approval in the `Production` environment for NuGet publication. The environment currently permits this maintenance branch and retains its required reviewer. The Actions guide's moving major-version tags are separate from this maintenance branch; NuGet releases retain fixed full-version tags. Do not move existing release tags.

CI requires Windows and Ubuntu tests and package validation. macOS is optional because hosted-runner capacity can prevent jobs from starting. To include it, enable `run_mac_tests` in the manual workflow form, or add `-f run_mac_tests=true` to the workflow command. The option includes macOS in both matrices; every selected job must pass before publication. A default run does not establish macOS compatibility. This follows the opt-in behavior in [Codebelt xUnit's workflow](https://github.com/codebeltnet/xunit/blob/main/.github/workflows/ci-pipeline.yml).

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

The MTP package-validation tests consume the locally packed `Codebelt.Coverlet.MTP` package from `artifacts/package/release`, so pack before running `test/coverlet.MTP.validation.tests` directly.

## Focused test runs

For this repository's xUnit v3 Microsoft Testing Platform test apps, use xUnit-specific filters such as `--filter-method`, `--filter-class`, or `--filter-query`; VSTest-style `--filter` is unsupported.

Examples:

```bash
dotnet test test/coverlet.MTP.tests/coverlet.MTP.tests.csproj -c Release -f net10.0 -- --filter-class Coverlet.MTP.Collector.Tests.CollectorExtensionTests
dotnet test test/coverlet.core.tests/coverlet.core.tests.csproj -c Release -f net9.0 -- --filter-method PatternMatchingOr_Should_Report_100_Percent_Branch_Coverage
```

## Debugging isolated coverage tests

`test/coverlet.core.coverage.tests` uses an isolated child-process path for instrumentation tests that require full process separation. Set `COVERLET_DEBUG_CHILD_PROCESS=1` before running a focused test if you need the child process to wait for debugging.

```pwsh
$env:COVERLET_DEBUG_CHILD_PROCESS = "1"
dotnet test test/coverlet.core.coverage.tests/coverlet.core.coverage.tests.csproj -c Release -f net10.0 -- --filter-query "/[method]=Coverage_SkippedInstrumentedMethod"
$env:COVERLET_DEBUG_CHILD_PROCESS = $null
```
