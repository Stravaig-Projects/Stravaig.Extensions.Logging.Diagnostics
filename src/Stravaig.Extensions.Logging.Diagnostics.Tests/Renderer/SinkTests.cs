using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;
using Stravaig.Extensions.Logging.Diagnostics.Render;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Renderer
{
    [TestFixture]
    public class SinkTests
    {
        [Test]
        public void ManyLogs_ConsoleSink()
        {
            var originalStream = Console.Out;
            var sb = new StringBuilder();
            try
            {
                using var writer = new StringWriter(sb);
                Console.SetOut(writer);
                var logger = new TestCaptureLogger();
                logger.LogTrace("First log at the trace level.");
                logger.LogDebug("Second log at the debug level.");
                logger.LogInformation("Third log at the information level.");
                logger.GetLogs()
                    .RenderLogs(
                        le => $"[{le.LogLevel}] {le.FormattedMessage}",
                        Sink.Console);
            }
            finally
            {
                Console.SetOut(originalStream);
            }

            var consoleOutput = sb.ToString();
            Console.WriteLine(consoleOutput);
            var logLines = consoleOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            
            logLines.Length.ShouldBe(3);
            logLines[0].ShouldBe("[Trace] First log at the trace level.");
            logLines[1].ShouldBe("[Debug] Second log at the debug level.");
            logLines[2].ShouldBe("[Information] Third log at the information level.");
        }
        
        [Test]
        public void ManyLogs_DebugSink()
        {
            var sb = new StringBuilder();
            using var writer = new StringWriter(sb);
            var traceListener = new TextWriterTraceListener(writer);
            try
            {
                Trace.Listeners.Add(traceListener);
                var logger = new TestCaptureLogger();
                logger.LogTrace("First log at the trace level.");
                logger.LogDebug("Second log at the debug level.");
                logger.LogInformation("Third log at the information level.");
                logger.GetLogs()
                    .RenderLogs(
                        le => $"[{le.LogLevel}] {le.FormattedMessage}",
                        Sink.Debug);
            }
            finally
            {
                traceListener.Flush();
                Trace.Listeners.Remove(traceListener);
            }

            var consoleOutput = sb.ToString();
            Console.WriteLine(consoleOutput);
            var logLines = consoleOutput.Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries);
            
            logLines.Length.ShouldBe(3);
            logLines[0].ShouldBe("[Trace] First log at the trace level.");
            logLines[1].ShouldBe("[Debug] Second log at the debug level.");
            logLines[2].ShouldBe("[Information] Third log at the information level.");
        }
    }
}