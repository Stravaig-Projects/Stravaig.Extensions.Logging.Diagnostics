using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests
{
    [TestFixture]
    public class StructuredLoggingTests
    {
        [Test]
        public void EnsureOriginalMessageAndPropertiesAreRetrievable()
        {
            var logger = new TestCaptureLogger();
            const string messageTemplate = "This is a {whatAmI} with {whatItHas}.";
            var whatAmIValue = "message";
            var whatItHasValue = "structured parameters";
            logger.LogInformation(
                messageTemplate,
                whatAmIValue,
                whatItHasValue);

            var logs = logger.GetLogs();
            logs.Count.ShouldBe(1);
            logs[0].Properties.ShouldNotBeNull();
            logs[0].Properties.Count.ShouldBe(3);
            logs[0].OriginalMessage.ShouldBe(messageTemplate);
            logs[0].Properties[0].Key.ShouldBe("whatAmI");
            logs[0].Properties[1].Key.ShouldBe("whatItHas");
        }
    }
}