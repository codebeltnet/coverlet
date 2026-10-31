# Repository guidance

This fork is intentionally narrower than `coverlet-coverage/coverlet`. Preserve the Codebelt package surface unless a task explicitly says otherwise.

## Upstream sync workflow

When synchronizing from `coverlet-coverage/coverlet`, keep the sync selective:

1. Read `UPSTREAM.md` before selecting changes. It records the reviewed upstream snapshot, completed ports, exclusions, and intentional fork differences. Fetch upstream without tags to avoid importing upstream release tags into this fork:

   ```powershell
   git fetch --no-tags https://github.com/coverlet-coverage/coverlet.git master:refs/remotes/coverlet-upstream/master
   ```

2. Compare from the shared base:

   ```powershell
   git merge-base HEAD coverlet-upstream/master
   git log --oneline --left-right --cherry-pick HEAD...coverlet-upstream/master
   ```

3. Port only changes that apply to the retained package surface: `src\coverlet.core`, `src\coverlet.MTP`, and the directly related tests.
4. Do not reintroduce upstream surfaces removed by this fork, including legacy console, collector, msbuild packages, legacy Azure Pipelines, broad upstream documentation, or `version.json`.
5. Preserve `Codebelt.Coverlet.MTP` package identity, `Coverlet.MTP` assembly identity, and the multi-targeted `netstandard2.0;net10.0;net11.0` layout.
6. Keep versioning on MinVer `v*` tags and the existing release workflow guards. Do not restore upstream Nerdbank.GitVersioning files.
7. Update `UPSTREAM.md` in the same change set as every upstream sync, including a no-change review. Record the operation date (`YYYY-MM-DD`), upstream repository/branch and exact reviewed commit, comparison base, source commits or PRs, what was ported or adapted, what was already present, and what was omitted or deferred with reasons. Link destination commits or the PR when available. Record validation actually performed, its target frameworks and outcomes, and any unresolved work. Distinguish reviewed, ported, tested, and released states; a reviewed upstream tip does not mean every preceding commit was merged. Update the entry when validation or integration completes, and preserve earlier entries.
   When reconstructing earlier operations, verify release-tag targets and distinguish inherited upstream ancestry, selective ports, and Codebelt-only maintenance. Keep the original upstream marker (`v10.0.1-coverlet`) distinct from the later shared comparison base. State whether a date comes from a source commit, integration commit, or recorded operation; do not invent tag creation dates, synchronization dates, or validation results. Do not move tags to make the history appear aligned.
8. Keep meaningful production behavior differences from upstream documented in `UPSTREAM.md`, with a reason and source or regression-test link. Preserve upstream behavior by default; the target-framework focus and Codebelt packaging/tooling do not authorize unrelated features or silent coverage-semantic changes.
9. Adapt release notes to this fork's root `CHANGELOG.md`; describe only changes that were actually ported. Keep `README.md` focused on orientation and usage, with a link to `UPSTREAM.md`; keep the detailed sync ledger in `UPSTREAM.md` and the maintenance rules here.
10. If an upstream tag was accidentally fetched and points at an upstream commit, delete that local tag before release preparation:

   ```powershell
   git tag -d v<version>
   ```

Before running tests, ask for explicit approval. Build-only validation is acceptable when it does not execute tests.
