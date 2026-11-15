---
uid: Coverlet.Core.Helpers.SystemConsole
example:
- *content
---
Write a coverage report location to standard output using the engine console adapter. The message is emitted immediately by WriteLine, allowing command-line tooling to direct users to the generated report.

```csharp
using Coverlet.Core.Helpers;

namespace CoverageTools.Reporting;

public static class ReportLocation
{
    public static void Print(string reportPath)
    {
        var console = new SystemConsole();
        console.WriteLine($"Coverage report: {reportPath}");
    }
}
```
