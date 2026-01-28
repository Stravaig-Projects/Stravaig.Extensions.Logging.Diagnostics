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
    private readonly IExternalScopeProvider _scopeProvider;
    private bool _needsSort;
    private bool _hasLastSequence;
    private int _lastSequence;
#if NET9_0_OR_GREATER
    private readonly Lock _listGuard = new();
#else
    private readonly object _listGuard = new();
#endif

    /// <summary>
    /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> class.
    /// </summary>
    public TestCaptureLogger()
        : this(string.Empty, new LoggerExternalScopeProvider())
    { }

    /// <summary>
    /// Initialises a new instance of the TestCaptureLogger class.
    /// </summary>
    /// <param name="categoryName">The name of the category</param>
    public TestCaptureLogger(string categoryName)
        : this(categoryName, new LoggerExternalScopeProvider())
    { }

    /// <summary>
    /// Initialises a new instance of the TestCaptureLogger class.
    /// </summary>
    /// <param name="categoryName">The name of the category</param>
    /// <param name="scopeProvider">The scope provider to use.</param>
    public TestCaptureLogger(string categoryName, IExternalScopeProvider scopeProvider)
    {
        ArgumentNullException.ThrowIfNull(scopeProvider);
        CategoryName = categoryName;
        _scopeProvider = scopeProvider;
    }


    /// <inheritdoc />
    public string CategoryName { get; }


    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogs()
    {
        lock (_listGuard)
        {
            EnsureSortedNoLock();
            return _logs.ToArray();
        }
    }

    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogs(Func<LogEntry, bool> predicate)
    {
        lock (_listGuard)
        {
            EnsureSortedNoLock();
            return _logs.Where(predicate).ToArray();
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
            if (_hasLastSequence)
            {
                if (logEntry.Sequence < _lastSequence)
                    _needsSort = true;
            }
            else
            {
                _hasLastSequence = true;
            }
            _lastSequence = logEntry.Sequence;
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
        var scopes = CaptureScopeStates(state);
        return new LogEntry(logLevel, eventId, state, exception, formattedMessage, CategoryName, scopes);
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
    /// <returns>A disposable object that ends the logical operation scope when Dispose is called.</returns>
    // ReSharper disable once ReturnTypeCanBeNotNullable
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => _scopeProvider.Push(state);

    /// <inheritdoc />
    public void Reset()
    {
        lock (_listGuard)
        {
            _logs.Clear();
            _needsSort = false;
            _hasLastSequence = false;
            _lastSequence = 0;
        }
    }

    private void EnsureSortedNoLock()
    {
        if (!_needsSort)
            return;

        _logs.Sort();
        _needsSort = false;
        UpdateLastSequenceNoLock();
    }

    private void UpdateLastSequenceNoLock()
    {
        if (_logs.Count == 0)
        {
            _hasLastSequence = false;
            _lastSequence = 0;
            return;
        }

        _hasLastSequence = true;
        _lastSequence = _logs[^1].Sequence;
    }

    private List<object?> CaptureScopeStates<TState>(TState? state)
    {
        // Create a list to capture scope states and add the outermost to
        // innermost (the state from the log operation itself is the
        // innermost state.)
        var scopesList = new List<object?>();
        _scopeProvider.ForEachScope(static (scope, list) => list.Add(scope), scopesList);
        scopesList.Add(state);
        return scopesList;
    }
}
