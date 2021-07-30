using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics;

namespace ExampleTestProject
{
    public class ExampleOneTests
    {
        public class MyService
        {
            private readonly ILogger<MyService> _logger;

            public MyService(ILogger<MyService> logger)
            {
                _logger = logger;
            }

            public void DoStuff()
            {
                // Do whatever the service needs to do.
                _logger.LogInformation("The work is done.");
            }
        }

        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void TestServiceLoggerCalled()
        {
            // Arrange
            var logger = new TestCaptureLogger<MyService>();
            var service = new MyService(logger);

            // Act
            service.DoStuff();

            // Assert
            var logs = logger.GetLogs();
            logs.Count.ShouldBe(1);
            logs[0].FormattedMessage.ShouldContain("The work is done.");
        }
    }
}