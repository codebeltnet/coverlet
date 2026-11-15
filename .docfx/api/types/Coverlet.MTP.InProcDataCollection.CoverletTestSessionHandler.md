---
uid: Coverlet.MTP.InProcDataCollection.CoverletTestSessionHandler
example:
- *content
---
Identify an existing legacy session-handler registration during a migration. The enabled check currently returns true, but the public session callbacks throw NotImplementedException and the explicit interface callbacks recurse; this diagnostic deliberately does not execute them. Replace legacy registration with AddCoverletExtensionProvider to use the active collection path.

```csharp
using System;
using System.Threading.Tasks;
using Coverlet.MTP.InProcDataCollection;

namespace CoverageTools.Migration;

public static class LegacyHandlerAudit
{
    public static async Task PrintAsync(CoverletTestSessionHandler handler)
    {
        bool enabled = await handler.IsEnabledAsync();
        Console.WriteLine($"{handler.DisplayName}: enabled={enabled}");
        Console.WriteLine("Replace this legacy registration before executing test sessions.");
    }
}
```
