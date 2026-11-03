# Upstream provenance

This ledger records the upstream origin and retained behavior relevant to the 10.1.2 hotfix branch. It is not a claim of a new upstream synchronization or a reconstruction of unreviewed history. Upstream is [coverlet-coverage/coverlet](https://github.com/coverlet-coverage/coverlet), branch `master`.

## Historical anchors

| Reference | Exact target | Meaning |
| --- | --- | --- |
| Original upstream marker `v10.0.1-coverlet` | `7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17` | Original marker retained by this fork; distinct from the later Codebelt release with a similar version number. |
| Codebelt `v10.0.1` | `97668061b72e03668de55d6cca39a559c1d3ff10` | Fork release whose source already includes the later upstream dependency-exclusion implementation. |
| Codebelt `v10.1.1` | `5bd86483a104328783255960116c1fb87ce2b35d` | Baseline used to compare this hotfix's implementation and unchanged package/framework declarations. |

These are verified commit targets. No tag-creation dates, earlier synchronization dates, or historical test outcomes are inferred from them. The 10.0.1 release date in the changelog is not a tag-creation timestamp.

## 2026-11-03: prepare the 10.x maintenance line

Operation date: 2026-11-03. The selected release strategy keeps `master` on the 11.0.0 development line and uses `releases/v10` for 10.x maintenance, matching the branch naming in the GitHub Actions versioning example. The maintenance branch was created at the exact `v10.1.1` target, `5bd86483a104328783255960116c1fb87ce2b35d`, with identical tree content, and renamed from the provisional local `release/10` to `releases/v10`. The branch is now published in `codebeltnet/coverlet`, and [PR #9](https://github.com/codebeltnet/coverlet/pull/9) targets it. The existing active branch ruleset was extended to include `refs/heads/releases/v10`, preserving the same pull-request, deletion, and non-fast-forward protections without changing the default branch's rules.

At inspection, `master` remains at `0e2e2fe394c2ea6343b9b228382f1dbd2ef165c3`, containing the earlier 11.0.0 work and ancestry-preserving upstream integration from PR #7. The existing `v11.0.0/double-digits-chore` branch, at `922b11cd71fccd97ce3fc8e369beb71fad4bb81d` and used by PR #8, contains that complete master history plus its subsequent work. Neither branch is reset, reverted, or rewritten by this maintenance preparation. Their detailed synchronization record remains in the 11.0.0 line's `UPSTREAM.md`.

PR #9's 13 existing commits are the hotfix work based on `v10.1.1`; the newer upstream and .NET 11 commits are already in `master`, not additional hotfix commits. Against the selected maintenance base, the hotfix has no merge conflicts and retains the tested 10.1.1 package/framework baseline. The workflow filter is extended to accept `releases/v10` as well as `master`. Future 10.x fixes must be forward-ported to the 11.0.0 line separately; no forward-port, upstream adoption, package release, or publication is performed by this branch preparation.

The existing manual workflow can run against `releases/v10` or a fixed release tag. The `Production` environment has no branch restriction and retains its required reviewer. A stable 10.1.2 release requires its fixed `v10.1.2` tag on the selected commit before the run; otherwise MinVer calculates a prerelease version. This setup does not create a release tag, approve deployment, or publish a package.

The earlier [manual run 41717154404](https://github.com/codebeltnet/coverlet/actions/runs/41717154404), on hotfix commit `a06eac4b88f682bdc838472bfc4b9b488d13f076`, passed the build, release-version guard, and all six Windows/Ubuntu test jobs. Its three macOS jobs were cancelled without acquiring a runner. Their annotations report hosted-runner acquisition failure and macOS arm64 capacity constraints; no macOS test execution occurred. Packing, package validation, and publication were skipped because the test matrix did not complete successfully. This is a partial CI result caused by runner allocation, not evidence of a failing macOS test or successful package validation.

Following the same queue behavior in [PR run 41722023562](https://github.com/codebeltnet/coverlet/actions/runs/41722023562), the user selected the opt-in macOS approach used by Codebelt xUnit. The new manual `run_mac_tests` boolean defaults to false. Windows and Ubuntu remain in both the test and package-validation matrices for every run; macOS joins both matrices when explicitly enabled. Existing matrix dependencies still block packing or publication if any selected job fails or is cancelled. The default run therefore does not validate macOS. This is a user-requested CI scheduling change, not an upstream integration or a product-support change.

## 2026-11-03: dependency exclusion correction for 10.1.2

Operation date: 2026-11-03. This is a fork-local correction of inherited behavior, not an upstream merge. The hotfix comparison base is Codebelt `v10.1.1` (`5bd86483a104328783255960116c1fb87ce2b35d`).

### Origin and source comparison

The defect originates in upstream [PR #1946](https://github.com/coverlet-coverage/coverlet/pull/1946), commit [`c5022539cfdf3c6246c2e49b7789d7e8e529ef47`](https://github.com/coverlet-coverage/coverlet/commit/c5022539cfdf3c6246c2e49b7789d7e8e529ef47). Its source-commit date is 2026-06-01. That implementation converts package IDs and loaded assembly names with at least three segments into wildcards after the second segment, allowing dependency exclusions to match project assemblies with the same prefix. It is inherited in the inspected Codebelt 10.0.1, 10.1.0, and 10.1.1 tag targets.

A targeted comparison also inspected upstream [PR #2069](https://github.com/coverlet-coverage/coverlet/pull/2069), commit [`f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a`](https://github.com/coverlet-coverage/coverlet/commit/f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a), source-commit date 2026-11-03. That snapshot narrows loaded-assembly filters and accepts hyphens, while retaining the package-name wildcard route for `deps.json`. This is a comparison of the relevant paths at a pinned commit, not a full review or integration of that upstream snapshot.

### Retained fork behavior and destination commits

The Codebelt correction resolves `deps.json` entries to their managed runtime assembly filenames, including multiple assemblies and RID-specific assets, and uses exact assembly filters for both discovery sources. Native assets and placeholders are skipped. Known xUnit build assets without runtime entries retain exact default exclusions. Hyphenated assembly filters are valid. Existing CLI/configuration precedence and intentionally broad permanent baseline filters remain in effect.

Consumer verification also found SDK metadata listing the same runner DLL as both a project and a reference. The follow-up in [ProcessAssemblyHelper](src/coverlet.core/Helpers/ProcessAssemblyHelper.cs) gives project runtime assets precedence over duplicate dependency entries, with regression coverage in the discovery test. It is recorded in destination commit `c65e2d357204e573eae59401a619252da6ec8075`.

This differs from the inspected upstream `f37bf3d` dependency-filter behavior because package-name wildcards still remove unrelated project modules. The reason and regression evidence are documented in the [coverage advisory](Documentation/DependencyExclusionRegression.md), [discovery regression](test/coverlet.MTP.tests/Configuration/CoverageConfigurationDependencyDiscoveryTests.cs), and [filter tests](test/coverlet.core.tests/Helpers/InstrumentationHelperTests.cs).

| Destination commit in this branch | Role |
| --- | --- |
| `86dba83dcbdd11054e7a6f6ab398ca4124c755c2` | Implements exact runtime-asset exclusions, retained xUnit build exclusions, filter validation, and regression tests. |
| `c65e2d357204e573eae59401a619252da6ec8075` | Preserves project runtime assets also listed in non-project reference entries. |
| `13b23354cebef95040bb5218a9fe5eb6f1de35f4` | Updates the integration guide for the corrected discovery behavior. |
| `7fc6b11d4535d29f400399506b762a976caee7b8` | Records the initial 10.1.2 changelog draft; this advisory expands that draft. |

No other changes from upstream #2069 are claimed as adopted. This operation does not merge or alter upstream ancestry, move tags, change package/assembly identities, alter dependencies, or introduce the planned major release's framework changes. This ledger's documentation additions remain pending until integrated by the repository's normal process.

### Validation and remaining work

- Windows x64: complete MTP tests passed 427 tests with seven platform skips on each of `net9.0` and `net10.0`; focused core discovery/filter tests passed 121 tests on each framework.
- Build-only: `netstandard2.0` MTP/core build passed with zero warnings and errors.
- Windows x64 consumer comparisons: 21 .NET 10 test projects across BenchmarkDotNet, Carter, Globalization, ASP.Versioning, Swashbuckle.AspNetCore, Newtonsoft.Json, and AWS Signature V4 passed 1,943 tests with each collector. The released baseline omitted all 15 library modules represented by these runs; the corrected collector included them with visited sequence points and retained BenchmarkDotNet's runner coverage. See the advisory for the exact scope and per-repository totals.
- The initial Cuemon .NET Framework 4.8 comparison passed 729 tests with each collector but still failed framework assembly resolution for three library modules. The separate resolver correction and successful revalidation are recorded below.
- Fixed-consumer Linux/macOS and .NET 11 execution, installation of a newly packed package, remote CI for this hotfix, and package publication have not been established by these local checks. Historical CI/report inspection is recorded separately from fixed-collector validation in the advisory.

The original bug is recorded in inherited ancestry; the correction is implemented and locally tested in the destination commits above. That does not establish that a synchronization PR is merged, remote CI is green, or 10.1.2 has been released. No release or publish operation is authorized by this ledger.

## 2026-11-03: runtime facade resolver correction for 10.1.2

Operation date: 2026-11-03. This is a second fork-local correction using the same Codebelt `v10.1.1` comparison base (`5bd86483a104328783255960116c1fb87ce2b35d`), not an upstream synchronization. The earlier dependency-filter correction alone did not repair the Cuemon .NET Framework failure.

### Origin and retained behavior

The relevant source pattern is present in upstream [PR #625](https://github.com/coverlet-coverage/coverlet/pull/625), commit [`3856c76a5627a8d38d2cf2eae9360f05f7d878c5`](https://github.com/coverlet-coverage/coverlet/commit/3856c76a5627a8d38d2cf2eae9360f05f7d878c5), source-commit date 2019-12-03: the runtime `netstandard.dll` facade is read into a static Cecil assembly without its owning custom resolver. This is source provenance, not a claim that every package or workload since 2019 fails. The concrete missing-module failure was reproduced with released Codebelt 10.1.1 and the intermediate filter-only correction. It has a different origin from dependency-filter PR #1946.

The correction in [CecilAssemblyResolver](src/coverlet.core/Instrumentation/CecilAssemblyResolver.cs) loads the same runtime facade lazily per resolver with `ReaderParameters.AssemblyResolver` set to that resolver. Registering the facade in the base cache gives it the same disposal lifetime. Forwarded enum types can then use the existing framework runtime/reference-assembly fallbacks and configured search directories when Cecil writes optional-parameter metadata. Official facade identity checks, fallback order, CLI/configuration behavior, dependencies, and package targets are preserved. There are no Cuemon-specific production branches. [CecilAssemblyResolverTests](test/coverlet.core.tests/Instrumentation/CecilAssemblyResolverTests.cs) exercises real facade forwarding and optional enum metadata writing for two enum types; both configured-path assertions failed before the correction and pass after it.

This correction differs from the inherited static, unbound facade behavior for the reason and reproduction above. Its source and regression tests are in the working tree; no destination commit, upstream adoption, remote CI result, or released package is claimed yet. No upstream commits were merged or ancestry changed by this operation.

### Validation and limits

- Windows x64, isolated Cuemon `Cuemon.Core.Tests` on .NET Framework 4.8: all 729 tests passed before and after. Before the resolver correction, `Cuemon.Core`, `Cuemon.Kernel`, and `Cuemon.Extensions.Core` were missing after framework assembly-resolution failures. After it, there were no module-instrumentation failures, and those modules respectively contained 2,194/2,825, 465/1,245, and 205/692 visited/total sequence points. Exclusion settings and consumer assemblies were unchanged between runs; only the two collector DLLs were replaced.
- Windows x64, complete core suite: 351 tests passed with nine platform skips on each of `net9.0` and `net10.0`; complete MTP suite: 427 tests passed with seven platform skips on each framework.
- Build-only: MTP/core `netstandard2.0` build passed with zero warnings and errors.
- Compatibility rerun after the resolver correction: all 1,943 tests in the 21 .NET 10 consumer projects passed again. Per-project module names and visited/total sequence points were unchanged from their preceding filter-corrected reports, including BenchmarkDotNet's runner.
- Ubuntu remote verification: the current working tree was built and tested using the `dotnet-remote-testing` skill in `codebeltnet/ubuntu-testrunner:8-9-10-11`, pinned to `sha256:1df4b744ff2b2bf215fa2dc63e4f81b3c8dcf7c701c9bd456c78a2ec160713d9`. On each of .NET 9 and .NET 10, core passed 347 tests with 13 skips, MTP passed 417 with 17 skips, and coverage instrumentation passed 95 with no skips. All resolver and real dependency-discovery regressions passed. The complete solution result was 1,722 passed, 72 skipped, and 72 failed: all 72 failures were package-validation tests requiring the absent `/workspace/artifacts/package/debug` fixture. This establishes the core/MTP/instrumentation results, not successful packaged installation. Cumulative test time was 547.2 seconds; total wall time was 325.7 seconds. See the [Ubuntu validation record](Documentation/DependencyExclusionRegression.md#ubuntu-remote-validation-on-2026-11-03) for details and artifact locations.
- The Cuemon check covers one test project, not its entire framework suite. Snapshot provenance, build settings, raw-report locations, and remaining platform/package validation limits are recorded in the [coverage advisory](Documentation/DependencyExclusionRegression.md#separate-cuemon-net-framework-instrumentation-fix).
