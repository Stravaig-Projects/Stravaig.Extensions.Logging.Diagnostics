using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

    [Test]
    public async Task MultipleLogsCreatedAndGetLogsReturnsAllLogsInOrderAsync()
    {
        int threadCount = Environment.ProcessorCount * 4;
        int logsPerThread = 1000;
        var logger = new TestCaptureLogger();
        Task[] tasks = new Task[threadCount];
        for (int i = 0; i < threadCount; i++)
        {
            int threadId = i;
            tasks[i] = Task.Run(() =>
            {
                for (int j = 0; j < logsPerThread; j++)
                {
                    logger.LogInformation("Thread {ThreadId} Log {LogIndex}", threadId, j);
                }
            });
        }

        await Task.WhenAll(tasks);

        var logs = logger.GetLogs();
        logs.Count.ShouldBe(threadCount * logsPerThread);

        // Verify sequence numbers are always incrementing
        for (int i = 1; i < logs.Count; i++)
        {
            logs[i].Sequence.ShouldBeGreaterThan(logs[i - 1].Sequence);
        }

        // Verify timestamps are always incrementing
        for (int i = 1; i < logs.Count; i++)
        {
            logs[i].TimestampUtc.ShouldBeGreaterThan(logs[i - 1].TimestampUtc);
        }

        var lastLogIndexByThread = new Dictionary<int, int>(threadCount);
        for (int i = 0; i < logs.Count; i++)
        {
            var properties = logs[i].PropertyDictionary;
            properties.ContainsKey("ThreadId").ShouldBeTrue();
            properties.ContainsKey("LogIndex").ShouldBeTrue();
            int threadId = Convert.ToInt32(properties["ThreadId"]);
            int logIndex = Convert.ToInt32(properties["LogIndex"]);

            if (lastLogIndexByThread.TryGetValue(threadId, out int lastIndex))
            {
                logIndex.ShouldBe(lastIndex + 1);
            }
            else
            {
                logIndex.ShouldBe(0);
            }

            lastLogIndexByThread[threadId] = logIndex;
        }

        lastLogIndexByThread.Count.ShouldBe(threadCount);
        foreach (var lastLogIndex in lastLogIndexByThread.Values)
        {
            lastLogIndex.ShouldBe(logsPerThread - 1);
        }
    }
}
