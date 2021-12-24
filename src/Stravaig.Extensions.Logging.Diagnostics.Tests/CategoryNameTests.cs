using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    [TestFixture]
    public class CategoryNameTests
    {
        [Test]
        public void Ctor()
        {
            var logEntry = new LogEntry(LogLevel.Information, new EventId(1), new object(), null, "Some message",
                "Some category");
            logEntry.CategoryName.ShouldBe("Some category");
        }

        [Test]
        public void FromBasicLogger()
        {
            var logger = new TestCaptureLogger();
            logger.LogInformation("Some log");
            logger.GetLogs()[0].CategoryName.ShouldBe(string.Empty);
        }

        [Test]
        public void FromBasicLoggerWithCategoryName()
        {
            var logger = new TestCaptureLogger("SomeCategory");
            logger.LogInformation("Some log");
            logger.GetLogs()[0].CategoryName.ShouldBe("SomeCategory");
        }

        [Test]
        public void FromGenericLogger()
        {
            var logger = new TestCaptureLogger<CategoryNameTests>();
            logger.LogInformation("Some log");
            logger.GetLogs()[0].CategoryName.ShouldBe(typeof(CategoryNameTests).FullName);
        }

        [Test]
        public void FromLogProvider()
        {
            var logProvider = new TestCaptureLoggerProvider();
            var logger = logProvider.CreateLogger("SomeCategory");
            logger.LogInformation("Some log");
            logProvider.GetAllLogEntries()[0].CategoryName.ShouldBe("SomeCategory");
        }
    }
}