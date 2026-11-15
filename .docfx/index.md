---
uid: frontpage-md
title: Codebelt.Coverlet.MTP
---
![Codebelt.Coverlet.MTP](images/coverlet-icon.png)

# Codebelt.Coverlet.MTP

Collect cross-platform .NET code coverage from Microsoft Testing Platform test projects. This Codebelt-maintained Coverlet fork ships one package, `Codebelt.Coverlet.MTP`, with .NET Standard 2.0, .NET 10 and .NET 11 assets.

## Collect coverage

Add the package to an MTP-enabled test project, then enable coverage collection:

```shell
dotnet add package Codebelt.Coverlet.MTP
dotnet test --coverlet
```

Normal package usage relies on build-time discovery; no manual API registration is needed. The extension collects coverage and produces JSON and Cobertura reports by default. See the [installation and configuration guide](https://github.com/codebeltnet/coverlet/blob/master/Documentation/Coverlet.MTP.Integration.md) for runner setup, filters, thresholds and additional report formats.

## API reference

Use the [MTP integration reference](xref:Coverlet.MTP) when building a custom test runner. The [configuration reference](xref:Coverlet.MTP.Configuration) distinguishes complete registration from the exposed legacy handler, which cannot run coverage sessions. The [diagnostics reference](xref:Coverlet.MTP.Diagnostics) explains environment switches for investigating instrumentation failures.

The reference also includes the small public surface of the bundled `coverlet.core.dll`. Most instrumentation, coverage-model and reporter types are internal and are not part of this API reference. The engine is not a separately published Codebelt package.

## Project and maintenance

The fork retains Coverlet's MTP assembly identity and focuses on the Codebelt package's framework support and maintenance. It is an extension for MTP, not a replacement test framework. For VSTest, console or MSBuild integrations, use the upstream Coverlet packages.

- [Repository and usage](https://github.com/codebeltnet/coverlet)
- [NuGet package](https://www.nuget.org/packages/Codebelt.Coverlet.MTP/)
- [Coverage advisory for versions 10.0.1–10.1.1](https://github.com/codebeltnet/coverlet/blob/master/Documentation/DependencyExclusionRegression.md)
- [Upstream synchronization record](https://github.com/codebeltnet/coverlet/blob/master/UPSTREAM.md)
- [Changelog](https://github.com/codebeltnet/coverlet/blob/master/CHANGELOG.md)

## Build this documentation

With the repository SDK and the DocFX CLI installed, run `docfx .docfx/docfx.json` from the repository root. Generated metadata is written under `.docfx/api`; the site is written to `.docfx/wwwroot`. Documentation samples are compiled separately by the digest verifier and do not execute tests.

For the same container workflow used by the other Codebelt libraries, run `pwsh .docfx/BuildDocfxImage.ps1` from the repository root. This generates API metadata, builds and loads `coverlet-docfx:<MinVer version>` for Linux ARM64 and AMD64, then removes generated metadata while preserving authored API pages. It requires the repository SDK, DocFX, MinVer, Docker Buildx with a multi-platform image store, and access to the shared `codebeltnet/docfx:2` and `dhi.io/nginx:1.31-alpine` images.

When ready to publish an already-built image, run `pwsh .docfx/PublishDocfxImage.ps1`. It tags and pushes the same version to `jcr.codebelt.net/geekle/coverlet-docfx`; authenticate to that registry first. The scripts resolve paths from their own location and can also be run inside `.docfx`. Publishing the image is separate from deploying it or releasing the NuGet package.
