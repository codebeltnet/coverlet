---
uid: Coverlet.MTP.CoverletExtensionProvider
example:
- *content
---
Register the complete Coverlet extension on a custom MTP builder before adding your test framework and building the application. The returned builder has controller instrumentation, host flushing and coverage command-line support registered. Use this only when package-generated discovery has not already registered Coverlet; the usual package workflow needs no manual call.

```csharp
using System.Threading.Tasks;
using Coverlet.MTP;
using Microsoft.Testing.Platform.Builder;

namespace CoverageTools.TestRunner;

public static class CoverageApplicationBuilder
{
    public static async Task<ITestApplicationBuilder> CreateAsync(string[] args)
    {
        ITestApplicationBuilder builder = await TestApplication.CreateBuilderAsync(args);
        builder.AddCoverletExtensionProvider();
        return builder;
    }
}
```
