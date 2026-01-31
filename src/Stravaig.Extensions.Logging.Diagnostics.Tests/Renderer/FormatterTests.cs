using System;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Time.Testing;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.Render;
using Stravaig.Extensions.Logging.Diagnostics.Tests.Helpers;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Renderer
{
    [TestFixture]
    [NonParallelizable]
    public class FormatterTests
    {
        [TearDown]
        public void TearDown()
        {
            LogEntryHelper.ResetTimeProvider();
        }

        [Test]
        public void SequenceWithNoException()
        {
            LogEntryHelper.ResetLogSequence(1);
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty);

            var renderedLog = Formatter.SimpleBySequence(logEntry);
            
            renderedLog.ShouldBe("[1 Information] This is the test log message.");
        }

        [Test]
        public void SequenceWithCategoryNameAndNoException()
        {
            LogEntryHelper.ResetLogSequence(1);
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests));

            var renderedLog = Formatter.SimpleBySequence(logEntry);
            
            renderedLog.ShouldBe("[1 Information FormatterTests] This is the test log message.");
        }

        [Test]
        public void UtcTimeWithNoException()
        {
            LogEntryHelper.SetTimeProvider(new FakeTimeProvider(DateTimeOffset.UnixEpoch));
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty);

            var renderedLog = Formatter.SimpleByUtcTime(logEntry);
            
            renderedLog.ShouldBe("[1970-01-01T00:00:00+00:00 Information] This is the test log message.");
        }

        [Test]
        public void UtcTimeWithCategoryNameAndNoException()
        {
            LogEntryHelper.SetTimeProvider(new FakeTimeProvider(DateTimeOffset.UnixEpoch));
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests));

            var renderedLog = Formatter.SimpleByUtcTime(logEntry);
            
            renderedLog.ShouldBe("[1970-01-01T00:00:00+00:00 Information FormatterTests] This is the test log message.");
        }
        
        [Test]
        public void LocalTimeWithNoException()
        {
            var timestamp = new DateTimeOffset(2021, 06, 12, 13, 14, 15, TimeSpan.FromHours(01));
            var tz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var fakeTimeProvider = new FakeTimeProvider(timestamp.ToUniversalTime());
            fakeTimeProvider.SetLocalTimeZone(tz);
            LogEntryHelper.SetTimeProvider(fakeTimeProvider);
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", string.Empty);

            var renderedLog = Formatter.SimpleByLocalTime(logEntry);
            
            renderedLog.ShouldBe("[2021-06-12T13:14:15+01:00 Information] This is the test log message.");
        }
        
        [Test]
        public void LocalTimeWithCategoryNameAndNoException()
        {
            var timestamp = new DateTimeOffset(2021, 06, 12, 13, 14, 15, TimeSpan.FromHours(01));
            var tz = TimeZoneInfo.FindSystemTimeZoneById("GMT Standard Time");
            var fakeTimeProvider = new FakeTimeProvider(timestamp.ToUniversalTime());
            fakeTimeProvider.SetLocalTimeZone(tz);

            LogEntryHelper.SetTimeProvider(fakeTimeProvider);
            var logEntry = new LogEntry(LogLevel.Information, new EventId(), null, null,
                "This is the test log message.", nameof(FormatterTests));

            var renderedLog = Formatter.SimpleByLocalTime(logEntry);
            
            renderedLog.ShouldBe("[2021-06-12T13:14:15+01:00 Information FormatterTests] This is the test log message.");
        }
    }
}
