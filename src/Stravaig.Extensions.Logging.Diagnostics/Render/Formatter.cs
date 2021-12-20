using System;

namespace Stravaig.Extensions.Logging.Diagnostics.Render
{
    /// <summary>
    /// Functions that format the log entries in specific ways
    /// </summary>
    public static class Formatter
    {
        /// <summary>
        /// Formats the log entry as '[sequence level] message exception'
        /// </summary>
        public static Func<LogEntry, string> SimpleBySequence =>
            le => $"[{le.Sequence} {le.LogLevel}] {le.FormattedMessage}{FormatAppendedException(le)}";
        
        /// <summary>
        /// Formats the log entry as '[local-timestamp level] message exception'
        /// </summary>
        public static Func<LogEntry, string> SimpleByLocalTime =>
            le => $"[{le.TimestampLocal:yyyy-MM-dd'T'HH:mm:sszzz} {le.LogLevel}] {le.FormattedMessage}{FormatAppendedException(le)}";

        /// <summary>
        /// Formats the log entry as '[utc-timestamp level] message exception'
        /// </summary>
        public static Func<LogEntry, string> SimpleByUtcTime =>
            le => $"[{le.TimestampUtc:yyyy-MM-dd'T'HH:mm:sszzz} {le.LogLevel}] {le.FormattedMessage}{FormatAppendedException(le)}";
        
        private static string FormatAppendedException(LogEntry le)
        {
            return le.Exception == null
                ? string.Empty
                : $"{Environment.NewLine}{le.Exception}";
        }
    }
}