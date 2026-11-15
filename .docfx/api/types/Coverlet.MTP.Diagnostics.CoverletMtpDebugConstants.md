---
uid: Coverlet.MTP.Diagnostics.CoverletMtpDebugConstants
example:
- *content
---
Run a diagnostic operation with tracker logging enabled, then restore the previous process environment even if it fails. Invoke this wrapper before the test process is created so the child can inherit the switch. Environment changes are process-wide, so serialize this operation with other tooling that changes the same variable.

```csharp
using System;
using Coverlet.MTP.Diagnostics;

namespace CoverageTools.Diagnostics;

public static class TrackerLogging
{
    public static void Run(Action launchDiagnosticRun)
    {
        string variable = CoverletMtpDebugConstants.EnableTrackerLog;
        string? previous = Environment.GetEnvironmentVariable(variable);
        try
        {
            Environment.SetEnvironmentVariable(variable, "1");
            launchDiagnosticRun();
        }
        finally
        {
            Environment.SetEnvironmentVariable(variable, previous);
        }
    }
}
```
