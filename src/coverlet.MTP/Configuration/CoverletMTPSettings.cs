// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Text;
using Coverlet.Core.Enums;

namespace Coverlet.MTP.Configuration;

/// <summary>
/// Describes coverage filters, reports and threshold settings for the MTP integration.
/// </summary>
public class CoverletMTPSettings
{
  /// <summary>
  /// Indicates whether this settings object was loaded from a configuration file.
  /// When true, defaults should not be applied - all values are user-defined.
  /// When false, built-in defaults should be applied for unspecified options.
  /// </summary>
  public bool IsFromConfigFile { get; set; }

  /// <summary>Gets or sets the path to the test assembly to instrument.</summary>
  public string? TestModule { get; set; }
  /// <summary>Gets or sets the requested coverage report formats. A new settings instance requests Cobertura.</summary>
  public string[] ReportFormats { get; set; } = ["cobertura"];
  /// <summary>Gets or sets the assembly/type filters eligible for coverage.</summary>
  public string[] IncludeFilters { get; set; } = [];
  /// <summary>Gets or sets additional directories to search for assemblies.</summary>
  public string[] IncludeDirectories { get; set; } = [];
  /// <summary>Gets or sets assembly/type filters excluded from coverage. New instances exclude coverlet.* assemblies.</summary>
  public string[] ExcludeFilters { get; set; } = ["[coverlet.*]*"]; // Exclude types in Coverlet namespaces by default
  /// <summary>Gets or sets source-file glob patterns excluded from coverage.</summary>
  public string[] ExcludeSourceFiles { get; set; } = [];
  /// <summary>Gets or sets attribute names that exclude annotated code from coverage.</summary>
  public string[] ExcludeAttributes { get; set; } = [];
  /// <summary>Gets or sets the path to an existing coverage JSON result to merge, or null for no merge.</summary>
  public string? MergeWith { get; set; }
  /// <summary>Gets or sets whether reports use Source Link URLs in place of local source paths.</summary>
  public bool UseSourceLink { get; set; }
  /// <summary>Gets or sets whether each instrumented location records only its first hit.</summary>
  public bool SingleHit { get; set; }
  /// <summary>Gets or sets whether test-assembly instrumentation is requested where the execution model permits it.</summary>
  public bool IncludeTestAssembly { get; set; }
  /// <summary>Gets or sets whether automatically implemented properties are excluded from instrumentation.</summary>
  public bool SkipAutoProps { get; set; }
  /// <summary>Gets or sets attribute names used to identify methods that do not return.</summary>
  public string[] DoesNotReturnAttributes { get; set; } = [];
  /// <summary>Gets or sets whether deterministic coverage report output is requested.</summary>
  public bool DeterministicReport { get; set; }
  /// <summary>Gets or sets the source-availability policy: MissingAll, MissingAny or None. The default is MissingAll.</summary>
  public string ExcludeAssembliesWithoutSources { get; set; } = "MissingAll";
  /// <summary>Gets or sets whether restoring original instrumented modules through managed code is disabled.</summary>
  public bool DisableManagedInstrumentationRestore { get; set; }
  /// <summary>Gets or sets the required coverage percentage, or null to disable threshold checking.</summary>
  public int? Threshold { get; set; }
  internal ThresholdStatistic ThresholdStat { get; set; } = ThresholdStatistic.Minimum;
  /// <summary>Gets or sets the coverage dimensions checked against the threshold. New instances check line coverage.</summary>
  public List<string> ThresholdType { get; set; } = ["line"];
  /// <summary>Formats instrumentation and threshold settings for diagnostics.</summary>
  /// <returns>A comma-separated settings snapshot. Report formats and the configuration-file flag are not included.</returns>
  public override string ToString()
  {
    var builder = new StringBuilder();
    builder.AppendFormat("TestModule: '{0}', ", TestModule);
    builder.AppendFormat("IncludeFilters: '{0}', ", string.Join(",", IncludeFilters));
    builder.AppendFormat("IncludeDirectories: '{0}', ", string.Join(",", IncludeDirectories));
    builder.AppendFormat("ExcludeFilters: '{0}', ", string.Join(",", ExcludeFilters));
    builder.AppendFormat("ExcludeSourceFiles: '{0}', ", string.Join(",", ExcludeSourceFiles));
    builder.AppendFormat("ExcludeAttributes: '{0}', ", string.Join(",", ExcludeAttributes));
    builder.AppendFormat("MergeWith: '{0}', ", MergeWith);
    builder.AppendFormat("UseSourceLink: '{0}', ", UseSourceLink);
    builder.AppendFormat("SingleHit: '{0}', ", SingleHit);
    builder.AppendFormat("IncludeTestAssembly: '{0}', ", IncludeTestAssembly);
    builder.AppendFormat("SkipAutoProps: '{0}', ", SkipAutoProps);
    builder.AppendFormat("DoesNotReturnAttributes: '{0}', ", string.Join(",", DoesNotReturnAttributes));
    builder.AppendFormat("DeterministicReport: '{0}', ", DeterministicReport);
    builder.AppendFormat("ExcludeAssembliesWithoutSources: '{0}', ", ExcludeAssembliesWithoutSources);
    builder.AppendFormat("Threshold: '{0}', ", Threshold);
    builder.AppendFormat("ThresholdStat: '{0}', ", ThresholdStat);
    builder.AppendFormat("ThresholdType: '{0}'", string.Join(",", ThresholdType));
    return builder.ToString();
  }
}
