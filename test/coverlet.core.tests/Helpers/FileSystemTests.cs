// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Coverlet.Core.Helpers;
using Xunit;

namespace Coverlet.Core.Tests.Helpers
{
  public class FileSystemTests
  {
    [Fact]
    public void TestMoveWithOverwriteReplacesExistingDestination()
    {
      string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(tempDir);

      try
      {
        string source = Path.Combine(tempDir, "source.txt");
        string destination = Path.Combine(tempDir, "destination.txt");
        File.WriteAllText(source, "new");
        File.WriteAllText(destination, "old");

        new FileSystem().Move(source, destination, true);

        Assert.False(File.Exists(source));
        Assert.Equal("new", File.ReadAllText(destination));
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Fact]
    public void TestMoveWithoutOverwriteThrowsWhenDestinationExists()
    {
      string tempDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
      Directory.CreateDirectory(tempDir);

      try
      {
        string source = Path.Combine(tempDir, "source.txt");
        string destination = Path.Combine(tempDir, "destination.txt");
        File.WriteAllText(source, "new");
        File.WriteAllText(destination, "old");

        Assert.Throws<IOException>(() => new FileSystem().Move(source, destination, false));

        Assert.Equal("new", File.ReadAllText(source));
        Assert.Equal("old", File.ReadAllText(destination));
      }
      finally
      {
        Directory.Delete(tempDir, true);
      }
    }

    [Theory]
    [InlineData(null, null)]
    [InlineData("", "")]
    [InlineData("filename.cs", "filename.cs")]
    [InlineData("filename{T}.cs", "filename{{T}}.cs")]
    public void TestEscapeFileName(string fileName, string expected)
    {
      string actual = FileSystem.EscapeFileName(fileName);

      Assert.Equal(expected, actual);
    }
  }
}
