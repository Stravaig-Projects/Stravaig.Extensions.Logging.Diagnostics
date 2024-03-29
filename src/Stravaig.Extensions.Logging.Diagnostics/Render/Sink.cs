using System;

namespace Stravaig.Extensions.Logging.Diagnostics.Render
{
    /// <summary>
    /// Basic sinks for writing captured log entries.
    /// </summary>
    public static class Sink
    {
        /// <summary>
        /// A console sink for the log renderer.
        /// </summary>
        public static Action<string> Console => System.Console.WriteLine;
    }
}