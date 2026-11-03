# Upstream provenance

This ledger records the upstream origin and retained behavior relevant to the 10.1.2 hotfix branch. It is not a claim of a new upstream synchronization or a reconstruction of unreviewed history. Upstream is [coverlet-coverage/coverlet](https://github.com/coverlet-coverage/coverlet), branch `master`.

## Historical anchors

| Reference | Exact target | Meaning |
| --- | --- | --- |
| Original upstream marker `v10.0.1-coverlet` | `7fbbe4e72b06b0a6fe3932f65944c7627ecf6b17` | Original marker retained by this fork; distinct from the later Codebelt release with a similar version number. |
| Codebelt `v10.0.1` | `97668061b72e03668de55d6cca39a559c1d3ff10` | Fork release whose source already includes the later upstream dependency-exclusion implementation. |
| Codebelt `v10.1.1` | `5bd86483a104328783255960116c1fb87ce2b35d` | Baseline used to compare this hotfix's implementation and unchanged package/framework declarations. |

These are verified commit targets. No tag-creation dates, earlier synchronization dates, or historical test outcomes are inferred from them. The 10.0.1 release date in the changelog is not a tag-creation timestamp.

## 2026-11-03: dependency exclusion correction for 10.1.2

Operation date: 2026-11-03. This is a fork-local correction of inherited behavior, not an upstream merge. The hotfix comparison base is Codebelt `v10.1.1` (`5bd86483a104328783255960116c1fb87ce2b35d`).

### Origin and source comparison

The defect originates in upstream [PR #1946](https://github.com/coverlet-coverage/coverlet/pull/1946), commit [`c5022539cfdf3c6246c2e49b7789d7e8e529ef47`](https://github.com/coverlet-coverage/coverlet/commit/c5022539cfdf3c6246c2e49b7789d7e8e529ef47). Its source-commit date is 2026-06-01. That implementation converts package IDs and loaded assembly names with at least three segments into wildcards after the second segment, allowing dependency exclusions to match project assemblies with the same prefix. It is inherited in the inspected Codebelt 10.0.1, 10.1.0, and 10.1.1 tag targets.

A targeted comparison also inspected upstream [PR #2069](https://github.com/coverlet-coverage/coverlet/pull/2069), commit [`f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a`](https://github.com/coverlet-coverage/coverlet/commit/f37bf3d1ea8795cc1bdc57b29ad2ec81d10c9f1a), source-commit date 2026-11-03. That snapshot narrows loaded-assembly filters and accepts hyphens, while retaining the package-name wildcard route for `deps.json`. This is a comparison of the relevant paths at a pinned commit, not a full review or integration of that upstream snapshot.

### Retained fork behavior and destination commits

The Codebelt correction resolves `deps.json` entries to their managed runtime assembly filenames, including multiple assemblies and RID-specific assets, and uses exact assembly filters for both discovery sources. Native assets and placeholders are skipped. Known xUnit build assets without runtime entries retain exact default exclusions. Hyphenated assembly filters are valid. Existing CLI/configuration precedence and intentionally broad permanent baseline filters remain in effect.

Consumer verification also found SDK metadata listing the same runner DLL as both a project and a reference. The pending follow-up in [ProcessAssemblyHelper](src/coverlet.core/Helpers/ProcessAssemblyHelper.cs) gives project runtime assets precedence over duplicate dependency entries, with regression coverage in the discovery test. That follow-up is in the working tree; no destination commit is claimed for it yet.

This differs from the inspected upstream `f37bf3d` dependency-filter behavior because package-name wildcards still remove unrelated project modules. The reason and regression evidence are documented in the [coverage advisory](Documentation/DependencyExclusionRegression.md), [discovery regression](test/coverlet.MTP.tests/Configuration/CoverageConfigurationDependencyDiscoveryTests.cs), and [filter tests](test/coverlet.core.tests/Helpers/InstrumentationHelperTests.cs).

| Destination commit in this branch | Role |
| --- | --- |
| `86dba83dcbdd11054e7a6f6ab398ca4124c755c2` | Implements exact runtime-asset exclusions, retained xUnit build exclusions, filter validation, and regression tests. |
| `13b23354cebef95040bb5218a9fe5eb6f1de35f4` | Updates the integration guide for the corrected discovery behavior. |
| `7fc6b11d4535d29f400399506b762a976caee7b8` | Records the initial 10.1.2 changelog draft; this advisory expands that draft. |

No other changes from upstream #2069 are claimed as adopted. This operation does not merge or alter upstream ancestry, move tags, change package/assembly identities, alter dependencies, or introduce the planned major release's framework changes. This ledger's documentation additions remain pending until integrated by the repository's normal process.

### Validation and remaining work

- Windows x64: complete MTP tests passed 427 tests with seven platform skips on each of `net9.0` and `net10.0`; focused core discovery/filter tests passed 121 tests on each framework.
- Build-only: `netstandard2.0` MTP/core build passed with zero warnings and errors.
- Windows x64 consumer comparisons: 21 .NET 10 test projects across BenchmarkDotNet, Carter, Globalization, ASP.Versioning, Swashbuckle.AspNetCore, Newtonsoft.Json, and AWS Signature V4 passed 1,943 tests with each collector. The released baseline omitted all 15 library modules represented by these runs; the corrected collector included them with visited sequence points and retained BenchmarkDotNet's runner coverage. See the advisory for the exact scope and per-repository totals.
- Separate Cuemon .NET Framework 4.8 comparison: 729 tests passed with each collector, but `Cuemon.Core` and `Cuemon.Kernel` instrumentation still failed resolving framework assemblies. That failure is reproduced and unresolved, not fixed by this patch.
- Fixed-consumer Linux/macOS and .NET 11 execution, installation of a newly packed package, remote CI for this hotfix, and package publication have not been established by these local checks. Historical CI/report inspection is recorded separately from fixed-collector validation in the advisory.

The original bug is recorded in inherited ancestry; the correction is implemented and locally tested in the destination commits above. That does not establish that a synchronization PR is merged, remote CI is green, or 10.1.2 has been released. No release or publish operation is authorized by this ledger.
