using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics;

namespace ExampleTestProject
{
    public class ExampleTwoTests
    {
        public class MyService
        {
            private readonly ILogger<MyService> _logger;

            public MyService(ILoggerFactory loggerFactory)
            {
                _logger = loggerFactory.CreateLogger<MyService>();
            }

            public void DoStuff()
            {
                // Do whatever the service needs to do.
                _logger.LogInformation("The work is done.");
            }
        }

        [Test]
        public void TestServiceLoggerCalled()
        {
            // Arrange
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });
            var service = new MyService(factory);

            // Act
            service.DoStuff();

            // Assert
            var logs = logProvider.GetLogEntriesFor<MyService>();
            logs.Count.ShouldBe(1);
            logs[0].FormattedMessage.ShouldContain("The work is done.");
        }

    }
}