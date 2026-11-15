# Repository guidance

This fork is intentionally narrower than `coverlet-coverage/coverlet`. Preserve the Codebelt package surface unless a task explicitly says otherwise.

## Upstream sync workflow

Synchronization flows only from `coverlet-coverage/coverlet` into `codebeltnet/coverlet`. Never push branches, tags, or pull requests to upstream. Preserve upstream ancestry while keeping file-level integration selective:

1. Read `UPSTREAM.md` before selecting changes. It records the reviewed upstream snapshot, completed ports, exclusions, and intentional fork differences. Fetch upstream without tags to avoid importing upstream release tags into this fork:

   ```powershell
   git fetch --no-tags https://github.com/coverlet-coverage/coverlet.git master:refs/remotes/coverlet-upstream/master
   ```

2. Start an isolated synchronization branch from this fork's default branch after integrating any baseline PRs. Preserve existing work in its original worktree. Capture the fork base and exact upstream tip, then compare from the shared base:

   ```powershell
   git merge-base HEAD coverlet-upstream/master
   git log --oneline --left-right --cherry-pick HEAD...coverlet-upstream/master
   ```

   Review every incoming commit as adopted, adapted, already present, or excluded. Use a normal `git merge --no-ff --no-commit <reviewed-upstream-tip>` and resolve paths individually, including automatic merge results. Do not use an `ours` strategy, blanket `-X ours`, or whole-tree restoration. Keep excluded surfaces excluded even though their commits become ancestors. If the tip is already an ancestor, record a no-change review instead of manufacturing a merge.

3. Port only changes that apply to the retained package surface: `src\coverlet.core`, `src\coverlet.MTP`, and the directly related tests.
4. Do not reintroduce upstream surfaces removed by this fork, including legacy console, collector, msbuild packages, legacy Azure Pipelines, broad upstream documentation, or `version.json`.
5. Preserve `Codebelt.Coverlet.MTP` package identity, `Coverlet.MTP` assembly identity, and the multi-targeted `netstandard2.0;net10.0;net11.0` layout.
6. Keep versioning on MinVer `v*` tags and the existing release workflow guards. Do not restore upstream Nerdbank.GitVersioning files.
7. Update `UPSTREAM.md` in the same change set as every upstream sync, including a no-change review. Record the operation date (`YYYY-MM-DD`), upstream repository/branch and exact reviewed commit, comparison base, source commits or PRs, what was ported or adapted, what was already present, and what was omitted or deferred with reasons. Link destination commits or the PR when available. Record validation actually performed, its target frameworks and outcomes, and any unresolved work. Distinguish reviewed, adopted, recorded in ancestry, tested, and released states; an upstream commit recorded as an ancestor does not mean every change in it was adopted. Update the entry when validation or integration completes, and preserve earlier entries.
   When reconstructing earlier operations, verify release-tag targets and distinguish inherited upstream ancestry, selective ports, and Codebelt-only maintenance. Keep the original upstream marker (`v10.0.1-coverlet`) distinct from the later shared comparison base. State whether a date comes from a source commit, integration commit, or recorded operation; do not invent tag creation dates, synchronization dates, or validation results. Do not move tags to make the history appear aligned.
8. Keep meaningful production behavior differences from upstream documented in `UPSTREAM.md`, with a reason and source or regression-test link. Preserve upstream behavior by default; the target-framework focus and Codebelt packaging/tooling do not authorize unrelated features or silent coverage-semantic changes.
9. Adapt release notes to this fork's root `CHANGELOG.md`; describe only changes that were actually ported. Keep `README.md` focused on orientation and usage, with a link to `UPSTREAM.md`; keep the detailed sync ledger in `UPSTREAM.md` and the maintenance rules here.
10. If an upstream tag was accidentally fetched and points at an upstream commit, delete that local tag before release preparation:

   ```powershell
   git tag -d v<version>
   ```

