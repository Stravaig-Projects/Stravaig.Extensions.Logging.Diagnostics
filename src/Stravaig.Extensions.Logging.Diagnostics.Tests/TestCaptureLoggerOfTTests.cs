using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

[TestFixture]
public class TestCaptureLoggerOfTTests
{
    [Test]
    public void ConstructorWithNullLoggerThrows()
    {
        Should.Throw<ArgumentNullException>(() => new TestCaptureLogger<object>(null!))
            .ParamName.ShouldBe("logger");
    }

    [Test]
    public void ConstructorCategoryMismatchThrows()
    {
        var underlyingLogger = new TestCaptureLogger("NotTheRightCategory");
        Should.Throw<InvalidOperationException>(() => new TestCaptureLogger<object>(underlyingLogger))
            .Message.ShouldBe("The category name does not match the type of this logger. Expected \"object\", got \"NotTheRightCategory\".");
    }

    [Test]
    public void CategoryNameIsBasedOnType()
    {
        var logger = new TestCaptureLogger<TestCaptureLoggerOfTTests>();
        logger.CategoryName.ShouldBe(GetType().FullName);
    }

    [Test]
    [TestCase(LogLevel.Trace)]
    [TestCase(LogLevel.Debug)]
    [TestCase(LogLevel.Information)]
    [TestCase(LogLevel.Warning)]
    [TestCase(LogLevel.Error)]
    [TestCase(LogLevel.Critical)]
    public void IsEnabledTests(LogLevel level)
    {
        var logger = new TestCaptureLogger<TestCaptureLoggerOfTTests>();
        logger.IsEnabled(level).ShouldBeTrue();
    }

    [Test]
    public void ResetRemovesCapturedLogs()
    {
        var logger = new TestCaptureLogger<TestCaptureLoggerOfTTests>();
        logger.LogInformation("Hello");
        logger.LogWarning("World");
        logger.GetLogs().Count.ShouldBe(2);

        logger.Reset();
        logger.GetLogs().Count.ShouldBe(0);
    }

    [Test]
    public void GetLogsWithExceptionsWillFilterOutNonExceptionLogs()
    {
        var logger = new TestCaptureLogger<TestCaptureLoggerOfTTests>();
        logger.LogInformation("Hello");
        logger.LogWarning(new Exception(), "World");
        logger.LogInformation("This is a log.");
        logger.LogError(new Exception(), "This has an exception.");

        logger.GetLogEntriesWithExceptions().Count.ShouldBe(2);
        logger.GetLogs().Count.ShouldBe(4);
    }

    [Test]
    public void GetLogsMatchingPredicateWillFilterOutUnwantedLogs()
    {
        var logger = new TestCaptureLogger<TestCaptureLoggerOfTTests>();
        logger.LogInformation("Hello");
        logger.LogWarning("Hello, {Location}!", "World");
        logger.LogInformation("This is a log.");
        logger.LogError(new Exception(), "This has an exception.");

        logger.GetLogs(static l => l.LogLevel == LogLevel.Information).Count.ShouldBe(2);

        logger.GetLogs(static l => l.PropertyDictionary.ContainsKey("Location") && (string)l.PropertyDictionary["Location"] == "World")
            .Count.ShouldBe(1);
        logger.GetLogs().Count.ShouldBe(4);
    }
}
