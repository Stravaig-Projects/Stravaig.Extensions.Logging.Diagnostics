using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyLogConverterSettingsTests
{
    [Test]
    public async Task TestPropertiesAsync()
    {
        var logger = new TestCaptureLogger<VerifyLogConverterSettingsTests>();
        logger.LogInformation("A {TypeOfThing} with {PropertyCount} properties.", "message", 2);
        var settings = new VerifySettings()
            .AddStravaigTests(Settings.FormattedMessage | Settings.Properties);
        await Verifier.Verify(logger.GetLogs(), settings);
    }
}