using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Stravaig.Extensions.Logging.Diagnostics.Verify;
using VerifyNUnit;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

[TestFixture]
public class VerifyLogPropertyTests
{
    [Test]
    public async Task TestPropertiesAsync()
    {
        var logger = new TestCaptureLogger<VerifyLogPropertyTests>();
        logger.LogInformation("A {TypeOfThing} with {PropertyCount} properties.", "message", 2);
        var settings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings
            {
                Sequence = Sequence.Hide,
                LogLevel = false,
                CategoryName = false,
                Message = MessageSetting.Formatted,
                Properties = PropertySetting.Verify,
            });
        await Verifier.Verify(logger.GetLogs(), settings);
    }
    
    [Test]
    [TestCase(PropertySetting.None)]
    [TestCase(PropertySetting.RedactNonDeterministic)]
    [TestCase(PropertySetting.CaseInsensitivePropertyMatch)]
    [TestCase(PropertySetting.CaseInsensitivePropertyMatch | PropertySetting.RedactNonDeterministic)]
    public async Task TestRedactedPropertiesAsync(PropertySetting setting)
    {
        var logger = new TestCaptureLogger<VerifyLogPropertyTests>();
        logger.LogInformation(
            "This is shown: {StandardProperty}, this is redacted: {NondeterministicProperty}; and this might be redacted if case insensitive {CaseDependentNondeterministicProperty}",
            "show me",
            "hide me",
            "maybe hide me");
        var settings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings
            {
                Sequence = Sequence.Hide,
                LogLevel = false,
                CategoryName = false,
                Message = MessageSetting.Formatted,
                Properties = PropertySetting.Verify | setting,
                NondeterministicPropertyNames = ["NondeterministicProperty", "caseDependentNondeterministicProperty"],
            });
        settings.UseParameters(setting);
        await Verifier.Verify(logger.GetLogs(), settings);
    }
    
    [Test]
    [TestCase(true)]
    [TestCase(false)]
    public async Task GuidProperties(bool scrubGuids)
    {
        var id1 = new Guid("40e4cfcb-2e13-4ce5-83dd-e2f1056fd173");
        var id2 = new Guid("a3022d80-1726-488c-9dd6-7e588fe1311b");
        var logger = new TestCaptureLogger<VerifyLogPropertyTests>();
        logger.LogInformation("Thing {id} is good.", id1);
        logger.LogInformation("Thing {id} is good too.", id2);
        var settings = new VerifySettings()
            .AddCapturedLogs(new LoggingCaptureVerifySettings
            {
                Sequence = Sequence.Hide,
                LogLevel = false,
                CategoryName = false,
                Message = MessageSetting.Formatted,
                Properties = PropertySetting.Verify,
                NondeterministicPropertyNames = ["ndId"],
            });
        settings.UseParameters(scrubGuids);
        if (!scrubGuids)
            settings.DontScrubGuids();
        await Verifier.Verify(logger.GetLogs(), settings);
    }
}