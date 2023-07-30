# Stravaig.Extensions.Logging.Diagnostics

A logger for use in tests so that the messages logged can be examined in tests.

* ![Nuget](https://img.shields.io/nuget/v/Stravaig.Extensions.Logging.Diagnostics?color=004880&label=nuget%20stable&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)
* ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Stravaig.Extensions.Logging.Diagnostics?color=ffffff&label=nuget%20latest&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)

Current version supports: 6.0, 7.0

.NET Core 3.1 and .NET 5.0 support was dropped as of v2.0. Use v1.x when targeting .NET Core 3.1 or .NET 5.0.

## Why test logs?

This was originally developed to test that background services were emitting logs in certain scenarios. Since the logs are the one of the primary views of how a background service is working it is essentially a first class output of the service. The user interface of the service, if you prefer, where the user is the developer or support technician attempting a diagnose an issue.

It can also be useful to test that structured logs emit the correct values so that values used when querying a logging sinks (such as ElasticSearch or New Relic) are available and correct.

### Other reasons to capture logs

This library also supports rendering the logs it captures, filtered in any way you see fit. This can be useful in scenarios, such as calling an ASP.NET application that will emit a deluge of logs that you are unlikely to be interested as it spins up the hosted server within the test.

## Usage

For details on usage see the [Stravaig Logging Diagnostics documentation](https://stravaig-projects.github.io/Stravaig.Extensions.Logging.Diagnostics/).

















### TestCaptureLoggerProvider methods

#### GetCategories()

Gets all the logger categories created by this provider. NOTE: The category may not contain any logs.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var categories = logProvider.GetCategories();
// categories is a read only list of strings.
```

#### GetAllLogEntries()

Gets all the LogEntry objects generated via this provider in sequential order.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var allLogs = logProvider.GetAllLogEntries();
// allLogs is a read only list of LogEntry objects in sequential order.
```

#### GetAllLogEntriesWithExceptions()

Gets all the log entries generated via this provider in sequential order that have exception objects attached to them.

```csharp
// Arrange
var logProvider = new TestCaptureLoggerProvider();

// Act... Do something using the logProvider

// Assert
var logs = logProvider.GetAllLogEntriesWithExceptions();
// logs is a read only list of LogEntry objects in sequential order.
```

#### Reset()

Resets the provider and discards all the logs captured to that point.

```csharp
var logProvider = new TestCaptureLoggerProvider();

// Create log entries here...

logProvider.Reset();
var logs = logProvider.GetAllLogEntries();
// logs is an empty array.
```

### IEnumerable<LogEntry>.RenderLogs()

This is an extension method in the `Stravaig.Extensions.Logging.Diagnostics.Renderer` namespace.

The implied object is an `IEnumerable<LogEntry>` so it will operate over any iterable collection of `LogEntry` objects.

The method takes two parameters, a formatting function and a sink action. The formatting function takes the LogEntry and formats it into a string. The sink action takes the formatted string and writes it to some output mechanism such as the console, or a debug trace listener.

e.g.

```csharp
logger.GetLogs()
      .RenderLogs(
          le => $"{le.Sequence}: {le.FormattedMessage}",
          msg => Debug.WriteLine(msg));
```

There are some built in Formatters and Sinks to get started with.

```csharp
logger.GetLogs()
      .RenderLogs(Formatter.SimpleBySequence, Sink.Console);
```

### Example project

In this repository there is an example project showing how this may be used in NUnit tests.

* [Example Solution](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/tree/main/Example)

## Rendering Logs on XUnit

To render logs while using XUnit test framework, there is an extension to the `ITestOutputHelper` to assist.

You will need to install the `Stravaig.Extensions.Logging.Diagnostics.XUnit` package.

In your test you can render logs to the test output helper from any `IReadOnlyList<LogEntry>`. e.g.

```csharp
var logEntries = logger.GetLogs();
outputHelper.RenderLogs(logEntries);
```

## Feedback, Issues, and Questions

You can create a [GitHub issue](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/issues) to leave bug reports, ask questions, or feature requests.

## Contributing / Getting Started

* Ensure you have PowerShell 7.1.x (or higher, except Windows) installed.
* At a PowerShell prompt
  * Navigate to the root of this repository
  * Run `./Install-GitHooks.ps1`
* Name the branch after the issue number. e.g. `#1234/fix-the-thing`. The Git Hooks will prefix each commit with the issue number.