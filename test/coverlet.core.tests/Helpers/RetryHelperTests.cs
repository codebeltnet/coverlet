// Copyright (c) Toni Solarin-Sodara
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.IO;
using Coverlet.Core.Helpers;
using Xunit;

namespace Coverlet.Core.Tests.Helpers
{
  public class RetryHelperTests
  {
    [Fact]
    public void TestRetryWithFixedRetryBackoff()
    {
      static TimeSpan retryStrategy()
      {
        return TimeSpan.FromMilliseconds(1);
      }

      var target = new RetryTarget();
      try
      {
        new RetryHelper().Retry(() => target.TargetActionThrows(), retryStrategy, 7);
      }
      catch
      {
        Assert.Equal(7, target.Calls);
      }
    }

    [Fact]
    public void TestRetryWithExponentialRetryBackoff()
    {
      int currentSleep = 6;
      TimeSpan retryStrategy()
      {
        var sleep = TimeSpan.FromMilliseconds(currentSleep);
        currentSleep *= 2;
        return sleep;
      }

      var target = new RetryTarget();
      try
      {
        new RetryHelper().Retry(() => target.TargetActionThrows(), retryStrategy, 3);
      }
      catch
      {
        Assert.Equal(3, target.Calls);
        Assert.Equal(24, currentSleep);
      }
    }

    [Fact]
    public void TestRetryFinishesIfSuccessful()
    {
      static TimeSpan retryStrategy()
      {
        return TimeSpan.FromMilliseconds(1);
      }

      var target = new RetryTarget();
      new RetryHelper().Retry(() => target.TargetActionThrows5Times(), retryStrategy, 20);
      Assert.Equal(6, target.Calls);
    }

    [Fact]
    public void TestRetryTreatsUnauthorizedAccessLikeSharingViolation()
    {
      // Windows reports a rename/delete against a still-mapped (loaded) file as ERROR_ACCESS_DENIED,
      // which surfaces as UnauthorizedAccessException rather than IOException; it must be retried the same way.
      static TimeSpan retryStrategy()
      {
        return TimeSpan.FromMilliseconds(1);
      }

      var target = new RetryTargetUnauthorizedAccessException();
      var exception = Assert.Throws<AggregateException>(() => new RetryHelper().Retry(() => target.TargetActionThrows(), retryStrategy, 4));

      Assert.Equal(4, target.Calls);
      Assert.All(exception.InnerExceptions, inner => Assert.IsType<UnauthorizedAccessException>(inner));
    }
  }

  public class RetryTargetUnauthorizedAccessException
  {
    public int Calls { get; set; }
    public void TargetActionThrows()
    {
      Calls++;
      throw new UnauthorizedAccessException("Simulating Failure");
    }
  }

  public class RetryTarget
  {
    public int Calls { get; set; }
    public void TargetActionThrows()
    {
      Calls++;
      throw new IOException("Simulating Failure");
    }
    public void TargetActionThrows5Times()
    {
      Calls++;
      if (Calls < 6) throw new IOException("Simulating Failure");
    }
  }

  public class RetryTargetIOException
  {
    public int Calls { get; set; }
    public void TargetActionThrows()
    {
      Calls++;
      throw new IOException("Simulating Failure");
    }
    public void TargetActionThrows5Times()
    {
      Calls++;
      if (Calls < 6) throw new Exception("Simulating Failure");
    }
  }
}
