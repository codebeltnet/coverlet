// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Coverlet.MTP.Collector;
using Coverlet.MTP.InProcess;
using coverlet.MTP.EnvironmentVariables;
using Microsoft.Testing.Platform.Builder;
using Microsoft.Testing.Platform.Configurations;
using Microsoft.Testing.Platform.Extensions.TestHostControllers;
using Microsoft.Testing.Platform.Services;

namespace Coverlet.MTP;

/// <summary>
/// Registers controller instrumentation, test-host coverage flushing and coverage command-line options with MTP.
/// </summary>
public static class CoverletExtensionProvider
{
  /// <summary>
  /// Adds the complete Coverlet coverage integration to a custom test application builder.
  /// </summary>
  /// <param name="builder">The builder to configure before constructing the test application.</param>
  /// <param name="ignoreIfNotSupported">Reserved for compatibility. This parameter is currently ignored.</param>
  /// <remarks>Register once. Package-generated discovery normally calls this through <see cref="TestingPlatformBuilderHook"/>.</remarks>
  public static void AddCoverletExtensionProvider(this ITestApplicationBuilder builder, bool ignoreIfNotSupported = false)
  {
    // ============================================================
    // OUT-OF-PROCESS EXTENSIONS (run in controller process)
    // These observe the test host from outside
    // ============================================================

    // Environment variable provider - passes configuration to test host
    builder.TestHostControllers.AddEnvironmentVariableProvider(serviceProvider
        => new CoverletExtensionEnvironmentVariableProvider(
            serviceProvider.GetConfiguration(),
            serviceProvider.GetCommandLineOptions(),
            serviceProvider.GetLoggerFactory()));

    // Process lifetime handler - instruments before tests, collects after
    builder.TestHostControllers.AddProcessLifetimeHandler(static serviceProvider
        =>
        {
          IConfiguration configuration = serviceProvider.GetConfiguration();
          return new CollectorExtension(
              serviceProvider.GetLoggerFactory(),
              serviceProvider.GetCommandLineOptions(),
              serviceProvider.GetOutputDevice(),
              configuration,
              messageBus: serviceProvider.GetMessageBus()) as ITestHostProcessLifetimeHandler;
        });

    // ============================================================
    // IN-PROCESS EXTENSIONS (run inside test host process)
    // These run in the same process as the tests
    // ============================================================

    // Test session lifetime handler - flushes coverage data when session ends
    builder.TestHost.AddTestSessionLifetimeHandler(static serviceProvider
        => new CoverletInProcessHandler(serviceProvider.GetLoggerFactory()));

    // ============================================================
    // COMMAND LINE
    // ============================================================

    builder.CommandLine.AddProvider(() => new CommandLine.CoverletExtensionCommandLineProvider(new CoverletExtension()));
  }
}
