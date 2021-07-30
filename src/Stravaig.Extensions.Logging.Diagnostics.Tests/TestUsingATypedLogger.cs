using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    public class TestUsingATypedLogger
    {
        public class MyServiceClass
        {
            private readonly ILogger<MyServiceClass> _logger;

            public MyServiceClass(ILogger<MyServiceClass> logger)
            {
                _logger = logger;
            }

            public void DoWork()
            {
                _logger.LogInformation("Called DoWork()");
            }
        }

        [Test]
        public void DirectlyInjectedLoggerLogsMessage()
        {
            var logger = new TestCaptureLogger<MyServiceClass>();
            var service = new MyServiceClass(logger);
            
            service.DoWork();

            var logs = logger.GetLogs();
            logs.Count.ShouldBe(1);
            logs[0].LogLevel.ShouldBe(LogLevel.Information);
            logs[0].FormattedMessage.ShouldContain("Called DoWork()");
        }

        [Test]
        public void InjectedViaLoggingFactoryLogsMessage()
        {
            var logProvider = new TestCaptureLoggerProvider();
            var factory = LoggerFactory.Create(builder =>
            {
                builder.AddProvider(logProvider);
            });
            var logger = factory.CreateLogger<MyServiceClass>();
            var service = new MyServiceClass(logger);
            
            service.DoWork();

            var logs = logProvider.GetLogEntriesFor<MyServiceClass>();
            logs.Count.ShouldBe(1);
            logs[0].LogLevel.ShouldBe(LogLevel.Information);
            logs[0].FormattedMessage.ShouldContain("Called DoWork()");
        }
    }
}