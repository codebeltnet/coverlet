# Coverage advisory: incomplete library coverage

Recorded on 2026-11-03 for the `Codebelt.Coverlet.MTP` 10.1.2 patch. This advisory describes inspected source and validation performed; it does not establish that a package has been published.

> [!WARNING]
> `Codebelt.Coverlet.MTP` 10.0.1, 10.1.0, and 10.1.1 can silently omit library assemblies when automatically generated dependency exclusions match a shared assembly-name prefix. Tests and CI can pass, and Codecov or Sonar can display a high percentage for the small amount of code that remains. Check the expected modules and source files, not just the percentage. The 10.1.2 patch corrects this filtering and a separate framework assembly-resolution defect reproduced with Cuemon on .NET Framework 4.8. The two defects have different upstream origins and failure conditions.

## Affected scope

The defect concerns automatically generated MTP exclusions. It is not limited to Codebelt names or C# namespaces: it matches assembly names, normally the DLL filename without its extension.

With default exclusions, the old implementation reads non-project dependency library names from `*.deps.json` and assemblies loaded in the controller. For names with at least three dot-separated segments, it replaces everything after the second segment with `.*`. Skipping project entries during discovery does not protect their assemblies from another dependency's resulting wildcard.

| Discovered dependency | Old generated exclusion | Unrelated project assembly matched |
| --- | --- | --- |
| `Codebelt.Extensions.Xunit` | `[Codebelt.Extensions.*]*` | `Codebelt.Extensions.BenchmarkDotNet.dll` |
| `Example.Testing.Infrastructure` | `[Example.Testing.*]*` | `Example.Testing.Product.dll` |

This does not mean every library, every Codebelt assembly, or every use of an affected version loses coverage. The discovered names, production assembly names, and effective configuration determine the result. A report may lose some modules or become entirely empty. Cuemon's successful coverage is compatible with this failure: the Codebelt example's wildcard does not match `Cuemon.*` assemblies.

An explicit `--coverlet-exclude` option bypasses dynamic discovery and merges the supplied filters with the permanent baseline. An authoritative configuration file also bypasses dynamic discovery. Those precedence rules are preserved. Explicit include filters do not override a matching exclusion.

## Origin and affected versions

