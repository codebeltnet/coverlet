// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using Coverlet.Core.Abstractions;

namespace Coverlet.Core.Helpers
{
  /// <summary>
  /// Writes coverage-tooling messages to the process standard output stream.
  /// </summary>
  public class SystemConsole : IConsole
  {
    /// <summary>Writes the supplied text followed by the current line terminator.</summary>
    /// <param name="value">The message to write.</param>
    public void WriteLine(string value)
    {
      Console.WriteLine(value);
    }
  }
}
