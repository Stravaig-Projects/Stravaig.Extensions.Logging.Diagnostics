using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using Stravaig.Extensions.Logging.Diagnostics.Extensions;
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
            : [];
    }

    /// <summary>
    /// Gets a list of log entries captured in the specified category.
    /// </summary>
    /// <param name="type">The type which forms the category name for the logger.</param>
    /// <returns>The list of log entries captured, empty if none.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesFor(Type type)
    {
        var categoryName = type.AsCategoryName();
        return GetLogEntriesFor(categoryName);
    }

    /// <summary>
    /// Gets a list of log entries captured in the specified category.
    /// </summary>
    /// <typeparam name="T">The type which forms the category name for the logger.</typeparam>
    /// <returns>The list of log entries captured, empty if none.</returns>
    public IReadOnlyList<LogEntry> GetLogEntriesFor<T>()
    {
        return GetLogEntriesFor(TestCaptureLogger<T>.CategoryNameForType);
    }

    ILogger ILoggerProvider.CreateLogger(string categoryName)
        => CreateLogger(categoryName);

    /// <summary>
    /// Creates a new <see cref="T:TestCaptureLogger"/> instance.
    /// </summary>
    /// <param name="categoryName">The category name for messages produced by the logger.</param>
    /// <returns>The instance of the <see cref="TestCaptureLogger"/> that was created.</returns>
    public TestCaptureLogger CreateLogger(string categoryName)
        => _captures.GetOrAdd(categoryName, static cn => new TestCaptureLogger(cn));

    /// <summary>
    /// Creates a new <see cref="T:TestCaptureLogger"/> instance.
    /// </summary>
    /// <typeparam name="T">The type that the logger is assigned to.</typeparam>
    /// <returns>The instance of the <see cref="TestCaptureLogger{TCategoryType}"/> that was created.</returns>
    public TestCaptureLogger<T> CreateLogger<T>()
        => (TestCaptureLogger<T>)_typedCaptures.GetOrAdd(
            typeof(T),
            static (type, that) =>
            {
                var categoryName = type.AsCategoryName();
                var underlyingLogger = that.CreateLogger(categoryName);
                return new TestCaptureLogger<T>(underlyingLogger);
            },
            this);

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
        var lists = new List<IReadOnlyList<LogEntry>>();
        int totalCount = 0;
        foreach (var logger in loggers)
        {
            var logs = logger.GetLogs();
            if (logs.Count == 0)
                continue;

            lists.Add(logs);
            totalCount += logs.Count;
        }

        return MergeSortedLogs(lists, totalCount);
    }

    /// <summary>
    /// Gets all log entries matching the predicate regardless of the Category
    /// they were logged as.
    /// </summary>
    /// <returns>A read only list of <see cref="LogEntry"/> objects.</returns>
    public IReadOnlyList<LogEntry> GetLogs(Func<LogEntry, bool> predicate)
    {
        var loggers = _captures.Values;
        var lists = new List<IReadOnlyList<LogEntry>>();
        int totalCount = 0;
        foreach (var logger in loggers)
        {
            var logs = logger.GetLogs(predicate);
            if (logs.Count == 0)
                continue;

            lists.Add(logs);
            totalCount += logs.Count;
        }

        return MergeSortedLogs(lists, totalCount);
    }

    private static IReadOnlyList<LogEntry> MergeSortedLogs(
        List<IReadOnlyList<LogEntry>> lists,
        int totalCount)
    {
        if (totalCount == 0)
            return [];

        if (lists.Count == 1)
            return lists[0];

        var result = new List<LogEntry>(totalCount);
        var queue = new PriorityQueue<LogCursor, int>(lists.Count);

        for (int i = 0; i < lists.Count; i++)
        {
            var entry = lists[i][0];
            queue.Enqueue(new LogCursor(i, 0, entry), entry.Sequence);
        }

        while (queue.Count > 0)
        {
            var cursor = queue.Dequeue();
            result.Add(cursor.Entry);

            int nextIndex = cursor.EntryIndex + 1;
            if (nextIndex < lists[cursor.ListIndex].Count)
            {
                var nextEntry = lists[cursor.ListIndex][nextIndex];
                queue.Enqueue(new LogCursor(cursor.ListIndex, nextIndex, nextEntry), nextEntry.Sequence);
            }
        }

        return result;
    }

    private readonly record struct LogCursor(int ListIndex, int EntryIndex, LogEntry Entry);


    /// <summary>
    /// Gets all log entries with exceptions attached regardless of the
    /// Category they were logged as.
    /// </summary>
    /// <returns>A read only list of <see cref="LogEntry"/> objects.</returns>
    public IReadOnlyList<LogEntry> GetAllLogEntriesWithExceptions()
        => GetLogs(static log => log.Exception != null);

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
        _captures.Clear();
        _typedCaptures.Clear();
        GC.SuppressFinalize(this);
    }
}
