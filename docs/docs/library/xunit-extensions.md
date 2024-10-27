---
layout: default
title: XUnit extensions
---

# XUnit Extensions

**Available from v2.1 in package [Stravaig.Extensions.Logging.Diagnostics.XUnit](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics.XUnit)**

## ITestOutputHelper.WriteLogs()

Writes the logs to XUnit's test output helper.

#### Arguments

* `IEnumerable<LogEntry> logEntries`: a sequence of log entries to render
* `Func<LogEntry, string>? formatter`: (optional) A function that formats the LogEntry to a string. If omitted then the `Formatter.SimpleBySequence` formatter is used.

#### Example

```csharp
public class ThingamajigTests
{
    private ITestOutputHelper _output;

    public ThingamajigTests(ITestOutputHelper output)
    {
        _output = output;
    }

    [Fact]
    public void TestTheThing()
    {
        // Arrange: Set up logger or provider, e.g.
        var logProvider = new TestCaptureLoggerProvider();

        // Act: Perform the action that creates the log entries

        // Assert: Test the results
        _output.WriteLogs(logProvider.GetAllLogEntries())
    }
}
```

#### Variants

* `ITestOutputHelper.WriteLogs(ITestCaptureLogger logger, Func<LogEntry, string>? formatter = null)`
  * Available from v3.0
  * Takes an `ITestCaptureLogger` and outputs all of the log messages captured by that logger.
* `ITestOutputHelper.WriteLogs(TestCaptureLoggerProvider provider, Func<LogEntry, string>? formatter = null)`
  * Available from v3.0
  * Takes an `TestCaptureLoggerProvider` and outputs all of the log messages captured by all the loggers in the provider.
