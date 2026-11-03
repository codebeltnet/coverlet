// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Coverlet.Core.Abstractions;
using Coverlet.Core.Helpers;
using Microsoft.Testing.Platform.CommandLine;
using Moq;
using Xunit;

namespace Coverlet.MTP.Configuration.Tests;

public sealed class CoverageConfigurationDependencyDiscoveryTests
{
  [Theory]
  [InlineData("Codebelt.Extensions.Xunit", "Codebelt.Extensions.Xunit", "Codebelt.Extensions.BenchmarkDotNet")]
  [InlineData("Example.Testing.Infrastructure", "Example.Testing.Infrastructure", "Example.Testing.Product")]
  [InlineData("xunit.v3.mtp-v2", "xunit.v3.core", "xunit.v3.Product")]
  [InlineData("Example.Testing.Package", "Example.Testing.test-host", "Example.Testing.Product")]
  public void WhenDependencySharesProjectPrefixThenOnlyDependencyAssembliesAreExcluded(
    string packageName, string dependencyAssembly, string projectAssembly)
  {
    string directory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString("N"));
    Directory.CreateDirectory(directory);
    try
    {
      File.WriteAllText(Path.Combine(directory, "MyTests.deps.json"), $$"""
        {
          "runtimeTarget": { "name": ".NETCoreApp,Version=v10.0" },
          "targets": {
            ".NETCoreApp,Version=v10.0": {
              "MyTests/1.0.0": { "runtime": { "MyTests.dll": {} } },
              "{{projectAssembly}}/1.0.0": { "runtime": { "{{projectAssembly}}.dll": {} } },
              "{{packageName}}/1.0.0": {
                "runtime": {
                  "lib/net10.0/{{dependencyAssembly}}.dll": {},
                  "lib/net10.0/External.Helper.dll": {}
                },
                "runtimeTargets": {
                  "runtimes/win/lib/net10.0/External.Windows.dll": { "rid": "win", "assetType": "runtime" },
                  "runtimes/win/native/External.Native.dll": { "rid": "win", "assetType": "native" }
                }
              },
              "Empty.Package/1.0.0": { "runtime": { "lib/net10.0/_._": {} } }
            }
          },
          "libraries": {
            "MyTests/1.0.0": { "type": "project", "serviceable": false, "sha512": "" },
            "{{projectAssembly}}/1.0.0": { "type": "project", "serviceable": false, "sha512": "" },
            "{{packageName}}/1.0.0": { "type": "package", "serviceable": false, "sha512": "" },
            "Empty.Package/1.0.0": { "type": "package", "serviceable": false, "sha512": "" }
          }
        }
        """);

      var config = new CoverageConfiguration(
        Mock.Of<ICommandLineOptions>(), configFileSettings: null,
        testModulePath: Path.Combine(directory, "MyTests.dll"));
      var selector = new InstrumentationHelper(
        Mock.Of<IProcessExitHandler>(), Mock.Of<IRetryHelper>(), Mock.Of<IFileSystem>(),
        Mock.Of<Coverlet.Core.Abstractions.ILogger>(), Mock.Of<ISourceRootTranslator>());
      string[] modules = [
        $"{projectAssembly}.dll", $"{dependencyAssembly}.dll", "External.Helper.dll", "External.Windows.dll",
        "Microsoft.VisualStudio.TestPlatform.TestFramework.dll", "testhost.dll", "ReportGenerator.Mtp.dll",
        "xunit.abstractions.dll", "xunit.runner.visualstudio.testadapter.dll",
        "xunit.v3.runner.utility.netcore.dll", "xunit.v3.runner.utility.netfx.dll"
      ];

      string[] filters = config.GetExcludeFilters();
      Assert.All(filters, filter => Assert.True(selector.IsValidFilterExpression(filter), $"Invalid generated filter: {filter}"));
      string[] validFilters = [.. filters.Where(selector.IsValidFilterExpression)];
      string[] selected = [.. selector.SelectModules(modules, config.GetIncludeFilters(), validFilters)];

      Assert.Contains($"{projectAssembly}.dll", selected);
      Assert.Equal([$"{projectAssembly}.dll"], selected);
      Assert.DoesNotContain("[_]*", filters);
      Assert.DoesNotContain("[External.Native]*", filters);
      Assert.DoesNotContain("[Empty.Package]*", filters);
    }
    finally
    {
      Directory.Delete(directory, recursive: true);
    }
  }

  [Theory]
  [InlineData(false)]
  [InlineData(true)]
  public void WhenWildcardExplicitlyConfiguredThenMatchingProjectIsStillExcluded(bool useConfigFile)
  {
    string[] explicitFilters = ["[Example.Testing.*]*"];
    var options = new Mock<ICommandLineOptions>();
    if (!useConfigFile)
    {
      string[]? cliFilters = explicitFilters;
      options.Setup(x => x.TryGetOptionArgumentList(Coverlet.MTP.CommandLine.CoverletOptionNames.Exclude, out cliFilters))
        .Returns(true);
    }

    var config = new CoverageConfiguration(options.Object,
      useConfigFile ? new CoverletMTPSettings { IsFromConfigFile = true, ExcludeFilters = explicitFilters } : null,
      testModulePath: null);
    var selector = new InstrumentationHelper(
      Mock.Of<IProcessExitHandler>(), Mock.Of<IRetryHelper>(), Mock.Of<IFileSystem>(),
      Mock.Of<Coverlet.Core.Abstractions.ILogger>(), Mock.Of<ISourceRootTranslator>());

    string[] selected = [.. selector.SelectModules(
      ["Example.Testing.Product.dll", "Other.Product.dll", "xunit.v3.runner.utility.netcore.dll"],
      config.GetIncludeFilters(), config.GetExcludeFilters())];

    Assert.Equal(["Other.Product.dll", "xunit.v3.runner.utility.netcore.dll"], selected);
  }
}
