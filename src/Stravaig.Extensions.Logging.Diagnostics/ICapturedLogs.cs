using System.Collections.Generic;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    public interface ICapturedLogs
    {
        IReadOnlyList<LogEntry> GetLogEntriesFor(string categoryName);
        IReadOnlyList<LogEntry> GetLogEntriesFor<T>();
    }
}