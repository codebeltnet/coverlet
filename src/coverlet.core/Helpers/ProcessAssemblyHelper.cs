// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Coverlet.Core.Abstractions;
using Microsoft.Extensions.DependencyModel;

namespace Coverlet.Core.Helpers
{
  internal sealed class ProcessAssemblyHelper : IProcessAssemblyHelper
  {
    /// <inheritdoc/>
    public IReadOnlyList<string> GetLoadedAssemblyNames(string testAssemblyName)
    {
      if (string.IsNullOrWhiteSpace(testAssemblyName))
        throw new ArgumentException("Test assembly name must not be null or whitespace.", nameof(testAssemblyName));

      return AppDomain.CurrentDomain
        .GetAssemblies()
        .Where(a => !a.IsDynamic && !string.IsNullOrWhiteSpace(a.Location))
        .Select(a => a.GetName().Name)
        .Where(name => !string.IsNullOrWhiteSpace(name) &&
                       !name!.Equals(testAssemblyName, StringComparison.OrdinalIgnoreCase))
        .Distinct(StringComparer.OrdinalIgnoreCase)
        .ToList()!;
    }

    /// <inheritdoc/>
    public IReadOnlyList<string> GetDepsJsonAssemblyNames(string testModuleDirectory, string testAssemblyName)
    {
      if (string.IsNullOrWhiteSpace(testModuleDirectory))
        throw new ArgumentException("Test module directory must not be null or whitespace.", nameof(testModuleDirectory));

      if (string.IsNullOrWhiteSpace(testAssemblyName))
        throw new ArgumentException("Test assembly name must not be null or whitespace.", nameof(testAssemblyName));

      var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
      var projectNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

      using var reader = new DependencyContextJsonReader();
      foreach (string depsFile in Directory.EnumerateFiles(testModuleDirectory, "*.deps.json"))
      {
        try
        {
          using FileStream stream = File.OpenRead(depsFile);
          DependencyContext context = reader.Read(stream);

          foreach (RuntimeLibrary lib in context.RuntimeLibraries)
          {
            bool isProject = lib.Type.Equals("project", StringComparison.OrdinalIgnoreCase);

            // Package IDs need not match their assemblies (for example xunit.v3.mtp-v2).
            // Include every managed runtime asset, including RID-specific assemblies.
            foreach (string assetPath in lib.RuntimeAssemblyGroups.SelectMany(group => group.AssetPaths))
            {
              string extension = Path.GetExtension(assetPath);
              if (!extension.Equals(".dll", StringComparison.OrdinalIgnoreCase) &&
                  !extension.Equals(".exe", StringComparison.OrdinalIgnoreCase))
                continue;

              string name = Path.GetFileNameWithoutExtension(assetPath);
              if (!string.IsNullOrWhiteSpace(name) &&
                  !name.Equals(testAssemblyName, StringComparison.OrdinalIgnoreCase))
              {
                if (isProject)
                  projectNames.Add(name);
                else
                  names.Add(name);
              }
            }
          }
        }
        catch (Exception)
        {
          // Best-effort; a corrupt or unreadable deps.json is not fatal.
        }
      }

      // The SDK can also list a project executable's DLL in a separate reference entry.
      // Its project identity takes precedence over that duplicate dependency entry.
      names.ExceptWith(projectNames);
      return [.. names];
    }

    /// <summary>
    /// Converts a simple assembly name to a coverlet exclude-filter pattern.
    /// Matches only that assembly, so dependencies cannot exclude project assemblies
    /// merely because they share a name prefix.
    /// </summary>
    internal static string ToExcludeFilter(string assemblyName)
    {
      return $"[{assemblyName}]*";
    }

    /// <summary>
    /// Removes exact-name filters from <paramref name="filters"/> that are already matched
    /// by a wildcard filter present in the same collection.
    /// For example <c>[coverlet.core]*</c> is redundant when <c>[coverlet.*]*</c> is present.
    /// </summary>
    internal static IEnumerable<string> PruneRedundantFilters(IEnumerable<string> filters)
    {
      // Collect wildcard filters of the form "[prefix.*]*"
      var wildcards = new HashSet<string>(
        filters.Where(f => f.EndsWith(".*]*", StringComparison.Ordinal)),
        StringComparer.OrdinalIgnoreCase);

      foreach (string filter in filters)
      {
        if (!filter.EndsWith(".*]*", StringComparison.Ordinal)
            && IsExactFilterCoveredByWildcard(filter, wildcards))
        {
          continue; // skip — already covered
        }

        yield return filter;
      }
    }

    // Checks whether an exact filter like "[coverlet.core]*" is already matched by
    // a wildcard like "[coverlet.*]*" in the wildcard set.
    private static bool IsExactFilterCoveredByWildcard(string filter, HashSet<string> wildcards)
    {
      // filter must be of the form "[AssemblyName]*"
      if (filter.Length < 4 || filter[0] != '[' || !filter.EndsWith("]*", StringComparison.Ordinal))
        return false;

      string name = filter.Substring(1, filter.Length - 3); // strip "[" and "]*"

      int dotIndex = name.LastIndexOf('.');
      while (dotIndex > 0)
      {
        string candidate = $"[{name.Substring(0, dotIndex)}.*]*";
        if (wildcards.Contains(candidate))
          return true;
        dotIndex = name.LastIndexOf('.', dotIndex - 1);
      }

      return false;
    }
  }
}
