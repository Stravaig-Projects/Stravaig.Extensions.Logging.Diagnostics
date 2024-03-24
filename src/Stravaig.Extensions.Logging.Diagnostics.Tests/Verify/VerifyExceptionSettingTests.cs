using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyExceptionSettingTests
{
    [Test]
    [TestCase(ExceptionSetting.None)]
    [TestCase(ExceptionSetting.Message)]
    [TestCase(ExceptionSetting.Type)]
    [TestCase(ExceptionSetting.StackTrace)]
    [TestCase(ExceptionSetting.IncludeInnerExceptions)]
    [TestCase(ExceptionSetting.IncludeInnerExceptions | ExceptionSetting.Message)]
    [TestCase(ExceptionSetting.IncludeInnerExceptions | ExceptionSetting.Type)]
    [TestCase(ExceptionSetting.IncludeInnerExceptions | ExceptionSetting.Type | ExceptionSetting.Message)]
    [TestCase(ExceptionSetting.Message | ExceptionSetting.Type | ExceptionSetting.StackTrace | ExceptionSetting.IncludeInnerExceptions)]
    public async Task VerifyMessageSettingAsync(ExceptionSetting setting)
    {
        // CAUTION: Moving the code around may break some test scenarios as the
        // StackTrack will be included in the verify file, which contains line
        // numbers.
        var logger = new TestCaptureLogger<VerifyExceptionSettingTests>();

        try
        {
            DoTheThing();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error happened that resulted in an exception being thrown.");
        }
        
        var logs = logger.GetLogs();

        VerifySettings verifySettings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings()
            {
                Exception = setting,
            });
        verifySettings.UseParameters(setting);

        await Verifier.Verify(logs, verifySettings);
    }

    private void DoTheThing()
    {
        try
        {
            PerformThePartOfTheTaskThatActuallyBreaks();
        }
        catch (Exception ex)
        {
            throw new ApplicationException("This is the application exception, it contains an inner exception.", ex);
        }
    }

    private void PerformThePartOfTheTaskThatActuallyBreaks()
    {
        // This will throw an exception.
        throw new InvalidOperationException("Boo! You performed an invalid operation.");
    }
}