# Rendering Extensions

Extensions to help render captured logs in to the test output.

## IEnumerable<LogEntry>.RenderLogs()

#### Arguments

* `Func<LogEntry, string> format`: A function that formats the log entry into a string.
* `Action<string> writeToSink`: A function that writed the formatted string to the rendered output.

#### Examples

```csharp
// Arrange
var logger = new TestCaptureLogger();

// Act: Do something using the logger

// Assert
var logs = logger.GetLogs();
logs.RenderLogs(
    le => $"[{le.Sequence} {le.LogLevel}] {le.FormattedMessage}",
    s => Console.WriteLine(s));
```

#### Remarks

There are some basic formatters already set up on the `Formatter` class. There is also a Console sink created on the `Sink` class. They can be used like the following:

```csharp
logs.RenderLogs(Formatter.SimpleSequential, Sink.Console);
```

