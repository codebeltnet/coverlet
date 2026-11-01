# Upstream relationship and synchronization record

This repository is a selective fork of [coverlet-coverage/coverlet](https://github.com/coverlet-coverage/coverlet), maintained as `Codebelt.Coverlet.MTP`. Coverlet supplies the instrumentation engine and Microsoft Testing Platform integration. The Codebelt variant focuses on explicit target-framework support, package dependencies, and its development and release experience.

The compatibility goal is to preserve upstream behavior within the retained surface. Meaningful behavior differences are listed below; a synchronization is not a blanket merge of upstream history. This document records repository state, which can include work that has not yet been released. See [CHANGELOG.md](CHANGELOG.md) for release outcomes, [README.md](README.md) for installation and usage, and [AGENTS.md](AGENTS.md) for synchronization rules.

## Current reviewed state

| Item | Recorded state |
| --- | --- |
| Upstream | [`coverlet-coverage/coverlet`, `master`](https://github.com/coverlet-coverage/coverlet/tree/master) |
| Latest reviewed upstream commit | [`2e0a852d3a0d9669d9c19445e366c69ccaaf6424`](https://github.com/coverlet-coverage/coverlet/commit/2e0a852d3a0d9669d9c19445e366c69ccaaf6424), dated 2026-10-27 |
| Latest sync operation | 2026-10-30; record and CI verified on 2026-10-31 |
| Ported changes verified at | [`ec345d4ce9d928142efc5744927d339f9d42e31d`](https://github.com/codebeltnet/coverlet/commit/ec345d4ce9d928142efc5744927d339f9d42e31d) on [PR #6](https://github.com/codebeltnet/coverlet/pull/6) |
| Integration state at verification | Committed on the PR branch; PR open; 11.0.0 release entry is a draft |
| Validation | [CI run 41077811675](https://github.com/codebeltnet/coverlet/actions/runs/41077811675) succeeded: build, release-version guard, nine test jobs, packing, and six package-validation jobs across .NET 10/11 and Linux, Windows, and macOS. Publication was skipped for the PR. |

## Original marker and release tags

Tag targets below were checked against this repository's remote on 2026-10-31. The `-coverlet` marker is the original starting point supplied for the fork; it must not be confused with the later Codebelt tag of the same version. These tags identify repository commits, rather than proving that all upstream changes through a similarly named upstream release were imported.

| Tag | Commit | Date evidence | Meaning |
| --- | --- | --- | --- |
| [`v10.0.1-coverlet`](https://github.com/codebeltnet/coverlet/tree/v10.0.1-coverlet) | [`7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17`](https://github.com/coverlet-coverage/coverlet/commit/7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17) | Source commit dated 2026-05-18; tag creation date is not recorded by this lightweight tag | Original upstream starting marker. |
| [`v10.0.1`](https://github.com/codebeltnet/coverlet/tree/v10.0.1) | [`9766806`](https://github.com/codebeltnet/coverlet/commit/97668061b72e03668de55d6cca39a559c1d3ff10) | Integration commit dated 2026-09-20 | First Codebelt release boundary, including [PR #1](https://github.com/codebeltnet/coverlet/pull/1) and the versioning/release guards in [PR #2](https://github.com/codebeltnet/coverlet/pull/2). |
| [`v10.1.0`](https://github.com/codebeltnet/coverlet/tree/v10.1.0) | [`91c8882`](https://github.com/codebeltnet/coverlet/commit/91c888280969bb23e95bbc471126714915994b2c) | Integration commit dated 2026-09-28 | Selective MTP/configuration and instrumentation ports in [PR #3](https://github.com/codebeltnet/coverlet/pull/3). |
| [`v10.1.1`](https://github.com/codebeltnet/coverlet/tree/v10.1.1) | [`5bd8648`](https://github.com/codebeltnet/coverlet/commit/5bd86483a104328783255960116c1fb87ce2b35d) | Integration commit dated 2026-10-20 | Dependency alignment and Codebelt tracker-mutex maintenance in [PR #4](https://github.com/codebeltnet/coverlet/pull/4). |

Upstream history continued after the original marker before the first Codebelt release. The shared ancestor of the current fork and upstream is [`da5c649`](https://github.com/coverlet-coverage/coverlet/commit/da5c649126c74985fc20c496bc898f143d18e21b), dated 2026-09-17: it includes 53 upstream commits after `7fbbe4e`. Thus `7fbbe4e` is the starting marker, while `da5c649` is the inherited upstream boundary used for later comparisons. The first Codebelt release then narrowed that inherited tree. The chronology does not establish separate sync-operation dates for those 53 commits.

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
