---
layout: default
title: TestCaptureLoggerProvider class
---

# TestCaptureLoggerProvider methods

The provider passed to the logging framework.

This class implements
* `ICapturedLogs`
* `ILoggingProvider`

## CreateLogger&lt;T>

Creates or returns an exsting `TestCaptureLogger<T>`. This is a generic alternative to the non-generic version from `ILoggerProvider`.

## GetCategories()

Gets all the logger categories created by this provider. NOTE: There is the possibility that the category may not contain any logs.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var categories = logProvider.GetCategories();
// categories is a read only list of strings.
```

#### Returns

An `IReadOnlyList<string>` representing a list of categories created in the provider.

---
## GetLogEntriesFor()

Gets the log entries for the specified category.

### Variants

* `GetLogEntriesFor(string categoryName)`
* `GetLogEntriesFor(type type)`
* `GetLogEntriesFor<T>()`

#### Returns

`IReadOnlyList<LogEntry>`: A list of log entries in sequence. See [LogEntry](log-entry.md).

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)


---
## GetAllLogEntries()

Gets all the `LogEntry`` objects generated via this provider in sequential order.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var allLogs = logProvider.GetAllLogEntries();
// allLogs is a read only list of LogEntry objects in sequential order.
```

#### Returns

`IReadOnlyList<LogEntry>`: A list of log entries in sequence. See [LogEntry](log-entry.md).

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)

---

## GetLogs(predicate)

The `TestCaptureLoggerProvider.GetLogs(predicate)` allows you to retrieve specific logs within your test method that match the predicate. The log entries will be in sequence, timestamps will be incremental, however adjacent log entries created sufficiently close to one another may contain the same timestamp due to the resolution of the clock.

#### Returns

`IReadOnlyList<LogEntry>`: A read only list of log entries. See [LogEntry](log-entry.md)

##### Example

This example checks that a specific log entry was generated.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act: Do something using the log provider.

// Assert
var logs = logProvider.GetLogs(
    static le => le.LogLevel == LogLevel.Warning &&
        le.OriginalMessage == "A thing happened.");
logs.Count.ShouldBe(1);
```

---
## GetAllLogEntriesWithExceptions()

Gets all the log entries generated via this provider in sequential order that have exception objects attached to them.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var logs = logProvider.GetAllLogEntriesWithExceptions();
// logs is a read only list of LogEntry objects in sequential order.
```

#### Returns

`IReadOnlyList<LogEntry>`: A list of log entries in sequence. See [LogEntry](log-entry.md).

##### Remarks

The result of the method can be passed into `RenderLogs()` extension method. See [Renderer](log-entry-renderer-extensions.md)

---
## Reset()

Resets the provider and discards all the logs captured to that point.

```csharp
var logProvider = new TestCaptureLoggerProvider();

// Create log entries here...

logProvider.Reset();
var logs = logProvider.GetAllLogEntries();
logs.Count.ShouldBe(0); // logs is an empty list.
```