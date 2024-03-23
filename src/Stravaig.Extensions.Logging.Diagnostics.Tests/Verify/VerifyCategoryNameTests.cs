using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyCategoryNameTests
{
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task VerifyCategoryNameAsync(bool setting)
    {
        var logs = GetLogs();
        VerifySettings verifySettings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings()
            {
                CategoryName = setting,
            });
        verifySettings.UseParameters(setting);

        await Verifier.Verify(logs, verifySettings);
    }
    
    private IReadOnlyList<LogEntry> GetLogs()
    {
        var provider = new TestCaptureLoggerProvider();
        LoggerFactory factory = new LoggerFactory([provider]);
        ILogger logger = factory.CreateLogger<VerifyCategoryNameTests>();
        logger.LogInformation("This is the first log. It is from the test class's category.");

        logger = factory.CreateLogger("custom-category-name");
        logger.LogInformation("This is the second log. It has a custom category name.");
        
        return provider.GetAllLogEntries();
    }
}