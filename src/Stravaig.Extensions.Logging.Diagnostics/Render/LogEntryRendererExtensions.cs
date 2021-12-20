using System;
using System.Collections.Generic;

namespace Stravaig.Extensions.Logging.Diagnostics.Render
{
    /// <summary>
    /// Extension methods for rendering logs
    /// </summary>
    public static class LogEntryRendererExtensions
    {
        /// <summary>
        /// Renders a log to a sink with a given format.
        /// </summary>
        /// <param name="logs">The logs to render.</param>
        /// <param name="format">The function that formats the log.</param>
        /// <param name="writeToSink">The action that writes the formatted log to the sink.</param>
        public static void RenderLogs(this IEnumerable<LogEntry> logs, Func<LogEntry, string> format, Action<string> writeToSink)
        {
            foreach (var log in logs)
            {
                var formatted = format(log);
                writeToSink(formatted);
            }
        }
    }
}