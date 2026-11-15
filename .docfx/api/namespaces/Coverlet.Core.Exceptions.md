---
uid: Coverlet.Core.Exceptions
summary: *content
---
`Coverlet.Core.Exceptions` lets integration tooling distinguish Coverlet failures from other application exceptions. Catch `CoverletException` at a coverage-operation boundary to report the failure message and retain the original cause when one is supplied.

Use this exception for diagnostics around existing integration code; it is not an entry point for running instrumentation. Normal package consumers enable coverage through the MTP command line.

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP>.
