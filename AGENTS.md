# Repository guidance

This fork is intentionally narrower than `coverlet-coverage/coverlet`. Preserve the Codebelt package surface unless a task explicitly says otherwise.

## Upstream sync workflow

When synchronizing from `coverlet-coverage/coverlet`, keep the sync selective:

1. Fetch upstream without tags to avoid importing upstream release tags into this fork:

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
5. Preserve `Codebelt.Coverlet.MTP` package identity, `Coverlet.MTP` assembly identity, and the multi-targeted `netstandard2.0;net9.0;net10.0` layout.
6. Keep versioning on MinVer `v*` tags and the existing release workflow guards. Do not restore upstream Nerdbank.GitVersioning files.
7. Adapt release notes to this fork's root `CHANGELOG.md`; describe only changes that were actually ported.
8. If an upstream tag was accidentally fetched and points at an upstream commit, delete that local tag before release preparation:

   ```powershell
   git tag -d v<version>
   ```

Before running tests, ask for explicit approval. Build-only validation is acceptable when it does not execute tests.