11. Preserve both parents of the upstream merge: the fork baseline first and the pinned upstream tip second. Integrate the synchronization PR into this fork using **Create a merge commit**, never squash or rebase. The fork permits merge commits and does not require linear history; retain its other PR, deletion, and force-push protections. Require the retained Linux/Windows/macOS CI matrix before integration, and verify the package identity, framework groups, MinVer calculation, and release guards.
12. After integration, fetch this fork's default branch without tags and verify `git merge-base --is-ancestor <reviewed-upstream-tip> origin/master` succeeds and `git rev-list --left-right --count origin/master...<reviewed-upstream-tip>` reports zero on the upstream side. Record the destination commit or merged PR and CI results in the ledger; the PR provides the authoritative merge state and destination commit when the ledger is part of that PR. This is synchronization to a pinned snapshot, not authorization to release or publish a package.

Before running tests, ask for explicit approval. Build-only validation is acceptable when it does not execute tests.

<!-- dotnet-docfx-digest:start -->
## DocFX Documentation Maintenance

When changing public .NET APIs, keep the DocFX documentation current in the same change set.

Documentation updates must cover public API only. Do not document private or internal types or members. Do not create namespace overview pages for namespaces that contain no public API.

Public non-abstraction types — including enums, structs, records, plain classes, and static extension containers — are valid documentation targets. Generic public types and generic extension methods are valid documentation targets too. Do not exclude a type solely because it is generic or because reflection reports it as abstract and sealed (that is the IL pattern for a static class).

For public non-abstraction types, include at least one realistic, copy/paste-ready usage example on the generated type page/overwrite section for that type UID. For example, a public `Class1` requires an example on the `Class1` API page, not only on the namespace page. Prefer deriving examples from existing unit, functional, or integration tests, but convert test code into real-life consumer-oriented usage.

Missing type examples must be added through per-type DocFX overwrite files under `.docfx/api/types/{TypeUid}.md` in Codebelt repositories. Namespace overview text and `Extension Members` tables are not substitutes for type-page examples.

Public extension methods must have examples too. Listing an extension method in an `Extension Members` table is required, but it is not enough.

All added or changed code samples must be deterministic and verified to compile. Do not add pseudo-code, ellipses, hidden test helpers, or examples that rely on unverified behavior.

Compilation is necessary but not sufficient. Do not present runtime implementation names such as `services.GetType().Name` or `host.GetType().FullName` as the example outcome. Show application behavior, configured state, a resolved domain service, an HTTP response, or another result that explains why a caller uses the API. Application-entry-point examples must not declare an empty local `Program` type merely to compile; show a real entry point or clearly identify the referenced application project.

Every namespace containing public API must have a DocFX namespace overview page named after the namespace, such as `X.Y.Z.md`, under `.docfx/api/namespaces/`, using DocFX overwrite front matter with the namespace `uid`.

Namespace pages must identify key entry points from release notes, package documentation, public factories/builders, and strong functional tests, then help readers choose among adjacent workflows. When the package complements a well-known upstream API, compare concrete acquisition, customization, lifecycle, and sharing tradeoffs from current official guidance; do not claim drop-in replacement compatibility without evidence.

Namespaces exposing public extension methods must document those extension members at namespace level. The namespace page must include an `Extension Members` table listing the extended type, the extension marker, and the public extension methods. Extension members are rendered under the heading `Extension Members`.

Both namespace overwrite files and type overwrite files are required deliverables in the same run. Generating only namespace pages or only type pages is incomplete.

`docfx.json` must keep namespace and type overwrite files in separate subdirectories. `build.overwrite` must include both `api/namespaces/**/*.md` (for namespace pages) and `api/types/**/*.md` (for type pages). `build.content` must exclude both `api/namespaces/**` and `api/types/**` to prevent overwrite Markdown from being treated as conceptual content. Do not use `api/**/*.md` under `build.overwrite` or `build.content`.

Availability must be documented by referencing the appropriate include file when one exists, or by adding explicit availability text when no suitable include exists. Availability must reflect the actual target frameworks, conditional compilation, and project configuration.

For conditionally compiled APIs, choose the executable test framework from the asset that contains the API. Inspect the preprocessor condition, project TFMs, package `lib/` assets, and resolved consumer asset before changing a sample. For APIs under `NETSTANDARD2_0` or `NETSTANDARD2_0_OR_GREATER`, when modern `lib/netX.0/` assets also exist, use `net48` (or another supported .NET Framework target from `net462` onward) so the consumer selects `lib/netstandard2.0/`. Never use `netstandard*` as an executable target, and never use a modern `netX.0` target when it selects an asset where the API is absent. For other TFM guards, select a runnable consumer TFM that resolves to the containing asset and confirm that selection from restore or build evidence.

