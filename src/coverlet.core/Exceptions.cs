// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;

namespace Coverlet.Core.Exceptions
{
  /// <summary>
  /// Identifies a coverage-operation failure and optionally preserves its underlying cause.
  /// </summary>
  public class CoverletException : Exception
  {
    /// <summary>Initializes a coverage failure with the default exception message.</summary>
    public CoverletException() { }
    /// <summary>Initializes a coverage failure with a diagnostic message.</summary>
    /// <param name="message">The message describing the failure.</param>
    public CoverletException(string message) : base(message) { }
    /// <summary>Initializes a coverage failure while preserving its underlying cause.</summary>
    /// <param name="message">The message describing the failure.</param>
    /// <param name="inner">The exception that caused the failure.</param>
    public CoverletException(string message, System.Exception inner) : base(message, inner) { }
  }

  internal class CecilAssemblyResolutionException : CoverletException
  {
    public CecilAssemblyResolutionException() { }
    public CecilAssemblyResolutionException(string message) : base(message) { }
    public CecilAssemblyResolutionException(string message, System.Exception inner) : base(message, inner) { }
  }
}
