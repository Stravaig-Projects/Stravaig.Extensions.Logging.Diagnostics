using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
/// A logger that writes messages to a store that can later be examined
/// programatically, such as in unit tests.
/// </summary>
public class TestCaptureLogger : ITestCaptureLogger
{
    private readonly List<LogEntry> _logs = [];
#if NET9_0_OR_GREATER
    private readonly Lock _listGuard = new();
#else
    private readonly object _listGuard = new object();
#endif

    /// <summary>
    /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> class.
    /// </summary>
    public TestCaptureLogger()
    {
        CategoryName = string.Empty;
    }

    /// <summary>
    /// Initialises a new instance of the TestCaptureLogger class.
    /// </summary>
    /// <param name="categoryName">The name of the category</param>
    public TestCaptureLogger(string categoryName)
    {
        CategoryName = categoryName;
    }


    /// <inheritdoc />
    public string CategoryName { get; }


    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogs()
    {
        lock (_listGuard)
        {
            _logs.Sort();
            return _logs.ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogs(Func<LogEntry, bool> predicate)
    {
        lock (_listGuard)
        {
            return _logs
                .Where(predicate)
                .OrderBy(static log => log)
                .ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogEntriesWithExceptions()
        => GetLogs(log => log.Exception != null);

    /// <summary>
    /// Writes a log entry
    /// </summary>
    /// <param name="logLevel"></param>
    /// <param name="eventId"></param>
    /// <param name="state"></param>
    /// <param name="exception"></param>
    /// <param name="formatter"></param>
    /// <typeparam name="TState"></typeparam>
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
    {
        var formattedMessage = formatter(state, exception);
        var logEntry = CreateLogEntry(logLevel, eventId, state, exception, formattedMessage);
        lock (_listGuard)
        {
            _logs.Add(logEntry);
        }
    }

    /// <summary>
    /// Creates the LogEntry object.
    /// </summary>
    /// <param name="logLevel">The level the entry is logged at</param>
    /// <param name="eventId">The event id</param>
    /// <param name="state">The state (properties) of the log entry</param>
    /// <param name="exception">Any exception</param>
    /// <param name="formattedMessage">The formatted message</param>
    /// <typeparam name="TState">The object type that holds the state</typeparam>
    /// <returns>A log entry</returns>
    private LogEntry CreateLogEntry<TState>(LogLevel logLevel, EventId eventId, TState? state, Exception? exception, string formattedMessage)
    {
        return new LogEntry(logLevel, eventId, state, exception, formattedMessage, CategoryName);
    }

    /// <summary>
    /// Checks whether the given <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> is enabled.
    /// </summary>
    /// <param name="logLevel"></param>
    /// <returns>true if enabled; false otherwise.</returns>
    public bool IsEnabled(LogLevel logLevel) => true;

    /// <summary>
    /// Begins a logical operation scope.
    /// </summary>
    /// <param name="state"></param>
    /// <typeparam name="TState"></typeparam>
    /// <returns>A disposable object that ends the logical operation scope on dispose.</returns>
#if NET7_0_OR_GREATER
    // ReSharper disable once ReturnTypeCanBeNotNullable
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => DoNothing.Instance;
#else
        public IDisposable BeginScope<TState>(TState state)
            => DoNothing.Instance;
#endif

    private class DoNothing : IDisposable
    {
        public static readonly DoNothing Instance = new ();
        public void Dispose()
        { }
    }

    /// <inheritdoc />
    public void Reset()
    {
        lock (_listGuard)
        {
            _logs.Clear();
        }
    }
}
