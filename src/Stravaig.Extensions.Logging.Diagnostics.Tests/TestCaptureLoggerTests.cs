using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

[TestFixture]
public class TestCaptureLoggerTests
{
    [Test]
    public void LogsWithExceptionOnlyReturnsLogsWithAnExceptionObjectAttached()
    {
        var logger = new TestCaptureLogger();

        logger.LogInformation("One");
        logger.LogWarning(new Exception("I'm an exception"), "Two");
        logger.LogError(new ApplicationException("I'm an application exception"), "Three");
        logger.LogError("Four");
        logger.LogCritical(new InvalidOperationException("I'm an invalid operation exception"), "Five");

        var logsWithExceptions = logger.GetLogEntriesWithExceptions();
        logsWithExceptions.Count.ShouldBe(3);
        logsWithExceptions[0].OriginalMessage.ShouldBe("Two");
        logsWithExceptions[1].OriginalMessage.ShouldBe("Three");
        logsWithExceptions[2].OriginalMessage.ShouldBe("Five");
    }
}
