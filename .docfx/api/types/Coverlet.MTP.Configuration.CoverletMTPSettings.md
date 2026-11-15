---
uid: Coverlet.MTP.Configuration.CoverletMTPSettings
example:
- *content
---
Print a coverage-settings snapshot to help diagnose filters and threshold expectations. This object describes requested values and its ToString output exposes the configured filters and threshold; constructing it does not configure the MTP runner. Apply actual run settings through the package command line or configuration file.

```csharp
using System;
using Coverlet.MTP.Configuration;

namespace CoverageTools.Diagnostics;

public static class CoverageSettingsSnapshot
{
    public static void Print(string testModule)
    {
        var settings = new CoverletMTPSettings
        {
            TestModule = testModule,
            ReportFormats = new[] { "json", "cobertura" },
            IncludeFilters = new[] { "[Orders]*" },
            Threshold = 80
        };
        Console.WriteLine(settings.ToString());
        Console.WriteLine("Report formats: " + string.Join(", ", settings.ReportFormats));
    }
}
```
