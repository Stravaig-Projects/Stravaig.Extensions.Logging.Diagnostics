using System;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    public class LogEntry
    {
        public LogLevel LogLevel { get; }
        public EventId EventId { get; }
        public object State { get; }
        public Exception Exception { get; }
        public string FormattedMessage { get; }

        public LogEntry(LogLevel logLevel, EventId eventId, object state, Exception exception, string formattedMessage)
        {
            LogLevel = logLevel;
            EventId = eventId;
            State = state;
            Exception = exception;
            FormattedMessage = formattedMessage;
        }
    }
}