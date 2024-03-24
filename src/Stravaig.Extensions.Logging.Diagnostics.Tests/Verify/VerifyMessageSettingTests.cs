using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyMessageSettingTests
{
    [Test]
    [TestCase(MessageSetting.None)]
    [TestCase(MessageSetting.Formatted)]
    [TestCase(MessageSetting.Template)]
    public async Task VerifyMessageSettingAsync(MessageSetting setting)
    {
        var logger = new TestCaptureLogger<VerifyMessageSettingTests>();
        logger.LogInformation("This is a test of the {SettingName} message setting.", setting);
        logger.LogInformation("This is a multiline log message.\nThis is the second line of the message.");
        var logs = logger.GetLogs();

        VerifySettings verifySettings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings()
            {
                Message = setting,
            });
        verifySettings.UseParameters(setting);

        await Verifier.Verify(logs, verifySettings);
        
    }
}