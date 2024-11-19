using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Stravaig.Extensions.Logging.Diagnostics.ExternalHelpers;

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
/// A logger that writes messages to a store that can later be examined
/// programatically, such as in unit tests.
/// </summary>
/// <typeparam name="TCategoryType"></typeparam>
public class TestCaptureLogger<TCategoryType> : ITestCaptureLogger, ILogger<TCategoryType>
{
    private readonly TestCaptureLogger _logger;

    /// <summary>
    /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger&lt;TCategoryType>"/> class.
    /// </summary>
    public TestCaptureLogger()
    {
        _logger = new TestCaptureLogger(TypeNameHelper.GetTypeDisplayName(typeof(TCategoryType)));
    }

    /// <summary>
    /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger&lt;TCategoryType>"/>
    /// class, using an existing logger as the underlying logger.
    /// </summary>
    public TestCaptureLogger(TestCaptureLogger logger)
    {
        ArgumentNullException.ThrowIfNull(logger, nameof(logger));
        var expectedCategoryName = TypeNameHelper.GetTypeDisplayName(typeof(TCategoryType));
        if (logger.CategoryName != expectedCategoryName)
            throw new InvalidOperationException(
                $"The category name does not match the type of this logger. Expected \"{expectedCategoryName}\", got \"{logger.CategoryName}\".");
        _logger = logger;
    }

    /// <summary>
    /// Converts a <see cref="TestCaptureLogger{TCategoryType}"/> instance to a <see cref="TestCaptureLogger"/> instance.
    /// </summary>
    /// <param name="logger">The <see cref="TestCaptureLogger{TCategoryType}"/> instance to convert.</param>
    /// <returns>A <see cref="TestCaptureLogger"/> instance.</returns>
    public static implicit operator TestCaptureLogger(TestCaptureLogger<TCategoryType> logger)
    {
        return logger._logger;
    }

    /// <summary>
    /// Defines an explicit conversion from a non-generic <see cref="TestCaptureLogger"/>
    /// to a generic <see cref="TestCaptureLogger{TCategoryType}"/>.
    /// </summary>
    /// <param name="logger">The instance of the non-generic <see cref="TestCaptureLogger"/> to convert.</param>
    /// <returns>
    /// A new instance of <see cref="TestCaptureLogger{TCategoryType}"/> if the category name
    /// of the provided logger matches the type of the category.
    /// </returns>
    /// <exception cref="InvalidCastException">
    /// Thrown if the category name of the provided logger does not match the expected
    /// category name for the specified <typeparamref name="TCategoryType"/>.
    /// </exception>
    public static explicit operator TestCaptureLogger<TCategoryType>(TestCaptureLogger logger)
    {
        // Check the category name to see if it matches or can be used for this type
        var expectedCategoryName = TypeNameHelper.GetTypeDisplayName(typeof(TCategoryType));
        if (logger.CategoryName != expectedCategoryName)
            throw new InvalidCastException(
                $"The category name does not match the type of this logger. Cannot cast a TestCaptureLogger with category name {logger.CategoryName} to TestCaptureLogger<{expectedCategoryName}>.");

        // Return a new generic logger using the existing logger
        return new TestCaptureLogger<TCategoryType>(logger);
    }

    /// <inheritdoc />
    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter)
        => _logger.Log(logLevel, eventId, state, exception, formatter);

    /// <inheritdoc />
    public bool IsEnabled(LogLevel logLevel)
        => _logger.IsEnabled(logLevel);

    /// <inheritdoc />
#if NET7_0_OR_GREATER
    public IDisposable? BeginScope<TState>(TState state) where TState : notnull
        => _logger.BeginScope(state);
#else
    public IDisposable BeginScope<TState>(TState state)
        => _logger.BeginScope(state);
#endif

    /// <summary>
    /// Resets the logger by discarding the captured logs.
    /// </summary>
    public void Reset()
        => _logger.Reset();

    /// <summary>
    /// Gets a read-only list of logs that is a snapshot of this logger.
    /// </summary>
    /// <remarks>Any additional logs added to the logger after this is
    /// called won't be available in the list, and it will have to be called again.</remarks>
    public IReadOnlyList<LogEntry> GetLogs()
        => _logger.GetLogs();

    /// <inheritdoc />
    public IReadOnlyList<LogEntry> GetLogs(Func<LogEntry, bool> predicate)
        => _logger.GetLogs(predicate);

    /// <summary>
    /// Gets a read-only list of logs that have an exception attached in sequential order.
    /// </summary>
    public IReadOnlyList<LogEntry> GetLogEntriesWithExceptions()
        => _logger.GetLogEntriesWithExceptions();

    /// <summary>
    /// The name of the category the log entry belongs to.
    /// </summary>
    public string CategoryName
        => _logger.CategoryName;
}
