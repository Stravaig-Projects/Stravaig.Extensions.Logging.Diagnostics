using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// A logger that writes messages to a store that can later be examined
    /// programatically, such as in unit tests.
    /// </summary>
    public class TestCaptureLogger : ILogger
    {
        private readonly List<LogEntry> _logs;

        /// <summary>
        /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> class.
        /// </summary>
        public TestCaptureLogger()
        {
            _logs = new List<LogEntry>();
        }

        /// <summary>
        /// Gets a read-only list of logs for this logger.
        /// </summary>
        public IReadOnlyList<LogEntry> Logs => _logs;

        /// <summary>
        /// Writes a log entry
        /// </summary>
        /// <param name="logLevel"></param>
        /// <param name="eventId"></param>
        /// <param name="state"></param>
        /// <param name="exception"></param>
        /// <param name="formatter"></param>
        /// <typeparam name="TState"></typeparam>
        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var formattedMessage = formatter(state, exception);
            _logs.Add(new LogEntry(logLevel, eventId, state, exception, formattedMessage));
        }

        /// <summary>
        /// Checks whether the given <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> is enabled.
        /// </summary>
        /// <param name="logLevel"></param>
        /// <returns>true if enabled; false otherwise.</returns>
        public bool IsEnabled(LogLevel logLevel) => true;

        /// <summary>
        /// Begins a logical operation scope.
        /// </summary>
        /// <param name="state"></param>
        /// <typeparam name="TState"></typeparam>
        /// <returns>A disposable object that ends the logical operation scope on dispose.</returns>
        public IDisposable BeginScope<TState>(TState state)
            => DoNothing.Instance;

        private class DoNothing : IDisposable
        {
            public static readonly DoNothing Instance = new DoNothing();
            public void Dispose()
            { }
        }
    }
}