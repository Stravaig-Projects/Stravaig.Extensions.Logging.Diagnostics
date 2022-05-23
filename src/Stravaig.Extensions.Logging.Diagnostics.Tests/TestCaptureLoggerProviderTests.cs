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
        
        [Test]
        public void GetCategoriesReturnsListOfCategoryNames()
        {
            var provider = new TestCaptureLoggerProvider();
            var factory = new LoggerFactory();
            factory.AddProvider(provider);

            provider.CreateLogger("logger1");
            provider.CreateLogger("logger2");
            factory.CreateLogger<TestCaptureLoggerTests>();
            
            var categories = provider.GetCategories();
            categories.Count.ShouldBe(3);
            categories.ShouldContain("logger1");
            categories.ShouldContain("logger2");
            categories.ShouldContain(typeof(TestCaptureLoggerTests).FullName);
        }
        
        [Test]
        public void ResetAfterLoggingReturnsZeroLogs()
        {
            var provider = new TestCaptureLoggerProvider();

            var logger1 = provider.CreateLogger("logger1");
            var logger2 = provider.CreateLogger("logger2");
            
            logger1.LogInformation("One");
            logger2.LogInformation("Two");
            logger1.LogInformation("Three");
            logger1.LogInformation("Four");
            logger2.LogInformation("Five");

            provider.Reset();
            
            provider.GetAllLogEntries().ShouldBeEmpty();
        }
        
        [Test]
        public void ReuseAfterResetReturnsOnlyNewLogs()
        {
            var provider = new TestCaptureLoggerProvider();

            var logger1 = provider.CreateLogger("logger1");
            var logger2 = provider.CreateLogger("logger2");
            
            logger1.LogInformation("Old-One");
            logger2.LogInformation("Old-Two");

            provider.Reset();

            var loggerNew1 = provider.CreateLogger("logger1");
            var loggerNew2 = provider.CreateLogger("logger2");
            var logger3 = provider.CreateLogger("logger3");
            
            loggerNew1.LogInformation("One");
            loggerNew2.LogInformation("Two");
            logger3.LogInformation("Three");
            
            var allLogs = provider.GetAllLogEntries();
            allLogs[0].OriginalMessage.ShouldBe("One");
            allLogs[0].CategoryName.ShouldBe("logger1");
            allLogs[1].OriginalMessage.ShouldBe("Two");
            allLogs[1].CategoryName.ShouldBe("logger2");
            allLogs[2].OriginalMessage.ShouldBe("Three");
            allLogs[2].CategoryName.ShouldBe("logger3");

            logger1.ShouldBeSameAs(loggerNew1);
            logger2.ShouldBeSameAs(loggerNew2);
        }
    }
}