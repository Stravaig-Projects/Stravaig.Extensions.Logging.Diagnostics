---
layout: default
title: TestCaptureLogger class
---

# TestCaptureLogger

This class implements
* `ITestCaptureLogger` (See: [ITestCaptureLogger interface](i-test-capture-logger-interface.md))
* `ILogger`

# TestCaptureLogger methods

## GetLogs()

The `TestCaptureLogger.GetLogs()` allows you to retrieve the logs within your test method. The log entries will be in sequence, timestamps will be incremental, however adjacent log entries created sufficiently close to one another may contain the same timestamp due to the resolution of the clock.

#### Returns

`IReadOnlyList<LogEntry>`: A read only list of log entries. See [LogEntry](log-entry.md)

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)

---
## GetLogs(predicate)

The `TestCaptureLogger.GetLogs(predicate)` allows you to retrieve specific logs within your test method that match the predicate. The log entries will be in sequence, timestamps will be incremental, however adjacent log entries created sufficiently close to one another may contain the same timestamp due to the resolution of the clock.

#### Returns

`IReadOnlyList<LogEntry>`: A read only list of log entries. See [LogEntry](log-entry.md)

##### Example

This example checks that a specific log entry was generated.

```csharp
// Arrange
var logger = new TestCaptureLogger();

// Act: Do something using the logger

// Assert
var logs = logger.GetLogs(
    static le => le.LogLevel == LogLevel.Warning &&
        le.OriginalMessage == "A thing happened.");
logs.Count.ShouldBe(1);
```



---
## GetLogEntriesWithExceptions()

Gets all the log entries generated via this logger in sequential order that have exception objects attached to them.

```csharp
// Arrange
var logger = new TestCaptureLogger();

// Act: Do something using the logger

// Assert
var logs = logger.GetAllLogEntriesWithExceptions();
// logs is a read only list of LogEntry objects in sequential order.
```

#### Returns

`IReadOnlyList<LogEntry>`: A read only list of log entries. See [LogEntry](log-entry.md)

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)

---
## Reset()

Clears all the logs in the Logger.

If you reuse the same logger in multiple tests, be careful that the tests do not run in parallel with one another.