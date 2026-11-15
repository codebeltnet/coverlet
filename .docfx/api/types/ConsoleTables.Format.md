---
uid: ConsoleTables.Format
summary: Selects a text layout for the bundled internal table renderer; these values do not select coverage report formats.
example:
- *content
---
Translate a retained console-table format into a file extension for an export destination. Markdown is the only value here mapped to a Markdown file; the other layouts use plain text. This is an application-side choice and does not change the coverage report format.

```csharp
using System;
using System.IO;
using ConsoleTables;

namespace CoverageTools.Reporting;

public static class TableExportPath
{
    public static string Resolve(string directory, Format format)
    {
        string extension = format switch
        {
            Format.MarkDown => ".md",
            Format.Default or Format.Alternative or Format.Minimal => ".txt",
            _ => throw new ArgumentOutOfRangeException(nameof(format))
        };
        string path = Path.Combine(directory, "coverage-summary" + extension);
        Console.WriteLine($"Table export destination: {path}");
        return path;
    }
}
```

---
uid: ConsoleTables.Format.Default
summary: *content
---
Uses the standard table layout, including a row count when enabled in the table options.

---
uid: ConsoleTables.Format.MarkDown
summary: *content
---
Uses a pipe-delimited Markdown table with a header separator.

---
uid: ConsoleTables.Format.Alternative
summary: *content
---
Uses a text table with plus signs at border intersections.

---
uid: ConsoleTables.Format.Minimal
summary: *content
---
Uses aligned text columns without pipe delimiters.
