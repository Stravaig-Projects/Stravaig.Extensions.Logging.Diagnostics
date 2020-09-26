using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    public class TestCaptureLoggerProvider : ILoggerProvider, ICapturedLogs
    {
        private readonly Dictionary<string, TestCaptureLogger> _captures;
        public TestCaptureLoggerProvider()
        {
            _captures = new Dictionary<string, TestCaptureLogger>();
        }

        public IReadOnlyList<LogEntry> GetLogEntriesFor(string categoryName)
        {
            if (_captures.TryGetValue(categoryName, out TestCaptureLogger logger))
            {
                return logger.Logs;
            }

            return Array.Empty<LogEntry>();
        }

        public IReadOnlyList<LogEntry> GetLogEntriesFor<T>()
        {
            return GetLogEntriesFor(typeof(T).FullName);
        }

        public ILogger CreateLogger(string categoryName)
        {
            if (_captures.TryGetValue(categoryName, out TestCaptureLogger logger))
            {
                return logger;
            }

            logger = new TestCaptureLogger();
            _captures.Add(categoryName, logger);
            return logger;
        }

        public void Dispose()
        { }
    }
}