using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.Render;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Renderer
{
    [TestFixture]
    public class FormatterTests
    {
        [Test]
        public void SequenceWithNoException()
        {
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty, 1, DateTime.UtcNow);

            var renderedLog = Formatter.SimpleBySequence(logEntry);
            
            renderedLog.ShouldBe("[1 Information] This is the test log message.");
        }

        [Test]
        public void SequenceWithCategoryNameAndNoException()
        {
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests), 1, DateTime.UtcNow);

            var renderedLog = Formatter.SimpleBySequence(logEntry);
            
            renderedLog.ShouldBe("[1 Information FormatterTests] This is the test log message.");
        }

        [Test]
        public void UtcTimeWithNoException()
        {
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty, 1, DateTime.UnixEpoch);

            var renderedLog = Formatter.SimpleByUtcTime(logEntry);
            
            renderedLog.ShouldBe("[1970-01-01T00:00:00+00:00 Information] This is the test log message.");
        }

        [Test]
        public void UtcTimeWithCategoryNameAndNoException()
        {
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests), 1, DateTime.UnixEpoch);

            var renderedLog = Formatter.SimpleByUtcTime(logEntry);
            
            renderedLog.ShouldBe("[1970-01-01T00:00:00+00:00 Information FormatterTests] This is the test log message.");
        }
        
        [Test]
        public void LocalTimeWithNoException()
        {
            var timestamp = new DateTimeOffset(2021, 06, 12, 13, 14, 15, TimeSpan.FromHours(01))
                .UtcDateTime;
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty, 1, timestamp);

            var renderedLog = Formatter.SimpleByLocalTime(logEntry);
            
            renderedLog.ShouldBe("[2021-06-12T13:14:15+01:00 Information] This is the test log message.");
        }
        
        [Test]
        public void LocalTimeWithCategoryNameAndNoException()
        {
            var timestamp = new DateTimeOffset(2021, 06, 12, 13, 14, 15, TimeSpan.FromHours(01))
                .UtcDateTime;
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests), 1, timestamp);

            var renderedLog = Formatter.SimpleByLocalTime(logEntry);
            
            renderedLog.ShouldBe("[2021-06-12T13:14:15+01:00 Information FormatterTests] This is the test log message.");
        }
    }
}