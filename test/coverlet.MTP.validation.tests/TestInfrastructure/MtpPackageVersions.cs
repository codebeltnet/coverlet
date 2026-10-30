// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Xml.Linq;

namespace Coverlet.MTP.validation.tests;

/// <summary>
/// Reads package versions from <c>Directory.Packages.props</c> at the repository root so that
/// generated test projects and package assertions use the same versions as the central package-management file.
/// Values are resolved lazily and cached after the first read.
/// </summary>
internal static class MtpPackageVersions
{
    private static readonly Lazy<(string MicrosoftTestingPlatform, string XunitV3, string MicrosoftNet10Runtime, string MicrosoftNet11Runtime)> s_versions =
        new(ReadVersionsFromProps);

    /// <summary>Gets the value of <c>MicrosoftTestingPlatformVersion</c> from <c>Directory.Packages.props</c>.</summary>
    internal static string MicrosoftTestingPlatform => s_versions.Value.MicrosoftTestingPlatform;

    /// <summary>Gets the value of <c>XunitV3Version</c> from <c>Directory.Packages.props</c>.</summary>
    internal static string XunitV3 => s_versions.Value.XunitV3;

    /// <summary>Gets the value of <c>MicrosoftNet10RuntimeVersion</c> from <c>Directory.Packages.props</c>.</summary>
    internal static string MicrosoftNet10Runtime => s_versions.Value.MicrosoftNet10Runtime;

    /// <summary>Gets the value of <c>MicrosoftNet11RuntimeVersion</c> from <c>Directory.Packages.props</c>.</summary>
    internal static string MicrosoftNet11Runtime => s_versions.Value.MicrosoftNet11Runtime;

    private static (string MicrosoftTestingPlatform, string XunitV3, string MicrosoftNet10Runtime, string MicrosoftNet11Runtime) ReadVersionsFromProps()
    {
        string? dir = AppContext.BaseDirectory;
        while (!string.IsNullOrEmpty(dir))
        {
            string candidate = Path.Combine(dir, "Directory.Packages.props");
            if (File.Exists(candidate))
            {
                XDocument doc = XDocument.Load(candidate);

                string? GetProp(string name) => doc
                    .Descendants("PropertyGroup")
                    .Elements(name)
                    .FirstOrDefault()?.Value?.Trim();

                string mtp = GetProp("MicrosoftTestingPlatformVersion")
                    ?? throw new InvalidOperationException(
                        $"Property 'MicrosoftTestingPlatformVersion' not found in '{candidate}'.");

                string xunit = GetProp("XunitV3Version")
                    ?? throw new InvalidOperationException(
                        $"Property 'XunitV3Version' not found in '{candidate}'.");

                string net10Runtime = GetProp("MicrosoftNet10RuntimeVersion")
                    ?? throw new InvalidOperationException(
                        $"Property 'MicrosoftNet10RuntimeVersion' not found in '{candidate}'.");

                string net11Runtime = GetProp("MicrosoftNet11RuntimeVersion")
                    ?? throw new InvalidOperationException(
                        $"Property 'MicrosoftNet11RuntimeVersion' not found in '{candidate}'.");

                return (mtp, xunit, net10Runtime, net11Runtime);
            }

            dir = Path.GetDirectoryName(dir);
        }

        throw new InvalidOperationException(
            "Could not find 'Directory.Packages.props' in any ancestor directory of the test assembly. " +
            "Ensure the test is run from within the repository.");
    }
}
