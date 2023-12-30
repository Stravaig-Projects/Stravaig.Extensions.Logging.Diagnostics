using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyLogExceptionTests
{
    [Test]
    public async Task VerifyInnerExceptionsAsync()
    {
        var logger = new TestCaptureLogger<VerifyLogExceptionTests>();
        try
        {
            ThrowAnExceptionWithInnerExceptions(5);
        }
        catch (Exception ex)
        {
            logger.LogInformation(ex, "An Exception was thrown.");
        }

        var logs = logger.GetLogs();

        VerifySettings verifySettings = new VerifySettings()
            .AddStravaigTests(Settings.Default | Settings.StackTrace);
        await Verifier.Verify(logs, verifySettings);
    }

    private static void ThrowAnExceptionWithInnerExceptions(int depth = 1)
    {
        try
        {
            if (depth > 0)
                ThrowAnExceptionWithInnerExceptions(depth - 1);
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"Current depth is {depth}.", ex);
        }

        throw new InvalidOperationException($"This method throws exceptions!");
    }
}