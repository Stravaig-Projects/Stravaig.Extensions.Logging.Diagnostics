using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using static Stravaig.Extensions.Logging.Diagnostics.ExternalHelpers.TypeNameHelper;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// A provider of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger"/> instances.
    /// </summary>
    public class TestCaptureLoggerProvider : ILoggerProvider, ICapturedLogs
    {
        private readonly ConcurrentDictionary<string, TestCaptureLogger> _captures;
        
        /// <summary>
        /// Creates an instance of <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLoggerProvider"/>
        /// </summary>
        public TestCaptureLoggerProvider()
        {
            _captures = new ConcurrentDictionary<string, TestCaptureLogger>();
        }

        /// <summary>
        /// Gets a list of log entries captured in the specified category.
        /// </summary>
        /// <param name="categoryName">The category name under which logs were captured.</param>
        /// <returns>The list of log entries captured, empty if none.</returns>
        public IReadOnlyList<LogEntry> GetLogEntriesFor(string categoryName)
        {
            return _captures.TryGetValue(categoryName, out TestCaptureLogger logger)
                ? logger.Logs
                : Array.Empty<LogEntry>();
        }

        /// <summary>
        /// Gets a list of log entries captured in the specified category.
        /// </summary>
        /// <param name="type">The type which forms the category name for the logger.</param>
        /// <returns>The list of log entries captured, empty if none.</returns>
        public IReadOnlyList<LogEntry> GetLogEntriesFor(Type type)
        {
            var categoryName = GetTypeDisplayName(type, includeGenericParameters: false, nestedTypeDelimiter: '.');
            return GetLogEntriesFor(categoryName);
        }
        
        /// <summary>
        /// Gets a list of log entries captured in the specified category.
        /// </summary>
        /// <typeparam name="T">The type which forms the category name for the logger.</typeparam>
        /// <returns>The list of log entries captured, empty if none.</returns>
        public IReadOnlyList<LogEntry> GetLogEntriesFor<T>()
        {
            return GetLogEntriesFor(typeof(T));
        }

        /// <summary>
        /// Creates a new <see cref="T:Microsoft.Extensions.Logging.ILogger"/> instance.
        /// </summary>
        /// <param name="categoryName">The category name for messages produced by the logger.</param>
        /// <returns>The instance of ILogger that was created.</returns>
        public ILogger CreateLogger(string categoryName)
        {
            return _captures.GetOrAdd(categoryName, _ => new TestCaptureLogger());
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
        }
    }
}