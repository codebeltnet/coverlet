// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Testing.Platform.Builder;
using Coverlet.MTP.InProcDataCollection;

namespace Coverlet.MTP.Configuration;

/// <summary>
/// Registers the legacy Coverlet session handler on a test application builder.
/// </summary>
public static class CoverletConfigurationExtensions
{
  /// <summary>
  /// Adds the legacy session handler, whose session callbacks are not implemented.
  /// </summary>
  /// <param name="builder">The builder on which to register the legacy handler.</param>
  /// <remarks>Use <see cref="CoverletExtensionProvider.AddCoverletExtensionProvider"/> for working coverage collection. This method does not register controller instrumentation or report collection.</remarks>
  public static void AddCoverletCodeCoverage(this ITestApplicationBuilder builder)
  {
    // Register the test session lifetime handler
    builder.TestHost.AddTestSessionLifetimeHandler(serviceProvider =>
        new CoverletTestSessionHandler());
  }
}
