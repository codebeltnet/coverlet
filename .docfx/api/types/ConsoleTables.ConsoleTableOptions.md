---
uid: ConsoleTables.ConsoleTableOptions
summary: Stores column headings and the row-count preference for the bundled internal table renderer.
example:
- *content
---
Inspect a retained table configuration before passing it through existing integration code. This prints the requested columns and row-count preference; the bundled table renderer is internal, so these options do not provide a standalone rendering API.

```csharp
using System;
using ConsoleTables;

namespace CoverageTools.Reporting;

public static class TablePreferences
{
    public static void Print()
    {
        var options = new ConsoleTableOptions
        {
            Columns = new[] { "Module", "Line coverage" },
            EnableCount = false
        };
        Console.WriteLine(string.Join(" | ", options.Columns));
        Console.WriteLine($"Include row count: {options.EnableCount}");
    }
}
```

---
uid: ConsoleTables.ConsoleTableOptions.Columns
summary: *content
---
Gets or sets the column headings. A new options instance starts with an empty collection.

---
uid: ConsoleTables.ConsoleTableOptions.EnableCount
summary: *content
---
Gets or sets whether the default table layout appends the number of rows. The default is `true`.
