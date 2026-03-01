using System;
using System.Collections.Generic;
using Stravaig.Extensions.Logging.Diagnostics.Render;
using Xunit;

namespace Stravaig.Extensions.Logging.Diagnostics.XUnit;

/// <summary>
/// An extension class to extend the ITestOutputHelper in order to render logs.
/// </summary>
public static class OutputTestHelperExtensions
{
    /// <param name="output">The xunit test output helper.</param>
    extension(ITestOutputHelper output)
    {
        /// <summary>
        /// Write the logs to the xunit test output helper.
        /// </summary>
        /// <param name="logEntries">The log entries.</param>
        /// <param name="formatter">An optional formatter that defines how the log entry is to be written.</param>
        public void WriteLogs(IEnumerable<LogEntry> logEntries, Func<LogEntry, string>? formatter = null)
        {
            logEntries.RenderLogs(formatter ?? Formatter.SimpleBySequence, output.WriteLine);
        }

        /// <summary>
        /// Write the logs to the xunit test output helper.
        /// </summary>
        /// <param name="logger">The logger from which to output the captured logs.</param>
        /// <param name="formatter">An optional formatter that defines how the log entry is to be written.</param>
        public void WriteLogs(ITestCaptureLogger logger, Func<LogEntry, string>? formatter = null)
            => output.WriteLogs(logger.GetLogs(), formatter);

        /// <summary>
        /// Write the logs to the xunit test output helper.
        /// </summary>
        /// <param name="provider">The log provider from which to output the captured logs.</param>
        /// <param name="formatter">An optional formatter that defines how the log entry is to be written.</param>
        public void WriteLogs(TestCaptureLoggerProvider provider, Func<LogEntry, string>? formatter = null)
            => output.WriteLogs(provider.GetAllLogEntries(), formatter);
    }
}
