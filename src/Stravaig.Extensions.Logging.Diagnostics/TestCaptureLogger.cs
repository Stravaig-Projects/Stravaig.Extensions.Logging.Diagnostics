using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly object _syncRoot;

        /// <summary>
        /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> class.
        /// </summary>
        public TestCaptureLogger()
        {
            _logs = new List<LogEntry>();
            _syncRoot = new object();
            CategoryName = string.Empty;
        }

        /// <summary>
        /// Initialises a new instance of the TestCaptureLogger class.
        /// </summary>
        /// <param name="categoryName">The name of the category</param>
        public TestCaptureLogger(string categoryName)
            : this()
        {
            CategoryName = categoryName;
        }

        /// <summary>
        /// The name of the category the log entry belongs to.
        /// </summary>
        public string CategoryName { get; }
        
        /// <summary>
        /// Gets a read-only list of logs that is a snapshot of this logger.
        /// </summary>
        /// <remarks>Any additional logs added to the logger after this is
        /// called won't be available in the list and it will have to be called again.</remarks>
        public IReadOnlyList<LogEntry> GetLogs()
        {
            lock (_syncRoot)
            {
                _logs.Sort();
                return _logs.ToArray();
            }
        }
        
        /// <summary>
        /// Gets a read-only list of logs that have an exception attached in sequential order.
        /// </summary>
        public IReadOnlyList<LogEntry> GetLogEntriesWithExceptions()
        {
            List<LogEntry> result;
            lock (_syncRoot)
            {
                result = _logs.Where(l => l.Exception != null).ToList();
            }
            result.Sort();
            return result;
        }

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
            var logEntry = CreateLogEntry(logLevel, eventId, state, exception, formattedMessage);
            lock (_syncRoot)
            {
                _logs.Add(logEntry);
            }
        }

        /// <summary>
        /// Creates the LogEntry object.
        /// </summary>
        /// <param name="logLevel">The level the entry is logged at</param>
        /// <param name="eventId">The event id</param>
        /// <param name="state">The state (properties) of the log entry</param>
        /// <param name="exception">Any exception</param>
        /// <param name="formattedMessage">The formatted message</param>
        /// <typeparam name="TState">The object type that holds the state</typeparam>
        /// <returns>A log entry</returns>
        protected LogEntry CreateLogEntry<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, string formattedMessage)
        {
            return new LogEntry(logLevel, eventId, state, exception, formattedMessage, CategoryName);
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

        /// <summary>
        /// Resets the logger by discarding the captured logs.
        /// </summary>
        public void Reset()
        {
            lock (_syncRoot)
            {
                _logs.Clear();
            }
        }
    }
}