---
uid: Coverlet.MTP.Diagnostics
summary: *content
---
`Coverlet.MTP.Diagnostics` helps investigate missing coverage and instrumentation failures through named environment switches. Use `CoverletMtpDebugConstants` when preparing the environment for a diagnostic test run, rather than duplicating the switch names in your tooling.

Enable tracker logging only for the investigation and restore the previous environment afterward. Debugger launch and wait switches require an interactive debugger workflow and can interrupt unattended runs.

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP>.
