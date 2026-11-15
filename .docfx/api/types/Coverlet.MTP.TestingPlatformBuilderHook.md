---
uid: Coverlet.MTP.TestingPlatformBuilderHook
example:
- *content
---
Connect a custom discovery adapter to the same registration hook used by the package build assets. The adapter receives an existing MTP builder and forwards its arguments to the hook; the hook currently ignores those arguments. Call this once, before application construction, and let your runner register its test framework separately.

```csharp
using System;
using Coverlet.MTP;
using Microsoft.Testing.Platform.Builder;

namespace CoverageTools.Discovery;

public static class CoverageHookAdapter
{
    public static void Register(ITestApplicationBuilder builder, string[] args)
    {
        ArgumentNullException.ThrowIfNull(builder);
        TestingPlatformBuilderHook.AddExtensions(builder, args);
        Console.WriteLine("Coverlet discovery hook registered.");
    }
}
```
