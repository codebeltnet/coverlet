---
uid: ConsoleTables
summary: *content
---
`ConsoleTables` exposes the table options and output-format values retained from the engine's vendored console formatter. These are compatibility types, not a public table-rendering workflow: the `ConsoleTable` renderer itself is internal.

Inspect `ConsoleTableOptions` for column names and row-count preferences, or `Format` for the available layout values when maintaining code that already exchanges those values. Neither type configures MTP coverage reports; choose coverage report formats through `--coverlet-output-format` instead.

[!INCLUDE [availability-default](../../includes/availability-default.md)]

Related: <xref:Coverlet.MTP.Configuration>.
