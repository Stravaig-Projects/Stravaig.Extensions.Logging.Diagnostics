using System;
using System.Collections.Generic;
using Stravaig.Extensions.Logging.Diagnostics.Render;
using Xunit.Abstractions;

namespace Stravaig.Extensions.Logging.Diagnostics.XUnit;

/// <summary>
/// An extension class to extend the ITestOutputHelper in order to render logs.
/// </summary>
public static class OutputTestHelperExtensions
{
    /// <summary>
    /// Write the logs to the xunit test output helper.
    /// </summary>
    /// <param name="output">The xunit test output helper.</param>
    /// <param name="logEntries">The log entries.</param>
    /// <param name="formatter"></param>
    public static void WriteLogs(this ITestOutputHelper output, IEnumerable<LogEntry> logEntries, Func<LogEntry, string>? formatter = null)
    {
        logEntries.RenderLogs(formatter ?? Formatter.SimpleBySequence, output.WriteLine);
    }
}