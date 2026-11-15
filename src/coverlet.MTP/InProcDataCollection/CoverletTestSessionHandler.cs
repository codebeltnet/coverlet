// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System.Reflection;
using Coverlet.Core.Instrumentation;
using Coverlet.MTP.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Services;

namespace Coverlet.MTP.InProcDataCollection;

/// <summary>
/// Exposes the legacy coverage session handler for compatibility with existing integrations.
/// </summary>
/// <remarks>
/// This handler is not a working collection path. Public session callbacks throw
/// <see cref="NotImplementedException"/>, and explicit interface callbacks recursively dispatch to themselves.
/// Use <see cref="CoverletExtensionProvider"/> to register the active coverage integration.
/// </remarks>
public class CoverletTestSessionHandler : ITestSessionLifetimeHandler
{
  private readonly CoverletMTPSettings _settings;

  /// <summary>Initializes the legacy handler with default settings.</summary>
  public CoverletTestSessionHandler()
  {
    _settings = new CoverletMTPSettings();
  }

  /// <summary>Initializes the legacy handler by parsing configuration for a test module.</summary>
  /// <param name="configuration">The configuration to parse, or null to use defaults.</param>
  /// <param name="testModule">The test assembly path used by the settings parser.</param>
  public CoverletTestSessionHandler(IConfiguration? configuration, string testModule)
  {
    _ = new CoverletMTPSettingsParser();
    _settings = CoverletMTPSettingsParser.Parse(configuration, testModule);
  }

  /// <summary>Gets the identifier of the legacy handler.</summary>
  public string Uid => nameof(CoverletTestSessionHandler);
  /// <summary>Gets the extension assembly version, falling back to 1.0.0 when unavailable.</summary>
  public string Version => typeof(CoverletExtension).Assembly.GetName().Version?.ToString() ?? "1.0.0";
  /// <summary>Gets the handler name displayed by MTP.</summary>
  public string DisplayName => "Coverlet Coverage Session Handler";
  /// <summary>Gets the handler's advertised purpose; this does not imply implemented session callbacks.</summary>
  public string Description => "Flushes coverage data at end of test session";

  /// <summary>Reports that the legacy handler is enabled, independently of configuration.</summary>
  /// <returns>A completed task containing true. This is not a collection-capability check.</returns>
  public Task<bool> IsEnabledAsync() => Task.FromResult(true);

  Task ITestSessionLifetimeHandler.OnTestSessionStartingAsync(ITestSessionContext testSessionContext)
  {
    return ((ITestSessionLifetimeHandler)this).OnTestSessionStartingAsync(testSessionContext);
  }

  Task ITestSessionLifetimeHandler.OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
  {
    return ((ITestSessionLifetimeHandler)this).OnTestSessionFinishingAsync(testSessionContext);
  }

  /// <summary>Exposes the unimplemented legacy session-start callback.</summary>
  /// <param name="testSessionContext">The test session context.</param>
  /// <returns>This method throws before returning a task.</returns>
  /// <exception cref="NotImplementedException">Always thrown.</exception>
  public Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext)
  {
    throw new NotImplementedException();
  }

  /// <summary>Exposes the unimplemented legacy session-finish callback.</summary>
  /// <param name="testSessionContext">The test session context.</param>
  /// <returns>This method throws before returning a task.</returns>
  /// <exception cref="NotImplementedException">Always thrown.</exception>
  public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
  {
    throw new NotImplementedException();
  }

  private static Task OnTestSessionStartingAsync_Impl(ITestSessionContext testSessionContext)
  {
    return Task.CompletedTask;
  }

  private Task OnTestSessionFinishingAsync_Impl(ITestSessionContext testSessionContext)
  {
    foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
    {
      Type? injectedInstrumentationClass = GetInstrumentationClass(assembly);
      if (injectedInstrumentationClass is null)
      {
        continue;
      }

      MethodInfo? unloadModule = injectedInstrumentationClass.GetMethod(
          nameof(ModuleTrackerTemplate.UnloadModule),
          [typeof(object), typeof(EventArgs)]);

      unloadModule?.Invoke(null, [this, EventArgs.Empty]);
    }

    return Task.CompletedTask;
  }

  internal static Type? GetInstrumentationClass(Assembly assembly)
  {
    try
    {
      foreach (Type type in assembly.GetTypes())
      {
        if (type.Namespace == "Coverlet.Core.Instrumentation.Tracker"
            && type.Name.StartsWith(assembly.GetName().Name + "_"))
        {
          return type;
        }
      }

      return null;
    }
    catch (Exception)
    {
      return null;
    }
  }
}
