using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    public class TestsUsingALoggerFactory
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void InformationMessageIsCapturedAndReadable()
        {
            // Arrange
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });
            var logger = factory.CreateLogger<TestsUsingALoggerFactory>();
            string message = "This is a test log message.";
            
            // Act
            logger.LogInformation(message);

            // Assert
            var logEntries = logProvider.GetLogEntriesFor<TestsUsingALoggerFactory>();
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
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
                builder.SetMinimumLevel(LogLevel.Trace);
            });
            var logger = factory.CreateLogger<TestsUsingALoggerFactory>();
            
            // Act
            logger.LogTrace("This is a trace message.");
            logger.LogDebug("This is a debug message.");
            logger.LogInformation("This is an information message.");
            logger.LogWarning("This is a warning message.");
            logger.LogError("This is an error message.");
            logger.LogCritical("This is a critical message.");
    
            // Assert
            var logEntries = logProvider.GetLogEntriesFor<TestsUsingALoggerFactory>();
            logEntries.Count.ShouldBe(6);
            logEntries[0].LogLevel.ShouldBe(LogLevel.Trace);
            logEntries[1].LogLevel.ShouldBe(LogLevel.Debug);
            logEntries[2].LogLevel.ShouldBe(LogLevel.Information);
            logEntries[3].LogLevel.ShouldBe(LogLevel.Warning);
            logEntries[4].LogLevel.ShouldBe(LogLevel.Error);
            logEntries[5].LogLevel.ShouldBe(LogLevel.Critical);
        }

        [Test]
        public void MultipleLoggersForSameClassCaptureToSameList()
        {
            // Arrange
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });

            // Act
            var logger1 = factory.CreateLogger<TestsUsingALoggerFactory>();
            logger1.LogInformation("This is a log message on the first logger.");

            var logger2 = factory.CreateLogger<TestsUsingALoggerFactory>();
            logger2.LogInformation("This is a log message on the second logger.");
            
            // Assert
            var logEntries = logProvider.GetLogEntriesFor<TestsUsingALoggerFactory>();
            logEntries.Count.ShouldBe(2);
            logEntries[0].FormattedMessage.ShouldContain("first logger");
            logEntries[1].FormattedMessage.ShouldContain("second logger");
        }

        [Test]
        public void NoLoggerCapturedReturnsEmptyList()
        {
            // Arrange
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });
            
            // Act - none, no logs are being created.
            
            // Assert
            var logEntries = logProvider.GetLogEntriesFor<TestsUsingALoggerFactory>();
            logEntries.Count.ShouldBe(0);

        }
        
        [Test]
        public void ManyThreadsCreatingLoggersFromProvider()
        {
            const int timeoutMs = 30000;
            const int iterationsPerThread = 1000;
            int numThreads = Environment.ProcessorCount;
            int expectedLogCount = iterationsPerThread * numThreads;
            Console.WriteLine($"Performing {iterationsPerThread} iterations on each of {numThreads} threads for an expected total of {expectedLogCount} log messages.");
            Task[] tasks = new Task[numThreads];
            var provider = new TestCaptureLoggerProvider();

            for (int taskNumber = 0; taskNumber < numThreads; taskNumber++)
            {
                int capturedTaskNumber = taskNumber;
                tasks[taskNumber] = Task.Factory.StartNew(() =>
                {
                    string categoryName = "Category_" + capturedTaskNumber;
                    for (int i = 0; i < iterationsPerThread; i++)
                    {
                        var logger = provider.CreateLogger(categoryName);
                        logger.LogInformation(
                            "Log iteration {iteration} on thread {threadId}",
                            i,
                            Thread.CurrentThread.ManagedThreadId);
                    }
                });
            }

            using CancellationTokenSource source = new CancellationTokenSource(timeoutMs);
            Task.WaitAll(tasks, source.Token);
            
            tasks.ShouldAllBe(t => t.IsCompleted);

            for (int i = 0; i < numThreads; i++)
            {
                var logs = provider.GetLogEntriesFor("Category_" + i);
                logs.ShouldNotBeNull();
                logs.Count.ShouldBe(iterationsPerThread);
            }
        }
    }
}