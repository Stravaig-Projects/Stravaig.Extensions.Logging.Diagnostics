using System.Diagnostics;
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
            var messageTemplate = "This is a {whatAmI} with {whatItHas}.";
            var whatAmIValue = "message";
            var whatItHasValue = "structured parameters";
            logger.LogInformation(
                messageTemplate,
                whatAmIValue,
                whatItHasValue);

            Debugger.Break();
            logger.Logs.Count.ShouldBe(1);
            logger.Logs[0].Properties.ShouldNotBeNull();
            logger.Logs[0].Properties.Count.ShouldBe(3);
            logger.Logs[0].OriginalMessage.ShouldBe(messageTemplate);
            logger.Logs[0].Properties[0].Key.ShouldBe("whatAmI");
            logger.Logs[0].Properties[1].Key.ShouldBe("whatItHas");
        }
    }
}