﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// A representation of an item that was logged.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplayString) + "}")]
    public class LogEntry : IComparable<LogEntry>
    {
        private static int _sequence = 0;
        private static readonly object SequenceSyncLock = new object();

        private const string OriginalMessagePropertyName = "{OriginalFormat}";
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
        /// The sequence number of the log message.
        /// </summary>
        /// <remarks>In a multi-threaded environment there may be gaps between adjacent log messages.</remarks>
        public int Sequence { get; }
        
        /// <summary>
        /// The time the log entry was created in UTC. 
        /// </summary>
        public DateTime TimestampUtc { get; }

        /// <summary>
        /// Ths time the log entry was created in the system's local time
        /// </summary>
        public DateTimeOffset TimestampLocal
        {
            get
            {
                TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(TimestampUtc);
                long localTicks = TimestampUtc.Ticks + offset.Ticks;
                return new DateTimeOffset(localTicks, offset);
            }
        }

        /// <summary>
        /// The properties, if any, for the log entry.
        /// </summary>
        public IReadOnlyList<KeyValuePair<string, object>> Properties =>
            State as IReadOnlyList<KeyValuePair<string, object>> ?? Array.Empty<KeyValuePair<string, object>>();

        /// <summary>
        /// The original message template, if available, for the log entry.
        /// </summary>
        public string OriginalMessage =>
            (string) Properties
                .FirstOrDefault(p => p.Key == OriginalMessagePropertyName)
                .Value;

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
            lock (SequenceSyncLock)
            {
                Sequence = _sequence++;
                TimestampUtc = DateTime.UtcNow;
            }
        }

        /// <inheritdoc />
        public int CompareTo(LogEntry other)
        {
            if (ReferenceEquals(this, other)) return 0;
            if (ReferenceEquals(null, other)) return 1;
            return Sequence.CompareTo(other.Sequence);
        }

        private string DebuggerDisplayString => $"[#{Sequence} @ {TimestampLocal:HH:mm:ss.fff zzz} {LogLevel}] {FormattedMessage}";
    }
}