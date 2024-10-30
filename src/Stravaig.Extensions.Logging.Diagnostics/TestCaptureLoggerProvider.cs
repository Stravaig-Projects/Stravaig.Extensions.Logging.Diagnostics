using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using static Stravaig.Extensions.Logging.Diagnostics.ExternalHelpers.TypeNameHelper;

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
/// A provider of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> instances.
/// </summary>
public class TestCaptureLoggerProvider : ILoggerProvider, ICapturedLogs
{
    private readonly ConcurrentDictionary<string, TestCaptureLogger> _captures;
    private readonly ConcurrentDictionary<Type, ITestCaptureLogger> _typedCaptures;

    /// <summary>
    /// Creates an instance of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLoggerProvider"/>
    /// </summary>
    public TestCaptureLoggerProvider()
    {
        _captures = new ConcurrentDictionary<string, TestCaptureLogger>();
        _typedCaptures = new ConcurrentDictionary<Type, ITestCaptureLogger>();
    }

    /// <summary>
    /// Gets a list of log entries captured in the specified category.
    /// </summary>
    /// <param name="categoryName">The category name under which logs were captured.</param>
    /// <returns>The list of log entries captured, empty if none.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesFor(string categoryName)
    {
        return _captures.TryGetValue(categoryName, out TestCaptureLogger? logger)
            ? logger.GetLogs()
            : Array.Empty<LogEntry>();
    }

    /// <summary>
    /// Gets a list of log entries captured in the specified category.
    /// </summary>
    /// <param name="type">The type which forms the category name for the logger.</param>
    /// <returns>The list of log entries captured, empty if none.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesFor(Type type)
    {
        var categoryName = GetTypeDisplayName(type, includeGenericParameters: false, nestedTypeDelimiter: '.');
        return GetLogEntriesFor(categoryName);
    }

    /// <summary>
    /// Gets a list of log entries captured in the specified category.
    /// </summary>
    /// <typeparam name="T">The type which forms the category name for the logger.</typeparam>
    /// <returns>The list of log entries captured, empty if none.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesFor<T>()
    {
        return GetLogEntriesFor(typeof(T));
    }

    ILogger ILoggerProvider.CreateLogger(string categoryName)
        => CreateLogger(categoryName);

    /// <summary>
    /// Creates a new <see cref="T:TestCaptureLogger"/> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The instance of the <see cref="TestCaptureLogger"/> that was created.</returns>
    public TestCaptureLogger CreateLogger(string categoryName)
        => _captures.GetOrAdd(categoryName, static (cn) => new TestCaptureLogger(cn));

    /// <summary>
    /// Creates a new <see cref="T:TestCaptureLogger"/> instance.
    /// </summary>
    /// <typeparam name="T">The type that the logger is assigned to.</typeparam>
    /// <returns>The instance of the <see cref="TestCaptureLogger{TCategoryType}"/> that was created.</returns>
    public TestCaptureLogger<T> CreateLogger<T>()
        => (TestCaptureLogger<T>)_typedCaptures.GetOrAdd(typeof(T), type =>
        {
            var categoryName = GetTypeDisplayName(typeof(T));
            var underlyingLogger = CreateLogger(categoryName);
            return new TestCaptureLogger<T>(underlyingLogger);
        });

    /// <summary>
    /// Gets a list of log categories that were set up by this provider.
    /// </summary>
    /// <returns>A read-only list of category names.</returns>
    public IReadOnlyList<string> GetCategories()
    {
        return _captures.Keys.ToArray();
    }

    /// <summary>
    /// Gets all log entries regardless of the Category they were logged as.
    /// </summary>
    /// <returns>A read only list of <see cref="LogEntry"/></returns>
    public IReadOnlyList<LogEntry> GetAllLogEntries()
    {
        var loggers = _captures.Values;
        var allLogs = loggers.SelectMany(l => l.GetLogs()).ToList();
        allLogs.Sort();
        return allLogs;
    }

    /// <summary>
    /// Gets all log entries matching the predicate regardless of the Category
    /// they were logged as.
    /// </summary>
    /// <returns>A read only list of <see cref="LogEntry"/> objects.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesMatchingPredicate(Func<LogEntry, bool> predicate)
    {
        var loggers = _captures.Values;
        var allLogs = loggers.SelectMany(l => l.GetLogs(predicate)).ToList();
        allLogs.Sort();
        return allLogs;
    }


    /// <summary>
    /// Gets all log entries with exceptions attached regardless of the
    /// Category they were logged as.
    /// </summary>
    /// <returns>A read only list of <see cref="LogEntry"/> objects.</returns>
    public IReadOnlyList<LogEntry> GetAllLogEntriesWithExceptions()
        => GetLogEntriesMatchingPredicate(static log => log.Exception != null);

    /// <summary>
    /// Resets the captures to an empty state.
    /// </summary>
    public void Reset()
    {
        foreach (var logger in _captures.Values)
        {
            logger.Reset();
        }
    }

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        Reset();
    }
}
