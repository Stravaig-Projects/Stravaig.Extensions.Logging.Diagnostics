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
    public void RenderedLogMessagesGetSentToOutputTestHelper()
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

    private void ResetLogSequence()
    {
        var logEntryType = typeof(LogEntry);
        var field = logEntryType.GetField("_sequence", BindingFlags.Static | BindingFlags.NonPublic);
        field.SetValue(null, 0);
    }
}