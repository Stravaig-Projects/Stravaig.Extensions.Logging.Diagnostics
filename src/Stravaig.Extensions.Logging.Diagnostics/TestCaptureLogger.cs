using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    public class TestCaptureLogger : ILogger
    {
        private readonly List<LogEntry> _logs;

        public TestCaptureLogger()
        {
            _logs = new List<LogEntry>();
        }

        public IReadOnlyList<LogEntry> Logs => _logs;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            var formattedMessage = formatter(state, exception);
            _logs.Add(new LogEntry(logLevel, eventId, state, exception, formattedMessage));
        }

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state)
            => DoNothing.Instance;

        private class DoNothing : IDisposable
        {
            public static readonly DoNothing Instance = new DoNothing();
            public void Dispose()
            { }
        }
    }

    public class TestCaptureLogger<TCategoryName> : TestCaptureLogger, ILogger<TCategoryName>
    {
    }
}