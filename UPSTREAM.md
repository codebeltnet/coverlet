# Upstream relationship and synchronization record

This repository is a selective fork of [coverlet-coverage/coverlet](https://github.com/coverlet-coverage/coverlet), maintained as `Codebelt.Coverlet.MTP`. Coverlet supplies the instrumentation engine and Microsoft Testing Platform integration. The Codebelt variant focuses on explicit target-framework support, package dependencies, and its development and release experience.

The compatibility goal is to preserve upstream behavior within the retained surface. Meaningful behavior differences are listed below. Synchronization preserves upstream ancestry while reviewing file changes selectively; recording a commit as an ancestor does not adopt every change in it. This document records repository state, which can include work that has not yet been released. See [CHANGELOG.md](CHANGELOG.md) for release outcomes, [README.md](README.md) for installation and usage, and [AGENTS.md](AGENTS.md) for synchronization rules.

## Current reviewed state

| Item | Recorded state |
| --- | --- |
| Upstream | [`coverlet-coverage/coverlet`, `master`](https://github.com/coverlet-coverage/coverlet/tree/master) |
| Latest reviewed upstream commit | [`f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a`](https://github.com/coverlet-coverage/coverlet/commit/f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a), dated 2026-11-03 |
| Latest sync operation | 2026-11-03; selective upstream merge on `v11.0.0/double-digits-chore` |
| Codebelt baseline | [`feef6ea0687684ecadc0bd147cc4d6a42f82f93f`](https://github.com/codebeltnet/coverlet/commit/feef6ea0687684ecadc0bd147cc4d6a42f82f93f) |
| Upstream ancestry merge | The local merge carrying this entry records the baseline as first parent and the reviewed upstream tip as second parent. |
| Destination integration | Pending; this operation is on the requested branch, not an integration or release. Earlier synchronization was integrated by [PR #7](https://github.com/codebeltnet/coverlet/pull/7). |
| Validation | Windows Release solution build, package inspection, and all four retained test suites passed on .NET 10/11. Cross-platform CI remains pending. Earlier validation evidence is recorded in the previous history entries. |

## Original marker and release tags

Tag targets below were checked against this repository's remote on 2026-10-31. The `-coverlet` marker is the original starting point supplied for the fork; it must not be confused with the later Codebelt tag of the same version. These tags identify repository commits, rather than proving that all upstream changes through a similarly named upstream release were imported.

| Tag | Commit | Date evidence | Meaning |
| --- | --- | --- | --- |
| [`v10.0.1-coverlet`](https://github.com/codebeltnet/coverlet/tree/v10.0.1-coverlet) | [`7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17`](https://github.com/coverlet-coverage/coverlet/commit/7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17) | Source commit dated 2026-05-18; tag creation date is not recorded by this lightweight tag | Original upstream starting marker. |
| [`v10.0.1`](https://github.com/codebeltnet/coverlet/tree/v10.0.1) | [`9766806`](https://github.com/codebeltnet/coverlet/commit/97668061b72e03668de55d6cca39a559c1d3ff10) | Integration commit dated 2026-09-20 | First Codebelt release boundary, including [PR #1](https://github.com/codebeltnet/coverlet/pull/1) and the versioning/release guards in [PR #2](https://github.com/codebeltnet/coverlet/pull/2). |
| [`v10.1.0`](https://github.com/codebeltnet/coverlet/tree/v10.1.0) | [`91c8882`](https://github.com/codebeltnet/coverlet/commit/91c888280969bb23e95bbc471126714915994b2c) | Integration commit dated 2026-09-28 | Selective MTP/configuration and instrumentation ports in [PR #3](https://github.com/codebeltnet/coverlet/pull/3). |
| [`v10.1.1`](https://github.com/codebeltnet/coverlet/tree/v10.1.1) | [`5bd8648`](https://github.com/codebeltnet/coverlet/commit/5bd86483a104328783255960116c1fb87ce2b35d) | Integration commit dated 2026-10-20 | Dependency alignment and Codebelt tracker-mutex maintenance in [PR #4](https://github.com/codebeltnet/coverlet/pull/4). |

Upstream history continued after the original marker before the first Codebelt release. Before the 2026-11-01 ancestry merge, the shared comparison base was [`da5c649`](https://github.com/coverlet-coverage/coverlet/commit/da5c649126c74985fc20c496bc898f143d18e21b), dated 2026-09-17: it includes 53 upstream commits after `7fbbe4e`. Thus `7fbbe4e` is the starting marker, while `da5c649` is the inherited upstream boundary used for earlier comparisons. The first Codebelt release then narrowed that inherited tree. The chronology does not establish separate sync-operation dates for those 53 commits. The ancestry merge advances the shared base to the exact reviewed tip without changing these historical boundaries or release tags.

## Intentional differences from upstream

These are the meaningful differences at the reviewed snapshots. They include existing fork fixes as well as packaging choices; they are not all changes from the latest sync.

| Area | Difference and reason | Evidence |
| --- | --- | --- |
| Package surface | Publish one package, `Codebelt.Coverlet.MTP`, retaining the Coverlet MTP assembly, namespace, and discovery-hook identities. Console, VSTest collector, and legacy MSBuild packages are outside this fork's surface. | [MTP project](src/coverlet.MTP/coverlet.MTP.csproj), [scope](README.md#scope) |
| Target frameworks and dependencies | Retain `netstandard2.0;net10.0;net11.0`, with .NET 10/11 runtime dependency versions selected per TFM and explicit compatibility overrides for .NET Standard 2.0. | [Central versions](Directory.Packages.props), [core project](src/coverlet.core/coverlet.core.csproj) |
| Tooling and versioning | Use the repository SDK selection, Microsoft Testing Platform runner, MinVer `v*` tags, Codebelt reusable workflows, and guarded package publication. Local package-validation builds prepare an isolated package; CI validates its downloaded package. | [SDK and runner](global.json), [CI](.github/workflows/ci-pipeline.yml), [contributing guide](CONTRIBUTING.md) |
| Restoring instrumented files | Stage the backup beside the target before replacement, rather than copying over the loaded file in place. Retry handling also covers access-denied failures. This addresses loaded-assembly corruption and locked-file restoration. | [Restoration](src/coverlet.core/Helpers/InstrumentationHelper.cs), [filesystem operations](src/coverlet.core/Helpers/FileSystem.cs), [retry handling](src/coverlet.core/Helpers/RetryHelper.cs), [fork initialization](https://github.com/codebeltnet/coverlet/commit/2515033182429c537ee25bd413ee1aaf55164619) |
| Shared-framework resolution | Parse runtime directory versions when selecting a matching framework. Version suffixes or unrelated nonnumeric directories must not prevent resolution; stable framework requests continue to select stable versions. | [Resolver](src/coverlet.core/Instrumentation/CecilAssemblyResolver.cs), [regression tests](test/coverlet.core.tests/Instrumentation/InstrumenterTests.cs), [fork fix](https://github.com/codebeltnet/coverlet/commit/55d2aa1567475976a15418bc162d8e8562e64007) |
| Tracker unload synchronization | Use the full hit-file name, including the session identifier, for the mutex name so unrelated sessions do not collide through an assembly-name prefix. | [Tracker](src/coverlet.core/Instrumentation/ModuleTrackerTemplate.cs), [maintenance PR #4](https://github.com/codebeltnet/coverlet/pull/4) |
| In-process diagnostics | Catch failures while obtaining an assembly name for diagnostics so that the diagnostic lookup cannot abort coverage flushing. | [In-process handler](src/coverlet.MTP/InProcess/CoverletInProcessHandler.cs), [fork initialization](https://github.com/codebeltnet/coverlet/commit/2515033182429c537ee25bd413ee1aaf55164619) |
| Test expectations | Pattern-matching branch assertions follow the selected SDK's emitted IL while retaining equality with the equivalent logical OR expression and requiring both reported paths to be covered. | [Symbol tests](test/coverlet.core.tests/Symbols/CecilSymbolHelperTests.cs), [coverage test](test/coverlet.core.coverage.tests/Coverage/CoverageTests.PatternMatchingOr.cs), [adaptation](https://github.com/codebeltnet/coverlet/commit/f6f030c66bdea899a70612a618749446edde159d) |

The `netstandard2.0` embedded tracker in `src/coverlet.template` is retained from upstream. Branch reachability and local-function instrumentation described below are upstream changes, rather than independently introduced Codebelt coverage rules.

## Synchronization history

### 2026-11-03: exception-handler instrumentation and dynamic exclusion fixes

Operation date: 2026-11-03. Reviewed `coverlet-coverage/coverlet:master` at [`f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a`](https://github.com/coverlet-coverage/coverlet/commit/f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a), fetched without tags. The shared comparison base is [`1d9db3aacfbf61e1b2b4d9190715a13ba694d8c9`](https://github.com/coverlet-coverage/coverlet/commit/1d9db3aacfbf61e1b2b4d9190715a13ba694d8c9); the requested branch baseline is `feef6ea0687684ecadc0bd147cc4d6a42f82f93f`. At the maintainer's explicit request this synchronization targets the existing `v11.0.0/double-digits-chore` branch, rather than starting a new branch from or integrating into `master`.

| Upstream source | Disposition |
| --- | --- |
| [`483ff5a`, #2067](https://github.com/coverlet-coverage/coverlet/commit/483ff5ac667a2d20fc4f3976251aa8f5335cc9ba) | Adopt exception-handler-aware branch trampolines, local trampoline fallback, stable iteration over original IL, and Issue 2058 regression coverage/samples. Preserve the fork's solution layout. Exclude upstream-only ILVerify tool manifests, legacy pipeline/tool configuration, and Copilot instruction edits. |
| [`345b63c`, #2068](https://github.com/coverlet-coverage/coverlet/commit/345b63cc18abe9587e353e75f882b59cbf3e372f) | Translate the adopted instrumentation fix into the root changelog; omit upstream release headings and removed package identities. |
| [`f37bf3d`, #2069](https://github.com/coverlet-coverage/coverlet/commit/f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a) | Adopt exact assembly-name filters for loaded controller assemblies, retain wildcard normalization for deps.json entries, accept hyphens in filters, and adopt corresponding core/MTP tests and retained MTP usage documentation. Translate release notes into the root changelog. |

All three incoming commits were reviewed; none was already present. Production and test changes match the reviewed upstream changes, including the .NET Standard trampoline compatibility path. The four modified/deleted or content-conflicted paths and all automatic merge results were reviewed individually. Removed tool manifests, legacy projects/pipelines, upstream changelog and versioning remain excluded. Codebelt package/assembly identities, `netstandard2.0;net10.0;net11.0`, runtime pins, MinVer `v*` tags, and release guards are preserved.

Validation on Windows with SDK `11.0.100-rc.3.26522.148`: `dotnet build coverlet.slnx -c Release -p:PrepareValidationPackage=true` succeeded with zero warnings/errors. Core and MTP compiled for .NET Standard 2.0, .NET 10 and .NET 11; isolated validation packages were created for the .NET 10/11 consumer tests. Inspection of both packages confirmed `Codebelt.Coverlet.MTP`, `coverlet.MTP.dll`, all three framework/dependency groups, the existing runtime pins and compatibility overrides, and Microsoft.Testing.Platform 2.5.2. MinVer calculated `10.1.2-alpha.0.4` before this merge commit; its configuration and release guards are unchanged and the guards were not executed locally.

With explicit approval, Release tests ran with `dotnet test test/<project>/<project>.csproj -c Release -f <net10.0|net11.0> --no-build -p:PrepareValidationPackage=false -- --report-trx --report-trx-filename <project>.<framework>.sync-20261103.trx`. All eight runs passed; package-validation framework runs were sequential to avoid shared generated-project directories.

| Project | .NET 10 | .NET 11 |
| --- | --- | --- |
| `coverlet.core.tests` | 374 passed, 9 skipped | 374 passed, 9 skipped |
| `coverlet.core.coverage.tests` | 102 passed, none skipped | 102 passed, none skipped |
| `coverlet.MTP.tests` | 434 passed, 7 skipped | 434 passed, 7 skipped |
| `coverlet.MTP.validation.tests` | 45 passed, none skipped | 45 passed, none skipped |

Skips require Linux or macOS. The four Issue 2058 JIT-preparation regressions passed on each runtime. .NET Standard 2.0 was built and packed, not executed as a test target. The normal merge carrying this entry records the branch baseline first and the pinned upstream tip second; the reviewed tip is an ancestor of the resulting branch with zero commits on the upstream side of its comparison. Destination integration into `master` and Linux/Windows/macOS CI remain pending; this operation neither releases nor publishes a package.

### 2026-11-01: Microsoft Testing Platform patch update

Reviewed `coverlet-coverage/coverlet:master` at [`1d9db3a`](https://github.com/coverlet-coverage/coverlet/commit/1d9db3aacfbf61e1b2b4d9190715a13ba694d8c9), compared from shared base [`2e0a852`](https://github.com/coverlet-coverage/coverlet/commit/2e0a852d3a0d9669d9c19445e366c69ccaaf6424). The requested branch started at `0e2e2fe394c2ea6343b9b228382f1dbd2ef165c3`. This normal merge records the four reviewed commits in ancestry; that does not adopt their excluded files.

| Upstream source | Disposition |
| --- | --- |
| [`1d9db3a`, #2066](https://github.com/coverlet-coverage/coverlet/commit/1d9db3aacfbf61e1b2b4d9190715a13ba694d8c9) | Adopt the shared Microsoft Testing Platform version update from 2.5.1 to 2.5.2 for the retained package and test dependencies, preserving the fork's central version layout. |
| [`b0b4190`, #2062](https://github.com/coverlet-coverage/coverlet/commit/b0b4190c995017d28c8b2cca5cad4f2d919591ff) | Omit the update to the removed upstream dotnet tool manifest. The separate `ReportGenerator.Core` test dependency was not changed upstream. |
| [`98d291d`, #2061](https://github.com/coverlet-coverage/coverlet/commit/98d291d713dde46a58ee1dcf7ce832bdf6c2b781) | Omit the update to the removed upstream CI workflow. |
| [`ea74a47`, #2060](https://github.com/coverlet-coverage/coverlet/commit/ea74a473c5c0f5ef7dffb425584da9a72a1769dd) | Omit the update to the removed upstream CodeQL workflow. |

The five centrally versioned Microsoft Testing Platform packages were verified at 2.5.2 via `dotnet package search`. On Windows with SDK `11.0.100-rc.3.26522.148`, solution restore and Release build with `PrepareValidationPackage=false` succeeded; the build had zero warnings and errors and compiled core and MTP for .NET Standard 2.0, .NET 10 and .NET 11. `dotnet pack` succeeded, and package inspection confirmed `Codebelt.Coverlet.MTP`, all three framework groups, their existing runtime pins and compatibility overrides, and Microsoft.Testing.Platform 2.5.2. MinVer calculated `10.1.2-alpha.0.3` before the merge commit. The release workflow guards were unchanged and were not executed locally. No release or publication was performed.

With approval, Windows Release tests passed against the merged source `feef6ea0687684ecadc0bd147cc4d6a42f82f93f`. For each of .NET 10 and .NET 11: `coverlet.core.tests` passed 369 and skipped 9; `coverlet.core.coverage.tests` passed 98; `coverlet.MTP.tests` passed 433 and skipped 7; and `coverlet.MTP.validation.tests` passed 45. Skips were platform-specific tests for Linux or macOS. The validation projects were rebuilt with `PrepareValidationPackage=true`, creating isolated packages at version `10.1.2-alpha.0.4`; their framework runs were sequential. .NET Standard 2.0 was built and packed, not executed as a test target. The Linux/Windows/macOS CI matrix remains required before integration.

### 2026-11-01: retain reviewed upstream ancestry without runtime changes

Operation date: 2026-11-01. Source: `coverlet-coverage/coverlet`, `master`, pinned at [`2e0a852d3a0d9669d9c19445e366c69ccaaf6424`](https://github.com/coverlet-coverage/coverlet/commit/2e0a852d3a0d9669d9c19445e366c69ccaaf6424). Fetching without tags confirmed that this is still the previously reviewed upstream tip. The starting fork baseline is [`6e72c0c11523bf3d6baf3ca25c2a457ce547674e`](https://github.com/codebeltnet/coverlet/commit/6e72c0c11523bf3d6baf3ca25c2a457ce547674e), after integrating the green [PR #6](https://github.com/codebeltnet/coverlet/pull/6); its tree was verified identical to the tested head `0b04b05d6d996ac945869d0f9e5e96224246d309`. The pre-merge comparison base is `da5c649126c74985fc20c496bc898f143d18e21b`.

Normal merge [`d587f8ca285f0bf147fb933e14ed48dfff8a7490`](https://github.com/codebeltnet/coverlet/commit/d587f8ca285f0bf147fb933e14ed48dfff8a7490) has the fork baseline as its first parent and the pinned upstream tip as its second parent. It records all 19 incoming commits as ancestors. Its complete tree is identical to its first parent's tree: applicable changes were already selectively ported, and this operation introduces no production, test, dependency, build, or package-layout changes. The following decisions cover every source commit, including the deliberately excluded changes now recorded in ancestry.

| Upstream source | Disposition in this merge |
| --- | --- |
| [`23578d4`, #2030](https://github.com/coverlet-coverage/coverlet/commit/23578d4), [`d212822`, #2032](https://github.com/coverlet-coverage/coverlet/commit/d212822), [`c6d3894`, #2033](https://github.com/coverlet-coverage/coverlet/commit/c6d3894) | Already present: MTP configuration selection, shared-framework fallback, and async exclusion regression coverage, with Codebelt resolver and test-host adaptations retained. |
| [`10a8051`, #2034](https://github.com/coverlet-coverage/coverlet/commit/10a8051), [`be3d696`, #2035](https://github.com/coverlet-coverage/coverlet/commit/be3d696) | Exclude upstream release/versioning files; retain MinVer, Codebelt release guards, root changelog, and focused documentation. |
| [`8b7654e`](https://github.com/coverlet-coverage/coverlet/commit/8b7654e) | Exclude upstream issue-template edits, as in the earlier selective review. |
| [`61e3350`, #2038](https://github.com/coverlet-coverage/coverlet/commit/61e3350) | Already present: central config-file and OpenCover tests. Retain Codebelt package preparation and test TFMs. |
| [`3142be5`, #2044](https://github.com/coverlet-coverage/coverlet/commit/3142be5) | Already present: Moq 4.21.0. |
| [`b330a1f`, #2043](https://github.com/coverlet-coverage/coverlet/commit/b330a1f), [`45e98f9`, #2039](https://github.com/coverlet-coverage/coverlet/commit/45e98f9) | Exclude SBOM and upstream CodeQL tooling outside the retained fork surface. |
| [`ecd6fd5`, #2046](https://github.com/coverlet-coverage/coverlet/commit/ecd6fd5) | Already present: local-function instrumentation fix and regression test. |
| [`221483a`, #2049](https://github.com/coverlet-coverage/coverlet/commit/221483a) | Exclude documentation for the removed console/global-tool driver. |
| [`710f301`, #2050](https://github.com/coverlet-coverage/coverlet/commit/710f301) | Applicable updates already adapted; preserve per-TFM runtime pins and .NET Standard overrides. Exclude removed drivers, benchmarks, and pipeline dependencies; omit the additional BCL compatibility reference in validation tests because these target .NET 10/11. |
| [`a924cc3`](https://github.com/coverlet-coverage/coverlet/commit/a924cc3) | Already present: validation-project PKCS reference; remove the duplicate produced by automatic merging. |
| [`d9e4357`, #2051](https://github.com/coverlet-coverage/coverlet/commit/d9e4357) | Already present: threshold/report diagnostics and resolver tests. |
| [`b5ba919`, #2055](https://github.com/coverlet-coverage/coverlet/commit/b5ba919) | Exclude legacy configurations and target-specific test dependency overrides; retain the already-adapted .NET Standard NuGet.Versioning 6.14.3 override and current test dependencies. |
| [`39f6b6f`, #2056](https://github.com/coverlet-coverage/coverlet/commit/39f6b6f) | Already present: branch reachability, serialization compatibility, reporters, and regression coverage. |
| [`870b835`, #2057](https://github.com/coverlet-coverage/coverlet/commit/870b835) | Exclude upstream changelog/release-plan files; retain the earlier translated release outcomes in the root changelog. |
| [`2e0a852`, #2059](https://github.com/coverlet-coverage/coverlet/commit/2e0a852) | Applicable .NET 10 dependency updates already present. Preserve the fork's .NET 11 SDK/runtime pins, .NET Standard overrides, and target layout; exclude legacy build/runtime files. |

Resolution review covered all 19 conflicted paths and all automatic changes. Removed legacy projects, pipelines, documentation, CodeQL workflow, and `version.json` remain absent. Automatic merging also duplicated the already-ported async regression method and introduced duplicate/legacy dependency references; these were resolved individually to the existing baseline. All intentional behavior differences in the table above remain intact, as does the embedded tracker.

The maintainer authorized completing this downstream-only workflow after the merge-policy and validation prerequisites were explained on 2026-11-01. All writes are confined to `codebeltnet/coverlet`. The fork's merge policy is to allow merge commits and remove the incompatible linear-history requirement while retaining the other PR, deletion, and force-push protections. The synchronization PR must use a merge commit; squash/rebase would lose the new upstream ancestry. Existing release tags are preserved; no release or package publication is requested.

Local validation on Windows used the repository SDK `11.0.100-rc.3.26522.148`. `dotnet build coverlet.slnx -c Release -p:PrepareValidationPackage=false` passed with zero warnings/errors, and `dotnet pack src/coverlet.MTP/coverlet.MTP.csproj -c Release --no-build` succeeded. The resulting `Codebelt.Coverlet.MTP.10.1.2-alpha.0.3.nupkg` has the retained assembly/discovery identity, three TFM/dependency groups, .NET 10/11 runtime pins, and .NET Standard compatibility overrides; no removed driver assets returned. MinVer uses the existing `v10.1.1` boundary and produces a prerelease version, not its `0.0.0-alpha` fallback. All local tag names and targets were compared with the pre-operation snapshot and are unchanged.

Approved local test commands use `dotnet test test/<project>/<project>.csproj -c Release -f <net10.0|net11.0> --no-build -p:PrepareValidationPackage=false -- --report-trx --report-trx-filename <project>.<framework>.trx`.

| Project | .NET 10 | .NET 11 |
| --- | --- | --- |
| `coverlet.core.tests` | 369 passed, 9 skipped | 369 passed, 9 skipped |
| `coverlet.core.coverage.tests` | 98 passed, none skipped | 98 passed, none skipped |
| `coverlet.MTP.tests` | 433 passed, 7 skipped | 433 passed, 7 skipped |
| `coverlet.MTP.validation.tests` | 45 passed, none skipped | 45 passed, none skipped on isolated full rerun |

The initial concurrent .NET 10/11 package-validation runs had one .NET 11 restore failure in `ConfigurationFile_IncludeFilters_CanBeConfigured` (44 passed, 1 failed). The unchanged [test harness](test/coverlet.MTP.validation.tests/TestInfrastructure/MtpValidationTestBase.cs) recreates solution directories under a configuration/test name shared by both frameworks; one run can delete files used by the other. After the .NET 10 run completed, a full .NET 11 rerun passed all 45 tests without any code, dependency, or assertion changes. Run package-validation framework legs sequentially within a worktree, as CI already does by using separate matrix jobs. .NET Standard 2.0 was built and packed, not executed as a test target. No release or package publication was performed.

Cross-platform validation: [CI run 41267824916](https://github.com/codebeltnet/coverlet/actions/runs/41267824916), source `1ddc5ab8bb25e437ebd3207769023e88c4a035e5`, completed successfully on 2026-11-01. All 18 validation/build/package jobs succeeded; `publish-nuget` was skipped for the PR. This verifies the retained tests and packaged consumer behavior on .NET 10/11 across Linux, Windows, and macOS. A documentation-only follow-up records these outcomes and remains subject to the same PR checks. Integration is through [PR #7](https://github.com/codebeltnet/coverlet/pull/7) into this fork's `master` using a merge commit; that PR records the actual destination commit and merged state. The reviewed tip is an ancestor of the PR branch, and GitHub's comparison reports zero commits behind upstream. Destination ancestry is checked again after integration. No production behavior, dependency, test assertion, or release tag was changed by this synchronization.

### 2026-10-30: selective core and MTP update

Reviewed upstream changes from the previously inspected snapshot [`be3d696`](https://github.com/coverlet-coverage/coverlet/commit/be3d696) through [`2e0a852`](https://github.com/coverlet-coverage/coverlet/commit/2e0a852d3a0d9669d9c19445e366c69ccaaf6424). The shared ancestry used to check already-ported changes was [`da5c649126c74985fc20c496bc898f143d18e21b`](https://github.com/coverlet-coverage/coverlet/commit/da5c649126c74985fc20c496bc898f143d18e21b). Source commits were ported selectively; their hashes need not appear as ancestors in this fork.

| Upstream source | Disposition | Result in this fork |
| --- | --- | --- |
| [`61e3350`, PR #2038](https://github.com/coverlet-coverage/coverlet/commit/61e335032fae02bfbd56392fdf5ca4be1aa5ad67) | Ported tests | Cover central `--config-file` selection and OpenCover report output. Preserve the fork's generated-project TFM and diagnostic/result directories. Included in [`06be507`](https://github.com/codebeltnet/coverlet/commit/06be507d84ddda8b1abf508ae503eeb3aa4b8b34). |
| [`ecd6fd5`, PR #2046](https://github.com/coverlet-coverage/coverlet/commit/ecd6fd5533e77880ab36748edb4f892df642a7dd) | Ported fix and regression test | Instrument local functions when compiler-generated attributes are excluded, while honoring other configured exclusions. Code in [`362dddf`](https://github.com/codebeltnet/coverlet/commit/362dddff57ba9070b0ca1356e0256eb30755e75f); tests in [`06be507`](https://github.com/codebeltnet/coverlet/commit/06be507d84ddda8b1abf508ae503eeb3aa4b8b34). |
| [`d9e4357`, PR #2051](https://github.com/coverlet-coverage/coverlet/commit/d9e43575632585bf48c8e4b0247880f460192768) | Ported tests | Add MTP threshold/report-message checks and shared-framework resolver checks. Included in [`06be507`](https://github.com/codebeltnet/coverlet/commit/06be507d84ddda8b1abf508ae503eeb3aa4b8b34). |
| [`39f6b6f`, PR #2056](https://github.com/coverlet-coverage/coverlet/commit/39f6b6fa9dd9761a6e9a8d9ffb31a44fc342162d) | Ported feature and regression tests | Compute branch reachability from method control flow, carry it through instrumentation/serialization, and exclude unreachable branches from coverage totals and Cobertura, LCOV, and OpenCover details. Older serialized instrumentation state defaults branches to reachable. Code in [`362dddf`](https://github.com/codebeltnet/coverlet/commit/362dddff57ba9070b0ca1356e0256eb30755e75f); tests in [`06be507`](https://github.com/codebeltnet/coverlet/commit/06be507d84ddda8b1abf508ae503eeb3aa4b8b34). |
| [`3142be5`, PR #2044](https://github.com/coverlet-coverage/coverlet/commit/3142be50ab2a46721f7d0bef424a38d2b83c4017), [`710f301`, PR #2050](https://github.com/coverlet-coverage/coverlet/commit/710f30144157ef19933f3f0ec48464874205e264), [`2e0a852`, PR #2059](https://github.com/coverlet-coverage/coverlet/commit/2e0a852d3a0d9669d9c19445e366c69ccaaf6424) | Adapted dependency updates | Update retained test/MTP/NuGet dependencies and the .NET Standard NuGet.Versioning override. Preserve this fork's SDK and .NET 10/11 runtime pins. The .NET 10 runtime pin already matched upstream. Included in [`a6f6dfb`](https://github.com/codebeltnet/coverlet/commit/a6f6dfb43618f46580c98cbcb8a49c4943a1d205). |
| [`a924cc3`](https://github.com/coverlet-coverage/coverlet/commit/a924cc3714eb9a4a02abb777231503440ae53a27) | Already present | The validation project's `System.Security.Cryptography.Pkcs` reference was already included by [maintenance PR #4](https://github.com/codebeltnet/coverlet/pull/4). |
| [`b5ba919`, PR #2055](https://github.com/coverlet-coverage/coverlet/commit/b5ba9197f032e22627836d899aa3956a50cd5017) | Omitted legacy configuration | `LEGACY` build switches and their package selections do not apply to the retained target layout. The compatible NuGet.Versioning update from #2050 was ported separately. |
| [`b330a1f`, PR #2043](https://github.com/coverlet-coverage/coverlet/commit/b330a1f) and [`45e98f9`, PR #2039](https://github.com/coverlet-coverage/coverlet/commit/45e98f9) | Omitted tooling outside the retained surface | This fork does not retain the upstream SBOM package or CodeQL workflow. |
| [`221483a`, PR #2049](https://github.com/coverlet-coverage/coverlet/commit/221483a) | Omitted console documentation | Global-tool installation/PATH guidance concerns the upstream console driver. |
| [`8b7654e`](https://github.com/coverlet-coverage/coverlet/commit/8b7654e), [`870b835`, PR #2057](https://github.com/coverlet-coverage/coverlet/commit/870b835), and release/build-file portions of the commits above | Omitted or translated | Upstream issue-template, release-plan, changelog, versioning, and pipeline changes were not imported. Ported release outcomes are recorded in the root [CHANGELOG.md](CHANGELOG.md). |

Validation: dependency restore, Release solution build, and package creation succeeded. Package inspection confirmed `Codebelt.Coverlet.MTP` and the three retained TFM/dependency groups. The [CI run at `ec345d4`](https://github.com/codebeltnet/coverlet/actions/runs/41077811675), verified on 2026-10-31, passed the full retained test matrix and package validation. This is evidence for the PR branch; it does not claim a published 11.0.0 package or test execution on .NET Standard 2.0.

### 2026-10-20: v10.1.1 dependency alignment and Codebelt maintenance

The range [`v10.1.0...v10.1.1`](https://github.com/codebeltnet/coverlet/compare/v10.1.0...v10.1.1) was integrated by [PR #4](https://github.com/codebeltnet/coverlet/pull/4), destination commit [`5bd8648`](https://github.com/codebeltnet/coverlet/commit/5bd86483a104328783255960116c1fb87ce2b35d), which is also the `v10.1.1` tag target. This was a mixture of dependency alignment and fork-specific fixes, rather than another full upstream merge.

| Change | Provenance and disposition |
| --- | --- |
| .NET 10/9 runtime pins changed from 10.0.12/9.0.20 to 10.0.13/9.0.21 | Aligns with the versions in upstream [`710f301`, PR #2050](https://github.com/coverlet-coverage/coverlet/commit/710f30144157ef19933f3f0ec48464874205e264), adapted to the fork's per-TFM version properties. This records matching changes; the integration commit does not carry an upstream cherry-pick trailer. |
| Add `System.Security.Cryptography.Pkcs` to the validation project | Matches upstream [`a924cc3`](https://github.com/coverlet-coverage/coverlet/commit/a924cc3714eb9a4a02abb777231503440ae53a27), using the fork's central runtime version. |
| Raise the core .NET Standard PKCS override from 8.0.1 to 8.0.2 | Codebelt compatibility/dependency maintenance recorded in the integration diff. |
| Use the full hit-file name in the unload mutex | Codebelt behavior fix with a regression test for collisions between session filenames sharing an assembly prefix. This remains an intentional difference from the reviewed upstream snapshot. |
| Read dependency versions in package assertions and change CI runner selections | Codebelt testing/tooling maintenance; excluded from claims about imported upstream coverage features. |

Historical validation: the [PR #4 head CI run](https://github.com/codebeltnet/coverlet/actions/runs/39473328299) completed successfully. Its source state was [`0733a28`](https://github.com/codebeltnet/coverlet/commit/0733a287b7645eb3ae2561fac735b0cf85d6a8a8); the tagged integration commit is listed separately above.

### 2026-09-28: v10.1.0 MTP configuration and instrumentation fixes

Reconstructed from [PR #3](https://github.com/codebeltnet/coverlet/pull/3), destination and `v10.1.0` tag commit [`91c8882`](https://github.com/codebeltnet/coverlet/commit/91c888280969bb23e95bbc471126714915994b2c), and the range [`v10.0.1...v10.1.0`](https://github.com/codebeltnet/coverlet/compare/v10.0.1...v10.1.0). The operation date comes from the integration commit; the release date in the changelog is separate.

| Upstream source | Ported result |
| --- | --- |
| [`23578d4`, PR #2030](https://github.com/coverlet-coverage/coverlet/commit/23578d4) | Honor the MTP `--config-file` command-line argument. |
| [`d212822`, PR #2032](https://github.com/coverlet-coverage/coverlet/commit/d212822) | Resolve shared-framework assemblies absent from `compileLibraries`, with regression coverage. |
| [`c6d3894`, PR #2033](https://github.com/coverlet-coverage/coverlet/commit/c6d3894) | Add regression coverage for async state machines with compiler-generated attribute exclusions. |

Upstream release/version files were excluded. Historical validation: the [PR #3 head CI run](https://github.com/codebeltnet/coverlet/actions/runs/36483200835) completed successfully at [`b9ea944`](https://github.com/codebeltnet/coverlet/commit/b9ea944b6d7961278e3ca227bb94cb2ee6e3ec75). This identifies the tested PR source state separately from the tagged integration commit.

### 2026-09-20: v10.0.1 inherited upstream state and Codebelt baseline

[PR #1](https://github.com/codebeltnet/coverlet/pull/1), destination commit [`2515033`](https://github.com/codebeltnet/coverlet/commit/2515033182429c537ee25bd413ee1aaf55164619), established the narrowed package surface and Codebelt tooling on top of upstream [`da5c649`](https://github.com/coverlet-coverage/coverlet/commit/da5c649126c74985fc20c496bc898f143d18e21b). [PR #2](https://github.com/codebeltnet/coverlet/pull/2), commit [`9766806`](https://github.com/codebeltnet/coverlet/commit/97668061b72e03668de55d6cca39a559c1d3ff10), then added version/trust validation and release guards; `v10.0.1` points to that second commit.

The inherited upstream range is [`7fbbe4e...da5c649`](https://github.com/coverlet-coverage/coverlet/compare/7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17...da5c649126c74985fc20c496bc898f143d18e21b). The retained implementation includes these source changes; they were inherited through ancestry, not newly cherry-picked on 2026-09-20:

| Upstream source | Retained behavior or test coverage |
| --- | --- |
| [`5654ffe`, #1938](https://github.com/coverlet-coverage/coverlet/commit/5654ffe) | Handle field references when detecting delegate-cache branches. |
| [`3a241bc`, #1940](https://github.com/coverlet-coverage/coverlet/commit/3a241bc) | Regression coverage for `DoesNotReturn` handling in async state machines. |
| [`c502253`, #1946](https://github.com/coverlet-coverage/coverlet/commit/c502253) and [`7f565ef`, #1958](https://github.com/coverlet-coverage/coverlet/commit/7f565ef) | MTP dynamic assembly exclusion filters and the ReportGenerator baseline exclusion. |
| [`6dbe06d`, #1941](https://github.com/coverlet-coverage/coverlet/commit/6dbe06d) | Auto-property skipping and record coverage improvements. |
| [`c962ca2`, #1967](https://github.com/coverlet-coverage/coverlet/commit/c962ca2) and [`1c2fc5d`, #2027](https://github.com/coverlet-coverage/coverlet/commit/1c2fc5d) | Instrumentation preflight checks and the later correction to unresolvable-dependency handling. |
| [`b68bf4f`, #1977](https://github.com/coverlet-coverage/coverlet/commit/b68bf4f) and [`330b1af`, #1979](https://github.com/coverlet-coverage/coverlet/commit/330b1af) | Pattern-matching branch detection and regression coverage. SDK-specific assertion updates are listed under intentional differences above. |
| [`f549c8b`, #1980](https://github.com/coverlet-coverage/coverlet/commit/f549c8b) | .NET Framework assembly-resolution improvements on Windows. |
| [`79dff97`, #1985](https://github.com/coverlet-coverage/coverlet/commit/79dff97) | Embed a .NET Standard tracker to preserve instrumentation compatibility with .NET Framework. |
| [`68e4db0`, #1987](https://github.com/coverlet-coverage/coverlet/commit/68e4db0), [`856637d`, #1988](https://github.com/coverlet-coverage/coverlet/commit/856637d), and [`e623ddc`, #2010](https://github.com/coverlet-coverage/coverlet/commit/e623ddc) | Tracker flush/unload handling and the corresponding MTP handler/registry changes. |
| [`eeb4310`, #2007](https://github.com/coverlet-coverage/coverlet/commit/eeb4310) | Fail explicitly when an instrumentation resource stream is unavailable. |
| [`4faeea2`, #2019](https://github.com/coverlet-coverage/coverlet/commit/4faeea2) | MTP coverage thresholds, exit-code handling, and coverage-message publishing. |
| [`4da2c9f`, #1936](https://github.com/coverlet-coverage/coverlet/commit/4da2c9f) and [`f9e6c86`, #2008](https://github.com/coverlet-coverage/coverlet/commit/f9e6c86) | MTP validation and internal regression-test infrastructure, adapted to the retained project set. |

The 53-commit range also contains upstream dependency, documentation, benchmark, console, legacy-driver, and automation changes. Inherited history does not mean those surfaces remain available: PR #1 removed the console/collector/MSBuild projects and associated examples, broad documentation, benchmarks, upstream automation, and Nerdbank versioning. It established Codebelt package naming, per-TFM dependencies, the `netstandard2.0;net9.0;net10.0` layout used at that time, and Codebelt workflows. Existing fork restoration/diagnostic adaptations are listed above. [The full Codebelt baseline comparison](https://github.com/codebeltnet/coverlet/compare/da5c649126c74985fc20c496bc898f143d18e21b...97668061b72e03668de55d6cca39a559c1d3ff10) shows that narrowing and those adaptations.

Historical validation: the [PR #1 head CI run](https://github.com/codebeltnet/coverlet/actions/runs/35518499258) succeeded at [`8c58545`](https://github.com/codebeltnet/coverlet/commit/8c585456041b54b73fa51dae615f97e88ec0f410). The subsequent [PR #2 head run](https://github.com/codebeltnet/coverlet/actions/runs/35523365224), at [`5c3fd90`](https://github.com/codebeltnet/coverlet/commit/5c3fd90164c410a6c4093b39bc61ed839b754ae1), failed during checkout before compilation or tests. No successful CI result is claimed here for that exact tag state. Later release validations are recorded separately.

### Original upstream marker: v10.0.1-coverlet

[`v10.0.1-coverlet`](https://github.com/codebeltnet/coverlet/tree/v10.0.1-coverlet) points to upstream commit [`7fbbe4e`](https://github.com/coverlet-coverage/coverlet/commit/7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17), whose source commit date is 2026-05-18. It records the original starting point, not a Codebelt release or the later shared comparison base. The lightweight tag does not establish when the fork was created or when the tag was applied. Original upstream history remains available in Git.

## Keeping this record current

Every sync must update this document in the same change set, as required by [AGENTS.md](AGENTS.md#upstream-sync-workflow). Record the operation date, exact upstream snapshot and comparison boundary, source-to-destination mapping, adaptations and exclusions, intentional behavior differences, and actual validation outcomes. Clearly mark deferred work or pending validation and update those statuses when completed. Preserve previous dated entries. Releases remain documented in `CHANGELOG.md`; this ledger tracks provenance and decisions.
