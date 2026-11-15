---
uid: Coverlet.Core.Exceptions.CoverletException
example:
- *content
---
Preserve the cause of a coverage-tooling failure while reporting a useful message to the caller. This boundary converts an I/O failure into a Coverlet-specific exception and leaves the original exception available through InnerException.

```csharp
using System.IO;
using Coverlet.Core.Exceptions;

namespace CoverageTools.Reports;

public static class CoverageReportReader
{
    public static string Read(string reportPath)
    {
        try
        {
            return File.ReadAllText(reportPath);
        }
        catch (IOException cause)
        {
            throw new CoverletException($"Cannot read coverage report '{reportPath}'.", cause);
        }
    }
}
```