Preserve manual documentation edits. Prefer additive changes, but correct stale or contradictory information so documentation remains accurate.

Preserve working Markdown links, `Related:` references, and historical URL citations during prose rewrites. Remove or replace a URL only after directly verifying that the current destination returns HTTP 404. Timeouts, 403s, rate limits, DNS failures, and other lookup problems are not removal evidence.

Interim scratch artifacts do not belong in the repository working tree. Store assessment queues, project manifests, review reports, captured validator output, progress notes, and one-off helper scripts in temp or session storage instead. New working-tree files are only legitimate when they are the managed `AGENTS.md` block, the active `docfx.json`, the deterministic `skip-compile-allowlist.json` waiver file when one is truly required, or DocFX-authored namespace/type Markdown that maps to a real public namespace or type. Everything else is blocking cleanup work, not a documentation deliverable. The validator auto-detects generic-arity type families (such as `MutableTuple`1`..`MutableTuple`N`) and skips redundant sibling examples from the public API surface alone, so no family-skip manifest is ever written into the repository.

Skip markers are waivers, not fixes. A skip marker only suppresses compilation when it both existed before the current run and matches an entry in `.docfx/skip-compile-allowlist.json`. Each allowlist entry must include `diagnosticCode`, `filePath`, `uid` or `symbol`, `reason`, `approval`, and `lifetime` (`temporary` or `permanent`). Newly introduced or unallowlisted skip markers remain fail-level diagnostics and do not permit a completion claim.

Do not emit a final report, audit result, completion summary, or handoff while `summary.canClaimCompletion` is false, `summary.remainingWorkItems` is greater than zero, `summary.remainingGates` is non-empty, `summary.fullVerificationRan` is false, fail-level diagnostics remain, `summary.newlyIntroducedSkipMarkers` is non-zero, or `summary.interimArtifacts` is non-zero. Large queues, many changed files, repetitive next steps, long runtimes, context pressure, session length, task size, or a "stable queue" are not valid stop reasons; the next action must be another remediation batch, a validator rerun, a validator/tooling fix, or a true blocker with exact evidence.

Context pressure is not a completion condition. If the session feels constrained while work remains, continue with a smaller deterministic batch, regenerate deterministic queue state such as `--assessment-queue`, `--project-manifest`, or the active dry-run manifest/review pair, or report a true tooling failure with the exact command, exit code, and output. When naming a queue-state regeneration command, resolve it to a concrete temp/session path instead of leaving `<temp-path>` as a placeholder. Do not stop with phrases like "given context constraints", "best done in a follow-up", "remaining work requires authoring", "this is a massive task", or "I will provide a focused summary". A context-sized handoff while work remains is `FAIL_CONTEXT_HANDOFF_WITH_REMAINING_WORK`; the remediation is to continue with a smaller deterministic batch.

Before completing documentation work, run the relevant verification commands, normally:

```bash
dotnet build
dotnet test
dotnet run --file <resolved-skill-dir>/scripts/docfx.cs -- --repo-root . --build-api-model --validate-samples --verify-docfx-build
```

Codebelt repositories are normally strong-name signed with a `.snk` file in the repository root on the main author's codespace. Preserve and copy that root `.snk` file when building a temporary copy. If the repository or temp copy has no root `.snk`, run build and test verification with `-p:SkipSignAssembly=true`, for example `dotnet build -p:SkipSignAssembly=true` and `dotnet test -p:SkipSignAssembly=true`.

The final DocFX verification must run outside the working tree when possible. The `--verify-docfx-build` option copies the repository to a temp workspace, runs DocFX against the resolved `docfx.json` there, and removes the temp workspace afterward so generated API YAML, manifest files, and site output do not flood git status. Do not call the work complete until the final JSON reports `summary.fullVerificationRan: true`, `summary.canClaimCompletion: true`, `summary.remainingWorkItems: 0`, an empty `summary.remainingGates`, an empty `summary.remainingDiagnosticsByCode`, `summary.newlyIntroducedSkipMarkers: 0`, and `summary.interimArtifacts: 0`.

If a command cannot be run, report the exact limitation or failure instead of claiming the documentation was verified.
<!-- dotnet-docfx-digest:end -->
