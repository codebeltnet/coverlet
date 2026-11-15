---
uid: Coverlet.MTP.Configuration
summary: *content
---
`Coverlet.MTP.Configuration` describes coverage filters, report formats and thresholds for the MTP integration. Inspect `CoverletMTPSettings` when diagnosing the effective coverage configuration; setting properties on a standalone instance does not apply those settings to a running test application. Normal consumers configure the package with command-line options or `coverlet.mtp.appsettings.json`.

Start with <xref:Coverlet.MTP.CoverletExtensionProvider> for complete extension registration. `CoverletConfigurationExtensions.AddCoverletCodeCoverage` registers only the legacy session handler: its callbacks are not implemented, so it is not a working alternative to full registration.

## Extension Members

| Type | Ext | Methods |
| --- | --- | --- |
| `ITestApplicationBuilder` | ⬇️ | `AddCoverletCodeCoverage` |

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP>, <xref:Coverlet.MTP.InProcDataCollection>.
