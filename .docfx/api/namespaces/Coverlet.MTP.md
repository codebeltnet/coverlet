---
uid: Coverlet.MTP
summary: *content
---
`Coverlet.MTP` connects coverage instrumentation and report collection to a Microsoft Testing Platform test application. Use the `Codebelt.Coverlet.MTP` package and `--coverlet` for normal test runs; its build assets arrange extension discovery without a handwritten registration call.

For a custom test application builder, `CoverletExtensionProvider.AddCoverletExtensionProvider` registers the controller, test-host and command-line components. `TestingPlatformBuilderHook` is the package discovery entry point and delegates to the same registration. Register once; a custom builder owns application construction and disposal, while the extension owns coverage collection during that application's execution. This is an MTP extension, rather than a replacement test framework or a VSTest collector. See Microsoft's [extension guidance](https://learn.microsoft.com/en-us/dotnet/core/testing/microsoft-testing-platform-architecture-extensions).

## Extension Members

| Type | Ext | Methods |
| --- | --- | --- |
| `ITestApplicationBuilder` | ⬇️ | `AddCoverletExtensionProvider` |

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP.Configuration>, <xref:Coverlet.MTP.Diagnostics>.
