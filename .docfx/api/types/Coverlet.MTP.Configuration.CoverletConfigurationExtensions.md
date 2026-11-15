---
uid: Coverlet.MTP.Configuration.CoverletConfigurationExtensions
example:
- *content
---
Keep legacy registration behind an explicit compatibility switch when maintaining a custom builder. AddCoverletCodeCoverage registers only CoverletTestSessionHandler, whose session callbacks are not implemented; this example therefore rejects that mode by default and does not run an application. For working coverage collection, use AddCoverletExtensionProvider instead of enabling this switch.

```csharp
using System;
using Coverlet.MTP.Configuration;
using Microsoft.Testing.Platform.Builder;

namespace CoverageTools.Compatibility;

public static class LegacyCoverageRegistration
{
    public static void Register(ITestApplicationBuilder builder, bool allowLegacyHandler)
    {
        if (!allowLegacyHandler)
        {
            throw new NotSupportedException("Use full Coverlet extension registration for coverage collection.");
        }
        builder.AddCoverletCodeCoverage();
        Console.WriteLine("Legacy handler registered; session execution is unsupported.");
    }
}
```
