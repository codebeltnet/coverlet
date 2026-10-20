// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;

namespace Coverlet.MTP.validation.tests;

internal static class ValidationPackage
{
    // MSBuild selects an isolated local package or the downloaded CI release artifact.
    internal static string DirectoryPath { get; } = typeof(ValidationPackage).Assembly
        .GetCustomAttributes<AssemblyMetadataAttribute>()
        .Single(attribute => attribute.Key == "ValidationPackagePath").Value
        ?? throw new InvalidOperationException("Validation package path is missing. Rebuild the validation test project.");
}
