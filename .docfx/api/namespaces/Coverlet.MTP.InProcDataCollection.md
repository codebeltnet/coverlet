---
uid: Coverlet.MTP.InProcDataCollection
summary: *content
---
`Coverlet.MTP.InProcDataCollection` exposes the legacy `CoverletTestSessionHandler` session integration. Use this page when maintaining code that already references that handler, rather than selecting it for a new coverage setup: its public start and finish callbacks throw `NotImplementedException`, and its explicit interface callbacks recursively dispatch to themselves.

Register coverage through <xref:Coverlet.MTP.CoverletExtensionProvider> instead. That path selects the active in-process handler together with the controller instrumentation and report collection components. The legacy handler's enabled status does not establish that it can collect coverage.

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP.Configuration>.
