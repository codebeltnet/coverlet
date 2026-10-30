# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/) and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [vNext] - 2026-10-30

This is a patch release that makes the MTP package-validation tests prepare their own Codebelt.Coverlet.MTP package during local builds, including Visual Studio Test Explorer runs, with no runtime or package behavior changes.

### Added

- Add automatic preparation of the Codebelt.Coverlet.MTP package for the MTP package-validation tests during local builds, skipped for IDE design-time builds, with isolated output under `artifacts/validation` per configuration and framework.

### Changed

- Change the validation tests to read the package folder from MSBuild metadata instead of a hard-coded `artifacts/package/<configuration>` path, with missing-package guidance that points to `-p:PrepareValidationPackage=true` or an existing package,
- Disable automatic package preparation in GitHub Actions so validation consumes the downloaded release package, while allowing opt-out outside GitHub Actions with `-p:PrepareValidationPackage=false`,
- Document the automatic packing behavior and opt-out in `CONTRIBUTING.md`, replacing the manual `dotnet pack` step.

## [10.1.1] - 2026-10-20

This is a patch release that fixes module unload mutex collisions between unrelated coverage sessions and updates the Microsoft runtime pins and the `System.Security.Cryptography.Pkcs` override used by coverlet.core, with no new capabilities.

### Changed

- Update the central Microsoft .NET 10 and .NET 9 runtime pins from 10.0.12 and 9.0.20 to 10.0.13 and 9.0.21,
- Update the `System.Security.Cryptography.Pkcs` override in coverlet.core from 8.0.1 to 8.0.2.

### Fixed

- Fix unrelated coverage sessions sharing the module unload mutex by naming it from the full hit file name instead of the extension-stripped name, with regression coverage for the legacy assembly-prefix collision.

## [10.1.0] - 2026-09-27

This release synchronizes the Codebelt.Coverlet.MTP fork with upstream coverlet 10.1.0 fixes that apply to the narrowed MTP/core package surface while preserving the fork's multi-targeted package layout.

### Fixed

- Honor the Microsoft Testing Platform `--config-file` argument in coverlet.MTP instead of falling back directly to legacy `coverlet.mtp.appsettings.json` discovery.
- Resolve shared-framework assemblies that are missing from `compileLibraries` by falling back to `NetCoreSharedFrameworkResolver`, preventing silently empty coverage for modules affected by newer SDK dependency context behavior.
- Add regression coverage for `ExcludeByAttribute=CompilerGeneratedAttribute` so async state-machine methods remain instrumented.

## [10.0.1] - 2026-09-20

This is a minor release focused on the Codebelt.Coverlet.MTP fork with narrowed scope, enhanced coverage analysis, improved testing infrastructure, and build reliability improvements.

### Added

- Codebelt CI/CD pipeline with shared workflow jobs replacing legacy Azure Pipelines,
- Coverage threshold failure messages and exit codes for MTP handler,
- Method coverage calculation and reporting alongside line and branch coverage,
- CoverletCoverageDataProducer for MTP message bus publishing of coverage data,
- Preflight checks for locked and unresolvable assemblies before instrumentation,
- Architecture documentation and diagrams for all integration points,
- Comprehensive unit tests for previously untested internal methods with dependency injection test setup,
- Benchmarks, GitHub Actions, and documentation infrastructure for performance testing,
- Test for [DoesNotReturn] detection in async state machines,
- Dynamic exclusion filters for Coverlet.MTP assemblies to improve filtering reliability,
- netstandard2.0 target framework support for broader compatibility,
- Trace diagnostics via --diag option and actionable warnings for instrumentation, hit, and empty-result failures,
- ResourceStream null guard to improve robustness,
- URL documentation for central testconfig.json configuration,
- MinVer tag prefix configuration for source projects to ensure correct semantic version tag detection during build,
- Pre-publish validation job in CI pipeline to verify calculated release version before packing,
- Comprehensive package artifact validation that verifies package filename, contents, and nuspec metadata match expected values,
- Targeted NuGet package push that publishes only the validated package instead of using wildcard patterns.

### Changed

- Rebranded repository to Codebelt.Coverlet.MTP fork with narrowed scope focusing on MTP integration,
- Removed legacy projects, workflows, documentation, and examples to simplify codebase,
- Updated target frameworks to netstandard2.0, net9.0, and net10.0,
- Aligned dependencies to modern framework versions including .NET 10.0.12,
- Enhanced core instrumentation code and capabilities with improved architecture,
- Integrated MTP extension and fixed test isolation issues,
- Updated test infrastructure for modern testing platform (xunit v3, Microsoft.Testing.Platform),
- Improved .NET Framework assembly resolution on Windows,
- Enhanced report output with summary table and console reporters,
- Improved pattern matching branch detection logic and documentation,
- Improved log formatting for multi-line messages in console output,
- Replaced ConcurrentBag with List for unload handlers registry with explicit locking,
- Eliminated phantom branches from async try-finally with await statements,
- Relaxed auto-property skip logic to improve coverage for records,
- Configuration parsing and CoverageConfiguration enhancements,
- Replaced legacy .sln files with modern .slnx format,
- Updated dependencies to latest stable releases across all packages.

### Fixed

- Fix silent zero coverage on .NET Framework that occurred since 8.0.0,
- Fix EndOfStreamException in coverage collection,
- Fix pattern matching 'or' synthetic branch detection,
- Fix FieldReference handling in delegate cache branch detection,
- Avoid unnecessary testhost restarts during test execution,
- Normalize Cobertura XML paths to forward slashes for consistency,
- Module restored atomically to prevent loaded assembly corruption,
- Unknown assembly fallback behavior and error handling,
- MTP validation tests infrastructure and test isolation issues,
- Remove skipping UnresolvableDependencies from preflight checks.

### Removed

- Legacy .sln solution files (replaced with .slnx format),
- Legacy projects and workflows,
- Documentation files (Changelog.md, GlobalTool.md, KnownIssues.md, MSBuildIntegration.md, VSTestIntegration.md, UnderstandingBranchCoverage.md, Troubleshooting.md, DeterministicBuild.md, etc.),
- Example projects for MSBuild and VSTest integrations,
- CodeQL GitHub Actions workflow (integrated into ci-pipeline.yml),
- Legacy dotnet.yml and other Azure Pipelines workflows,
- Legacy build scripts (scripts/build.ps1, scripts/test.ps1, scripts/report.ps1),
- .vscode/settings.json configuration,
- .devcontainer legacy configuration.

[10.1.2]: https://github.com/codebeltnet/coverlet/compare/v10.1.1...vNext
[10.1.1]: https://github.com/codebeltnet/coverlet/compare/v10.1.0...v10.1.1
[10.1.0]: https://github.com/codebeltnet/coverlet/compare/v10.0.1...v10.1.0
[10.0.1]: https://github.com/codebeltnet/coverlet/compare/v10.0.0...v10.0.1
