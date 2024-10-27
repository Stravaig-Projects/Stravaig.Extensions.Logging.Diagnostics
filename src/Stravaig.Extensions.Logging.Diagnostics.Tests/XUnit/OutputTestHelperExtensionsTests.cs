using System;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.XUnit;
using Xunit.Abstractions;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.XUnit;

public class OutputTestHelperExtensionTests
{
    private class OutputHelper : ITestOutputHelper
    {
        public readonly List<string> Messages = new();
        public void WriteLine(string message)
        {
            Messages.Add(message);
            Console.WriteLine(message);
        }

        public void WriteLine(string format, params object[] args)
        {
        }
    }

    [Test]
    public void RenderedLogEntryMessagesSentToOutputTestHelper()
    {
        ResetLogSequence();
        var outputHelper = new OutputHelper();
        var logger = new TestCaptureLogger<OutputTestHelperExtensionTests>();
        logger.LogInformation("This is an information Message.");
        logger.LogInformation("This is a Warning Message.");

        var logs = logger.GetLogs();
        outputHelper.WriteLogs(logs);

        outputHelper.Messages[0].ShouldBe("[0 Information Stravaig.Extensions.Logging.Diagnostics.Tests.XUnit.OutputTestHelperExtensionTests] This is an information Message.");
        outputHelper.Messages[1].ShouldBe("[1 Information Stravaig.Extensions.Logging.Diagnostics.Tests.XUnit.OutputTestHelperExtensionTests] This is a Warning Message.");
    }

    [Test]
    public void RenderedLoggerSentToOutputTestHelper()
    {
        ResetLogSequence();
        var outputHelper = new OutputHelper();
        var logger = new TestCaptureLogger<OutputTestHelperExtensionTests>();
        logger.LogInformation("This is an information Message.");
        logger.LogInformation("This is a Warning Message.");

        outputHelper.WriteLogs(logger);

        outputHelper.Messages[0].ShouldBe("[0 Information Stravaig.Extensions.Logging.Diagnostics.Tests.XUnit.OutputTestHelperExtensionTests] This is an information Message.");
        outputHelper.Messages[1].ShouldBe("[1 Information Stravaig.Extensions.Logging.Diagnostics.Tests.XUnit.OutputTestHelperExtensionTests] This is a Warning Message.");
    }

    [Test]
    public void RenderedLoggerProviderSentToOutputTestHelper()
    {
        ResetLogSequence();
        var outputHelper = new OutputHelper();
        var loggerProvider = new TestCaptureLoggerProvider();
        var logger1 = loggerProvider.CreateLogger("logger1");
        var logger2 = loggerProvider.CreateLogger("logger2");
        logger1.LogInformation("This is an information Message on the first logger.");
        logger2.LogInformation("This is an information Message on the second logger.");
        logger2.LogWarning("This is a Warning Message on the second logger.");
        logger1.LogWarning("This is a Warning Message on the first logger.");

        outputHelper.WriteLogs(loggerProvider);

        outputHelper.Messages[0].ShouldBe("[0 Information logger1] This is an information Message on the first logger.");
        outputHelper.Messages[1].ShouldBe("[1 Information logger2] This is an information Message on the second logger.");
        outputHelper.Messages[2].ShouldBe("[2 Warning logger2] This is a Warning Message on the second logger.");
        outputHelper.Messages[3].ShouldBe("[3 Warning logger1] This is a Warning Message on the first logger.");
    }

    private void ResetLogSequence()
    {
        var logEntryType = typeof(LogEntry);
        var field = logEntryType.GetField("_sequence", BindingFlags.Static | BindingFlags.NonPublic);
        if (field == null)
            throw new InvalidOperationException("Expected LogEntry to have a private field called _sequence.");
        field.SetValue(null, 0);
    }
}
