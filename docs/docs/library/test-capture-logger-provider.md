---
layout: default
title: TestCaptureLoggerProvider class
---

# TestCaptureLoggerProvider methods

The provider passed to the logging framework.

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

* GetLogEntriesFor(string categoryName)
* GetLogEntriesFor(type type)
* GetLogEntriesFor<T>()

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
// logs is an empty array.
```