---
uid: Coverlet.Core.Helpers
summary: *content
---
`Coverlet.Core.Helpers` exposes `SystemConsole` for writing coverage-related messages to standard output. Use it when maintaining an integration that already relies on the engine's console adapter; its `WriteLine` forwards the supplied text to `Console.WriteLine`.

The other engine helpers are internal implementation details. For collecting coverage in a test project, use the MTP package's options rather than constructing engine helpers.

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP>.
