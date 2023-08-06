using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyDefaultSettingsTests
{
    [Test]
    public async Task TestExplicitDefaultAsync()
    {
        var settings = new VerifySettings().AddStravaigTests();
        var logs = GetLogEntries();
        await Verifier.Verify(logs, settings);
    }

    public async Task TestImplicitDefaultAwait()
    {
        // The settings are added as part of the module initialiser
        var logs = GetLogEntries();
        await Verifier.Verify(logs);
    }

    private IEnumerable<LogEntry> GetLogEntries()
    {
        var logger = new TestCaptureLogger<VerifyDefaultSettingsTests>();
        logger.LogInformation("This is the first default log message");
        logger.LogWarning("This is a warning");
        try
        {
            throw new ApplicationException("I'm a fake exception to be put in the log.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception was thrown. See the exception for details.");
        }

        try
        {
            try
            {
                throw new InvalidOperationException("An invalid operation happened.");
            }
            catch (Exception ex)
            {
                throw new ApplicationException("An application exception happened", ex);
            }
        }
        catch (Exception ex)
        {
            logger.LogCritical(ex, "An exception was thrown, which caused another exception to be thrown.");
        }

        return logger.GetLogs();
    }
}