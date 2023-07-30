---
layout: default
title: TestCaptureLogger class
---

# TestCaptureLogger methods

## GetLogs()

The `TestCaptureLogger.GetLogs()` allows you to retrieve the logs within your test method. The property will be in sequence, timestamps will be incremental, however adjacent log entries created sufficiently close to one another may contain the same timestamp due to the resolution of the clock.

#### Returns

`IReadOnlyList<LogEntry>`: A read only list of log entries. See [LogEntry](log-entry.md)

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)


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