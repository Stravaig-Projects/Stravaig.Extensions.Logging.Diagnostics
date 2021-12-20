using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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
            const string message = "This is a test log message.";
            
            // Act
            logger.LogInformation(message);

            // Assert
            var logEntries = logger.GetLogs();
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
            var logEntries = logger.GetLogs();
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
            var logEntries = logger.GetLogs();
            logEntries.Count.ShouldBe(0);
        }

        [Test]
        public void SequenceNumberIncreasedOnEachLogMessage()
        {
            // Arrange
            var logger = new TestCaptureLogger();

            logger.LogInformation("Message 1");
            logger.LogInformation("Message 2");

            var logEntries = logger.GetLogs();
            logEntries[0].Sequence.ShouldBeLessThan(logEntries[1].Sequence);
        }

        [Test]
        public void SequenceNumberIncreasedOnEachLogMessageAcrossMultipleLoggers()
        {
            // Arrange
            var logger1 = new TestCaptureLogger();
            var logger2 = new TestCaptureLogger();

            logger1.LogInformation("Message 1");
            logger2.LogInformation("Message 2");

            logger1.GetLogs()[0].Sequence.ShouldBeLessThan(logger2.GetLogs()[0].Sequence);
        }

        [Test]
        public void ManyThreadsAccessingLogger()
        {
            const int timeoutMs = 30000;
            const int iterationsPerThread = 25000;
            int numThreads = Environment.ProcessorCount;
            int expectedLogCount = iterationsPerThread * numThreads;
            Console.WriteLine($"Performing {iterationsPerThread} iterations on each of {numThreads} threads for an expected total of {expectedLogCount} log messages.");
            Task[] tasks = new Task[numThreads];
            var logger = new TestCaptureLogger();

            for (int taskNumber = 0; taskNumber < numThreads; taskNumber++)
            {
                tasks[taskNumber] = Task.Factory.StartNew(() =>
                {
                    for (int i = 0; i < iterationsPerThread; i++)
                    {
                        logger.LogInformation(
                            "Log iteration {Iteration} on thread {ThreadId}",
                            i,
                            Thread.CurrentThread.ManagedThreadId);
                    }
                });
            }

            using CancellationTokenSource source = new CancellationTokenSource(timeoutMs);
            Task.WaitAll(tasks, source.Token);
            
            tasks.ShouldAllBe(t => t.IsCompleted);
            var logs = logger.GetLogs();
            logs.Count.ShouldBe(expectedLogCount);
            
            // Check no duplicate sequence numbers
            logs.Select(l => l.Sequence).Distinct().Count().ShouldBe(expectedLogCount);
            
            // Checks that log entries are in sequence order
            logs.ShouldBeInOrder(SortDirection.Ascending);
            
            // Check that timestamps progressed forward-only
            Enumerable.Range(0, logs.Count - 2)
                .Select( i => new
                {
                    Index = i,
                    Current = logs[i],
                    Next = logs[i+1]
                }).ShouldAllBe(e => e.Current.TimestampUtc <= e.Next.TimestampUtc);

            DateTime first = logs.First().TimestampUtc;
            DateTime last = logs.Last().TimestampUtc;
            Console.WriteLine($"Logging started at {first:HH:mm:ss.fff} and ended at {last:HH:mm:ss.fff} taking a total of {(last-first):G}");
        }
    }
}