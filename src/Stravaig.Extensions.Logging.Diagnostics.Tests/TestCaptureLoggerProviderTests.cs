using System;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    [TestFixture]
    public class TestCaptureLoggerProviderTests
    {
        [Test]
        public void GetAllLogEntriesReturnsLogsInCorrectSequence()
        {
            var provider = new TestCaptureLoggerProvider();

            var logger1 = provider.CreateLogger("logger1");
            var logger2 = provider.CreateLogger("logger2");
            
            logger1.LogInformation("One");
            logger2.LogInformation("Two");
            logger1.LogInformation("Three");
            logger1.LogInformation("Four");
            logger2.LogInformation("Five");

            var allLogs = provider.GetAllLogEntries();
            allLogs[0].OriginalMessage.ShouldBe("One");
            allLogs[1].OriginalMessage.ShouldBe("Two");
            allLogs[2].OriginalMessage.ShouldBe("Three");
            allLogs[3].OriginalMessage.ShouldBe("Four");
            allLogs[4].OriginalMessage.ShouldBe("Five");
        }
        
        [Test]
        public void GetAllLogEntriesWithExceptionsReturnsLogsInCorrectSequence()
        {
            var provider = new TestCaptureLoggerProvider();

            var logger1 = provider.CreateLogger("logger1");
            var logger2 = provider.CreateLogger("logger2");
            
            logger1.LogInformation(new Exception(), "One");
            logger2.LogInformation("Two");
            logger1.LogInformation("Three");
            logger1.LogInformation(new Exception(), "Four");
            logger2.LogInformation(new Exception(), "Five");

            var allLogsWithExceptions = provider.GetAllLogEntriesWithExceptions();
            allLogsWithExceptions[0].OriginalMessage.ShouldBe("One");
            allLogsWithExceptions[1].OriginalMessage.ShouldBe("Four");
            allLogsWithExceptions[2].OriginalMessage.ShouldBe("Five");
        }
    }
}