The prefix-generating code originated in [upstream PR #1946](https://github.com/coverlet-coverage/coverlet/pull/1946), [commit `c5022539cfdf3c6246c2e49b7789d7e8e529ef47`](https://github.com/coverlet-coverage/coverlet/commit/c5022539cfdf3c6246c2e49b7789d7e8e529ef47), whose source-commit date is 2026-06-01. The change combined loaded-assembly discovery with `deps.json` discovery and introduced wildcard expansion after the second name segment. The Codebelt fork inherited that implementation; package branding did not introduce it.

| Codebelt package version | Release date recorded in this repository | Evidence for this defect |
| --- | --- | --- |
| 10.0.1 | 2026-09-20 | Tagged source already contains the broad filters. No full consumer rerun against this package was performed. |
| 10.1.0 | 2026-09-27 | Tagged source retains the same logic. No full consumer rerun against this package was performed. |
| 10.1.1 | 2026-10-20 | Tagged source retains the logic; full baseline runs reproduced missing modules in the seven repositories below. |
| 10.1.2 | Patch target in this branch | Exact dependency assembly filters and their validation are corrected; local fixed-collector runs are recorded below. |

Source anchors: [10.0.1 helper](https://github.com/codebeltnet/coverlet/blob/v10.0.1/src/coverlet.core/Helpers/ProcessAssemblyHelper.cs), [10.1.0 helper](https://github.com/codebeltnet/coverlet/blob/v10.1.0/src/coverlet.core/Helpers/ProcessAssemblyHelper.cs), and [10.1.1 helper](https://github.com/codebeltnet/coverlet/blob/v10.1.1/src/coverlet.core/Helpers/ProcessAssemblyHelper.cs). Release dates come from the [changelog](../CHANGELOG.md), not inferred tag-creation times. Identically numbered upstream `coverlet.MTP` and Codebelt packages must not be treated as the same release. The fork's original upstream marker and its later release/comparison commits are distinguished in [UPSTREAM.md](../UPSTREAM.md).

## Historical BenchmarkDotNet evidence

The [2026-10-22 BenchmarkDotNet CI run](https://github.com/codebeltnet/benchmarkdotnet/actions/runs/39814229489), at commit `5cfb096dee8c2c9369e8651ac0d937d4df7ce8ff`, succeeded. Its [Codecov commit record](https://api.codecov.io/api/v2/github/codebeltnet/repos/benchmarkdotnet/commits/5cfb096dee8c2c9369e8651ac0d937d4df7ce8ff/) reports 92.3% coverage, but contains only `tooling/Codebelt.Extensions.BenchmarkDotNet.Runner/Program.cs`: 12 covered lines out of 13, with no library `src/` files.

The downloaded `TestResults-Release-Linux-X64-` artifact establishes that the omission happened before Codecov ingestion. Its .NET 10 and .NET 11 OpenCover reports have zero modules for the main unit tests, console unit tests, and console functional tests. The main functional tests report only `dotnet-bdn-runner`, with 11 of 12 sequence points visited in that Linux build. Its assembly name does not match `[Codebelt.Extensions.*]*`, explaining why a nonempty report survived.

An [earlier Codecov record](https://api.codecov.io/api/v2/github/codebeltnet/repos/benchmarkdotnet/commits/87a13e5b7162fb35c417658976a421a53e4d5683/) includes both libraries and the runner. These are distinct compilation/reporting snapshots; their totals demonstrate lost report scope, not a controlled percentage comparison.

| Recorded commit | Files | Measured lines | Covered lines | Coverage |
| --- | ---: | ---: | ---: | ---: |
| `87a13e5b7162fb35c417658976a421a53e4d5683` | 7 | 389 | 355 | 91.25% |
| `5cfb096dee8c2c9369e8651ac0d937d4df7ce8ff` | 1 | 13 | 12 | 92.3% |

## Correction and compatibility

The 10.1.2 correction reads actual managed runtime asset filenames from non-project `deps.json` libraries, including RID-specific assets and packages containing multiple assemblies. Package IDs no longer stand in for assembly names. Both dependency-derived names and loaded assembly names produce exact assembly filters; filter pruning still removes exact filters already covered by an intentional permanent baseline wildcard.

Project runtime assets are removed from dependency-derived exclusions even if the SDK lists the same DLL again in a non-project reference entry. Expanded BenchmarkDotNet functional validation exposed this compatibility case for `dotnet-bdn-runner`: the duplicate reference must not suppress a DLL already identified as project output. Reference-only external assets remain eligible for dependency exclusion.

Native assets and `_._` placeholders do not become managed assembly exclusions. Exact defaults also retain the known xUnit build assets copied without runtime dependency entries: `xunit.abstractions`, `xunit.runner.visualstudio.testadapter`, `xunit.v3.runner.utility.netcore`, and `xunit.v3.runner.utility.netfx`. The filter validator now accepts hyphenated names such as `xunit.v3.mtp-v2`, rather than silently discarding those exact filters as invalid.

The patch preserves package identity, assembly identity, public API surface, dependency versions, and the `netstandard2.0;net9.0;net10.0` package targets. It restores the intended separation between dependency assemblies and project assemblies. The planned major release's framework changes are outside this hotfix. The patch classification does not promise identical report contents: restored modules can change totals, percentages, coverage thresholds, and quality-gate outcomes. Review those changes rather than automatically lowering a threshold to preserve an old result.

Existing wildcard syntax and CLI/configuration precedence remain intact. An explicitly configured broad wildcard still excludes every matching module. Hyphenated filter expressions are now honored where the previous validator rejected them.

## Consumer verification on 2026-11-03

The comparison used Windows x64 and .NET 10 Release builds of isolated copies of the available local consumer checkouts, including their existing edits. These are not pristine release-tag builds. All discovered test projects in the seven repositories were run. Original checkouts were not edited. The released 10.1.1 collector was the baseline; only `coverlet.core.dll` and `coverlet.MTP.dll` were replaced for fixed runs. Exclusions were unchanged. OpenCover was selected to inspect visited sequence points.

| Repository | Test projects | Tests passing with each collector | Baseline library modules | Fixed library modules | Fixed visited / total sequence points |
| --- | ---: | ---: | ---: | ---: | ---: |
| BenchmarkDotNet | 4 | 171 | 0 | 2 | 240 / 257 |
| Carter | 6 | 39 | 0 | 5 | 52 / 52 |
| Globalization | 1 | 1,223 | 0 | 1 | 121 / 121 |
| ASP.Versioning | 2 | 237 | 0 | 1 | 383 / 387 |
| Swashbuckle.AspNetCore | 4 | 121 | 0 | 2 | 779 / 782 |
| Newtonsoft.Json | 3 | 151 | 0 | 3 | 637 / 638 |
| AWS Signature V4 | 1 | 1 | 0 | 1 | 65 / 65 |
| Total | 21 | 1,943 | 0 | 15 | 2,277 / 2,302 |

The table counts library modules and their sequence points. BenchmarkDotNet also retains `dotnet-bdn-runner` coverage, with 11 of 12 sequence points visited in its functional test report. Its main library unit-test report alone has 173 of 194 sequence points visited. For repositories with multiple test projects, sequence points are deduplicated by module, source file, method, and source span; visited points are the union across their reports. They are not sums of repeated module totals. These results confirm module inclusion and executed code, not that every line must be covered. Swashbuckle's first long-path temporary copy triggered a Windows XML/URI path error with both collectors; the reported comparison was repeated successfully from a short path.

All 21 projects were rerun after the separate resolver correction described below. All 1,943 tests passed again, and each project's module names, total sequence points, and visited sequence points matched its preceding filter-corrected report. Those reruns are retained in each project's `fixed-resolver` directory, with `resolver-consumer-summary.json` recording the comparison under the local validation-artifact root.

Local snapshot provenance (all include pre-existing working-tree edits): BenchmarkDotNet `c95c59c`, Carter `bb8e6b5`, Globalization `af2cf95`, ASP.Versioning `b79c017`, Swashbuckle.AspNetCore `3393a7c`, Newtonsoft.Json `c061a0b`, and AWS Signature V4 `cc77402`. A commit identifier alone does not reproduce those edited snapshots. Build logs, test logs, snapshot metadata, and raw reports are retained locally under `artifacts/dependency-exclusion-validation`; they are ignored validation artifacts, not files shipped in the package.

## Separate Cuemon .NET Framework instrumentation fix

> [!IMPORTANT]
> The dependency-prefix correction alone did not repair Cuemon's .NET Framework coverage. A second correction in 10.1.2 fixes the reproduced assembly-resolution failure. Successful test execution alone concealed both defects.

An isolated `Cuemon.Core.Tests` .NET Framework 4.8 run passed all 729 tests with both released 10.1.1 and the collector containing only the dependency-filter correction. Both reports omitted `Cuemon.Core`, `Cuemon.Kernel`, and `Cuemon.Extensions.Core`. Diagnostics showed `Mono.Cecil.AssemblyResolutionException` while writing metadata for optional enum parameters: `Cuemon.Core.dll` and `Cuemon.Extensions.Core.dll` could not resolve `System, Version=4.0.0.0`, and `Cuemon.Kernel.dll` could not resolve `mscorlib, Version=4.0.0.0`. The collector restored the modules after those instrumentation failures and continued running tests. The resulting reports had only 832 visited sequence points out of 4,286 across the remaining modules.

The runtime `netstandard.dll` facade was cached globally as a Cecil assembly loaded without `ReaderParameters.AssemblyResolver`. Resolving a forwarded enum therefore used Cecil's default resolver instead of Coverlet's configured runtime/reference-assembly fallbacks. The correction loads the same runtime facade lazily per resolver with `AssemblyResolver = this` and registers it in the resolver's cache for disposal. Official facade identity checks, framework search order, and exclusion configuration are preserved; there are no Cuemon-specific paths or names in production code.

That unbound facade-loading pattern is already present in upstream [PR #625](https://github.com/coverlet-coverage/coverlet/pull/625), [commit `3856c76a5627a8d38d2cf2eae9360f05f7d878c5`](https://github.com/coverlet-coverage/coverlet/commit/3856c76a5627a8d38d2cf2eae9360f05f7d878c5), whose source-commit date is 2019-12-03. This establishes source provenance, not the first failing package release or that every historical .NET Framework workload failed. It is distinct from dependency-filter PR #1946. The concrete failure was reproduced against released Codebelt 10.1.1 and the intermediate filter-only correction.

After replacing only the two `netstandard2.0` collector DLLs with the resolver-corrected build, the same 729 tests passed with unchanged exclusion settings. Diagnostics contained no module-instrumentation failures. The three previously missing library modules appeared with executed sequence points:

| Module | Before resolver correction | Fixed visited / total sequence points |
| --- | --- | ---: |
| `Cuemon.Core` | Missing | 2,194 / 2,825 |
| `Cuemon.Kernel` | Missing | 465 / 1,245 |
| `Cuemon.Extensions.Core` | Missing | 205 / 692 |

That snapshot is based on local Cuemon commit `9d5e7197` plus existing edits. It was built with SDK `11.0.100-rc.3.26522.148`; `MinVerVersionOverride=11.0.0-rc.3` retained the assembly major version expected by its tests in the Git-free copy. An initial build without that override failed one assembly-version assertion with either collector; the versioned comparison above passed all tests. This check covers one Cuemon test project on Windows x64, not its entire .NET Framework suite. Logs and reports for the focused reproduction are retained under `artifacts/dependency-exclusion-validation/consumers/cuemon-net48-versioned/resolver-before` and `resolver-after`, with adjacent test logs.

## Regression protection and validation limits

[CoverageConfigurationDependencyDiscoveryTests](../test/coverlet.MTP.tests/Configuration/CoverageConfigurationDependencyDiscoveryTests.cs) exercises real `deps.json` reading, dynamic configuration, production filter validation, and module selection. It covers the reported Codebelt names, a generic shared prefix, differing package/assembly names, multiple runtime assemblies, RID-specific assets, duplicate project/reference entries, hyphens, infrastructure exclusions, and explicit CLI/configuration wildcard behavior. [ProcessAssemblyHelperTests](../test/coverlet.core.tests/Helpers/ProcessAssemblyHelperTests.cs) and [InstrumentationHelperTests](../test/coverlet.core.tests/Helpers/InstrumentationHelperTests.cs) cover discovery, exact filters, pruning, and filter validity.

[CecilAssemblyResolverTests](../test/coverlet.core.tests/Instrumentation/CecilAssemblyResolverTests.cs) resolves real runtime facade forwards through a private framework search directory for `StringComparison` and `MidpointRounding`, then writes a consumer assembly containing an optional enum parameter. The original resolver failed the configured-path assertions; the corrected resolver passes both cases. This regression exercises Cecil's real resolution and metadata-writing path without requiring Cuemon or a .NET Framework installation.

On Windows, after the resolver correction, the complete core suite passed 351 tests with nine platform skips, and the complete MTP suite passed 427 tests with seven platform skips, on each of .NET 9 and .NET 10. The `netstandard2.0` build passed without warnings or errors. The consumer checks above used replacement collector DLLs, not installation of a newly packed NuGet package. Fixed-consumer runs on Linux/macOS and .NET 11 were not performed; the historical .NET 11 reports establish the earlier failure only. These results do not assert that all possible coverage failures are resolved.

## Ubuntu remote validation on 2026-11-03

The current working tree, including the resolver correction and its new tests, was built and tested in an isolated Linux x64 Docker workspace through the `dotnet-remote-testing` skill. The runner automatically selected `codebeltnet/ubuntu-testrunner:8-9-10-11` to cover the discovered framework set, pinned to digest `sha256:1df4b744ff2b2bf215fa2dc63e4f81b3c8dcf7c701c9bd456c78a2ec160713d9`. The executed test projects target .NET 9 and .NET 10; the image's additional SDKs do not establish execution on those other frameworks. The observed build/test SDK was 10.0.403. The run used the root `coverlet.slnx` solution and Debug configuration without modifying project files or package references.

| Test project | .NET 9 passed / skipped / failed | .NET 10 passed / skipped / failed |
| --- | ---: | ---: |
| `coverlet.core.tests` | 347 / 13 / 0 | 347 / 13 / 0 |
| `coverlet.MTP.tests` | 417 / 17 / 0 | 417 / 17 / 0 |
| `coverlet.core.coverage.tests` | 95 / 0 / 0 | 95 / 0 / 0 |
| `coverlet.MTP.validation.tests` | 2 / 6 / 36 | 2 / 6 / 36 |

Both forwarded-enum resolver cases passed on each framework. All six real dependency-discovery/filter-application cases also passed on each framework, including the exact `Codebelt.Extensions.Xunit` / `Codebelt.Extensions.BenchmarkDotNet` reproduction, generic names, and explicit CLI/configuration wildcards. The three core/MTP/instrumentation suites passed 1,718 tests in total with 60 platform skips.

The entire solution run was not green: its aggregate result was 1,722 passed, 72 skipped, and 72 failed, with exit code 1. Every failure was in package validation because `/workspace/artifacts/package/debug` was absent from the isolated workspace. For example, `CollectCoverageTests.BasicCoverage_CollectsDataForCoveredLines` failed on both frameworks with `DirectoryNotFoundException` in `MtpValidationTestBase.GetCoverletMtpPackageVersion`, which requires a package produced by `dotnet pack` first. CI supplies that package in a separate pack/download phase; the remote runner performs restore, build, and test. This run does not validate installation or use of a newly packed package.

The runner reported 547.2 seconds of cumulative test time and 325.7 seconds of wall time including preparation, restore, and build; parallel execution accounts for cumulative test time exceeding wall time. Runner output and the retained core, MTP, and package-validation TRX files are under `artifacts/dependency-exclusion-validation/ubuntu-remote`. Cuemon's actual .NET Framework 4.8 execution remains the Windows check described above; the portable resolver regression was verified on Linux. Modern consumer repositories were not rerun in this container.

## Upgrade and report review

1. When the 10.1.2 package is available, update the test projects that use an affected `Codebelt.Coverlet.MTP` version and regenerate coverage.
2. Confirm each expected library module appears in the raw report with visited sequence points, and check the included source-file list before accepting an aggregate percentage. For JSON, inspect module keys; for OpenCover, inspect `Modules/Module`, source files, and visited sequence points.
3. Re-evaluate Codecov/Sonar totals and coverage gates against the restored module set. Existing historical uploads are not repaired by upgrading the collector.

If upgrading is deferred, an explicitly reviewed CLI exclusion set or authoritative configuration file can bypass dynamic defaults. In that mode, maintain the required infrastructure/external-dependency exclusions yourself; simply adding an include filter does not override the bad generated exclusion. See [configuration precedence and default exclusions](Coverlet.MTP.Integration.md#default-exclusions-behavior).

Implementation provenance and full commit identifiers are recorded in [UPSTREAM.md](../UPSTREAM.md#2026-11-03-dependency-exclusion-correction-for-1012).
