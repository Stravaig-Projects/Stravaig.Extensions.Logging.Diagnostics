using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifySequenceTests
{
    [Test]
    [TestCase(LogLevel.Trace)]
    [TestCase(LogLevel.Information)]
    [TestCase(LogLevel.Error)]
    public async Task ConsecutiveSequenceTest(LogLevel minLevel)
    {
        var logs = GetLogs().Where(l => l.LogLevel >= minLevel);
        VerifySettings verifySettings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings()
            {
                Message = MessageSetting.Formatted,
                Sequence = Sequence.ShowAsConsecutive,
                CategoryName = false,
            });
        verifySettings.UseParameters(minLevel);

        await Verifier.Verify(logs, verifySettings);
    }
    
    [Test]
    [TestCase(LogLevel.Trace)]
    [TestCase(LogLevel.Information)]
    [TestCase(LogLevel.Error)]
    public async Task CadenceSequenceTest(LogLevel minLevel)
    {
        var logs = GetLogs().Where(l => l.LogLevel >= minLevel);
        VerifySettings verifySettings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings()
            {
                Message = MessageSetting.Formatted,
                Sequence = Sequence.ShowAsCadence,
                CategoryName = false,
            });
        verifySettings.UseParameters(minLevel);

        await Verifier.Verify(logs, verifySettings);
    }

    private IReadOnlyList<LogEntry> GetLogs()
    {
        var logger = new TestCaptureLogger<VerifySequenceTests>();
        logger.LogInformation("This is the first log.");
        logger.LogTrace("This is the second log.");
        logger.LogWarning("This is the third log.");
        logger.LogDebug("This is the fourth log.");
        logger.LogError("This is the fifth log.");
        logger.LogCritical("This is the sixth log.");
        
        logger.LogInformation("This is the seventh log.");
        logger.LogTrace("This is the eighth log.");
        logger.LogWarning("This is the ninth log.");
        logger.LogDebug("This is the tenth log.");
        logger.LogError("This is the eleventh log.");
        logger.LogCritical("This is the twelfth log.");

        return logger.GetLogs();
    }
}