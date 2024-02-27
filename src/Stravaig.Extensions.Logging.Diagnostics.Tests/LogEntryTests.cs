using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

public class LogEntryTests
{
    [Test]
    public void MinimalToString()
    {
        var log = new LogEntry(
            LogLevel.Information,
            new EventId(),
            null,
            null,
            "This is the log message.",
            "CategoryName");
        
        log.ToString().ShouldBe("LogEntry: [Information CategoryName] This is the log message.");
    }

    [Test]
    public void WithEventIdToString()
    {
        var log = new LogEntry(
            LogLevel.Information,
            new EventId(1),
            null,
            null,
            "This is the log message.",
            "CategoryName");
        
        log.ToString().ShouldBe("LogEntry: [Information CategoryName #1] This is the log message.");
    }

    [Test]
    public void WithEventIdAndNameToString()
    {
        var log = new LogEntry(
            LogLevel.Information,
            new EventId(1,"event-name"),
            null,
            null,
            "This is the log message.",
            "CategoryName");
        
        log.ToString().ShouldBe("LogEntry: [Information CategoryName #1/event-name] This is the log message.");
    }
    
    [Test]
    public void WithExeptionToString()
    {
        var log = new LogEntry(
            LogLevel.Error,
            new EventId(),
            null,
            new InvalidOperationException(),
            "This is the log message.",
            "CategoryName");
        
        log.ToString().ShouldBe("LogEntry: [Error CategoryName $InvalidOperationException] This is the log message.");
    }
    
    [Test]
    public void EverythingToString()
    {
        var log = new LogEntry(
            LogLevel.Error,
            new EventId(2, "full-event"),
            null,
            new InvalidOperationException(),
            "This is the log message.",
            "CategoryName");
        
        log.ToString().ShouldBe("LogEntry: [Error CategoryName #2/full-event $InvalidOperationException] This is the log message.");
    }
    
    [Test]
    public void RegressionShouldlyShouldDisplayCoherentMessage()
    {
        try
        {
            var logs = new[]
            {
                new LogEntry(
                    LogLevel.Information,
                    new EventId(),
                    null,
                    null,
                    "This is a test log entry",
                    nameof(LogEntryTests)),
                new LogEntry(
                    LogLevel.Warning,
                    new EventId(1, "test-event"),
                    null,
                    new InvalidOperationException(),
                    "This is a second test message.",
                    nameof(LogEntryTests)),
            };
            logs.ShouldBeEmpty();
        }
        catch(Exception ex)
        {
            string expectedMessage = 
@"logs
    should be empty but had
2
    items and was
[LogEntry: [Information LogEntryTests] This is a test log entry, LogEntry: [Warning LogEntryTests #1/test-event $InvalidOperationException] This is a second test message.]"
    .Replace("\r", string.Empty);
            string actualMessage = ex.Message.Replace("\r", string.Empty);
            Console.WriteLine(actualMessage);
            actualMessage.ShouldBe(expectedMessage);
        }
    }
}