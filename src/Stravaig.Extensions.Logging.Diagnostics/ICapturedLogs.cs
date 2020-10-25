using System.Collections.Generic;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// Represents a collection of captured logs.
    /// </summary>
    public interface ICapturedLogs
    {
        /// <summary>
        /// Gets a list of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/> objects for the given category.
        /// </summary>
        /// <param name="categoryName">The name of the category to fetch</param>
        /// <returns>A list of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/> objects.</returns>
        IReadOnlyList<LogEntry> GetLogEntriesFor(string categoryName);

        /// <summary>
        /// Gets a list of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/> objects for the given category.
        /// </summary>
        /// <typeparam name="T">The category type to fetch</typeparam>
        /// <returns>A list of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/> objects.</returns>
        IReadOnlyList<LogEntry> GetLogEntriesFor<T>();
    }
}