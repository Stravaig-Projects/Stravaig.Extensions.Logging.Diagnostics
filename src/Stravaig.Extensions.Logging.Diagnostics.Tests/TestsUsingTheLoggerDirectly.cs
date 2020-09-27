using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    public class TestsUsingTheLoggerDirectly
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InformationMessageIsCapturedAndReadable()
        {
            // Arrange
            var logger = new TestCaptureLogger();
            string message = "This is a test log message.";
            
            // Act
            logger.LogInformation(message);

            // Assert
            var logEntries = logger.Logs;
            logEntries.Count.ShouldBe(1);
            var entry = logEntries.First();
            entry.Exception.ShouldBeNull();
            entry.EventId.ShouldBe(0);
            entry.LogLevel.ShouldBe(LogLevel.Information);
            entry.FormattedMessage.ShouldContain(message);
        }
        
        [Test]
        public void MultipleLogsAreCapturedAndRetrievedInTheCorrectOrder()
        {
            // Arrange
            var logger = new TestCaptureLogger();
            
            // Act
            logger.LogTrace("This is a trace message.");
            logger.LogDebug("This is a debug message.");
            logger.LogInformation("This is an information message.");
            logger.LogWarning("This is a warning message.");
            logger.LogError("This is an error message.");
            logger.LogCritical("This is a critical message.");
    
            // Assert
            var logEntries = logger.Logs;
            logEntries.Count.ShouldBe(6);
            logEntries[0].LogLevel.ShouldBe(LogLevel.Trace);
            logEntries[1].LogLevel.ShouldBe(LogLevel.Debug);
            logEntries[2].LogLevel.ShouldBe(LogLevel.Information);
            logEntries[3].LogLevel.ShouldBe(LogLevel.Warning);
            logEntries[4].LogLevel.ShouldBe(LogLevel.Error);
            logEntries[5].LogLevel.ShouldBe(LogLevel.Critical);
        }

        [Test]
        public void NoLoggerCapturedReturnsEmptyList()
        {
            // Arrange
            var logger = new TestCaptureLogger();
            
            // Act - none, no logs are being created.
            
            // Assert
            var logEntries = logger.Logs;
            logEntries.Count.ShouldBe(0);
        }
    }
}