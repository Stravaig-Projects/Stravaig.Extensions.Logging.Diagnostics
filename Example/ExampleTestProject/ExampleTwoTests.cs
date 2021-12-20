using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics;
using Stravaig.Extensions.Logging.Diagnostics.Render;

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
                _logger.LogInformation("About to do some work.");
                // Do whatever the service needs to do.
                _logger.LogInformation("The work is done.");
            }
        }

        [Test]
        public void TestServiceLoggerCalled()
        {
            // ****************************************
            // Arrange
            
            // Set up the logger factory with a test capture logger
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });
            
            // Inject the factory into the service.
            var service = new MyService(factory);

            // ****************************************
            // Act
            service.DoStuff();

            // ****************************************
            // Assert
            
            // Retrieve the logs for the MyService source
            var logs = logProvider.GetLogEntriesFor<MyService>();
            
            // Render the logs to the console
            logs.RenderLogs(Formatter.SimpleByLocalTime, Sink.Console);
            
            // Assert that the logs are as they should be
            logs.Count.ShouldBe(2);
            logs[0].FormattedMessage.ShouldContain("About to do some work.");
            logs[1].FormattedMessage.ShouldContain("The work is done.");
        }

    }
}