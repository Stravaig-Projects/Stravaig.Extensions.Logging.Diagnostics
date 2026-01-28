using System;
using System.Collections.Generic;
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

        try
        {
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

    [Test]
    public void PropertiesFromNonEnumerableStateUsesTypeNameAndToString()
    {
        var log = new LogEntry(
            LogLevel.Information,
            new EventId(),
            new SimpleState(),
            null,
            "Message",
            "CategoryName");

        log.Properties.Count.ShouldBe(1);
        log.Properties[0].Key.ShouldBe(typeof(SimpleState).FullName);
        log.Properties[0].Value.ShouldBe("simple-state");
        log.PropertyDictionary[typeof(SimpleState).FullName!].ShouldBe("simple-state");
    }

    [Test]
    public void PropertiesFromNonEnumerableStateToStringThrowsUsesExceptionString()
    {
        var log = new LogEntry(
            LogLevel.Information,
            new EventId(),
            new ThrowingState(),
            null,
            "Message",
            "CategoryName");

        var value = (string)log.Properties[0].Value!;
        value.ShouldContain(nameof(InvalidOperationException));
    }

    [Test]
    public void PropertyDictionaryLastWinsOnDuplicateKeys()
    {
        var state = new List<KeyValuePair<string, object?>>
        {
            new("Key", "First"),
            new("Key", "Second"),
        };

        var log = new LogEntry(
            LogLevel.Information,
            new EventId(),
            state,
            null,
            "Message",
            "CategoryName");

        log.Properties.Count.ShouldBe(2);
        log.PropertyDictionary["Key"].ShouldBe("Second");
    }

    private sealed class SimpleState
    {
        public override string ToString() => "simple-state";
    }

    private sealed class ThrowingState
    {
        public override string ToString() => throw new InvalidOperationException("boom");
    }
}
