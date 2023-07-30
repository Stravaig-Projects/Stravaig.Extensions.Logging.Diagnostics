---
layout: default
title: XUnit extensions
---

# XUnit Extensions

**Available from v2.1 in package [Stravaig.Extensions.Logging.Diagnostics.XUnit](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics.XUnit)**

## ITestOutputHelper.WriteLogs()

Writes the logs to XUnit's test output helper.

#### Arguments

* `IReadOnlyList<LogEntry> logEntries`: a list of log entries to render
* `Func<LogEntry, string>? formatter`: A function that formats the LogEntry to a string.

#### Example

```csharp
```