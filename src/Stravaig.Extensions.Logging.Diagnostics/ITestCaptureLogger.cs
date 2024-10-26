using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
///
/// </summary>
public interface ITestCaptureLogger : ILogger
{
    /// <summary>
    /// The name of the category the log entry belongs to.
    /// </summary>
    string CategoryName { get; }

    /// <summary>
    /// Gets a read-only list of logs that is a snapshot of this logger.
    /// </summary>
    /// <remarks>Any additional logs added to the logger after this is
    /// called won't be available in the list, and it will have to be called again.</remarks>
    IReadOnlyList<LogEntry> GetLogs();

    /// <summary>
    /// Gets a read-only list of logs that have an exception attached in sequential order.
    /// </summary>
    IReadOnlyList<LogEntry> GetLogEntriesWithExceptions();

    /// <summary>
    /// Resets the logger by discarding the captured logs.
    /// </summary>
    void Reset();
}
