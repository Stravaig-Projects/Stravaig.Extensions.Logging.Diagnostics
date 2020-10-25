using System;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// A representation of an item that was logged.
    /// </summary>
    public class LogEntry
    {
        /// <summary>
        /// The <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> that the item was logged at.
        /// </summary>
        public LogLevel LogLevel { get; }
        
        /// <summary>
        /// An <see cref="T:Microsoft.Extensions.Logging.EventId"/> that identifies a logging event.
        /// </summary>
        public EventId EventId { get; }
        
        /// <summary>
        /// The entry to be written. Can be also an object.
        /// </summary>
        public object State { get; }
        
        /// <summary>
        /// The <see cref="T:System.Exception"/> that was attached to the log.
        /// </summary>
        public Exception Exception { get; }
        
        /// <summary>
        /// The formatted message.
        /// </summary>
        public string FormattedMessage { get; }

        /// <summary>
        /// Initialises a <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/>.
        /// </summary>
        /// <param name="logLevel">The <see cref="T:Microsoft.Extensions.Logging.LogLeve"/> that was logged.</param>
        /// <param name="eventId">An <see cref="T:Microsoft.Extensions.Logging.EventId"/> that identifies the logging event.</param>
        /// <param name="state">The entry that was written. Can be also an object.</param>
        /// <param name="exception">The <see cref="T:System.Exception"/> that was attached to the log.</param>
        /// <param name="formattedMessage">The formatted message.</param>
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