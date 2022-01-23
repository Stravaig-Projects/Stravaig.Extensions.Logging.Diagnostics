using System.Linq;
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
            var logEntry = logs[0];
            logEntry.Properties.ShouldNotBeNull();
            logEntry.Properties.Count.ShouldBe(3);
            logEntry.OriginalMessage.ShouldBe(messageTemplate);
            logEntry.Properties[0].Key.ShouldBe("whatAmI");
            logEntry.Properties[1].Key.ShouldBe("whatItHas");
            logEntry.CategoryName.ShouldBe(string.Empty);
            logEntry.PropertyDictionary["whatAmI"].ShouldBe(whatAmIValue);
            logEntry.PropertyDictionary["whatItHas"].ShouldBe(whatItHasValue);
        }
    }
}