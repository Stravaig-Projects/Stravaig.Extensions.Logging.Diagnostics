#nullable enable
using System;
using System.Reflection;
using System.Threading;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Helpers;

public static class LogEntryHelper
{
#if NET9_0_OR_GREATER
    public static readonly Lock SequenceSyncLock;
#else
    public static readonly object SequenceSyncLock;
#endif

    static LogEntryHelper()
    {
        var logEntryType = typeof(LogEntry);
        var field = logEntryType.GetField("SequenceSyncLock", BindingFlags.Static | BindingFlags.NonPublic);
        field.ShouldNotBeNull("Expected LogEntry to have a private static field called SequenceSyncLock.");

        var lockValue = field.GetValue(null);
        lockValue.ShouldNotBeNull("Expected LogEntry.SequenceSyncLock to have a value.");

#if NET9_0_OR_GREATER
        SequenceSyncLock = (Lock)lockValue;
#else
        SequenceSyncLock = lockValue;
#endif

    }

    public static void ResetLogSequence(int sequence = 0)
    {
        var logEntryType = typeof(LogEntry);
        var field = logEntryType.GetField("_sequence", BindingFlags.Static | BindingFlags.NonPublic);
        field.ShouldNotBeNull("Expected LogEntry to have a private static field called _sequence.");
        lock (SequenceSyncLock)
        {
            field.SetValue(null, sequence);
            field.GetValue(null).ShouldBe(sequence);
        }
    }

    public static void ResetTimeProvider() => SetTimeProvider(TimeProvider.System);

    public static void SetTimeProvider(TimeProvider timeProvider)
    {
        timeProvider ??= TimeProvider.System;
        var logEntryType = typeof(LogEntry);
        var field = logEntryType.GetField("_timeProvider", BindingFlags.Static | BindingFlags.NonPublic);
        field.ShouldNotBeNull("Expected LogEntry to have a private static field called _timeProvider.");

        var lastTimestampField = logEntryType.GetField("_lastTimestampUtc", BindingFlags.Static | BindingFlags.NonPublic);
        lastTimestampField.ShouldNotBeNull("Expected LogEntry to have a private static field called _lastTimestampUtc.");

        lock (SequenceSyncLock)
        {
            field.SetValue(null, timeProvider);
            field.GetValue(null).ShouldBe(timeProvider);

            // Reset the last timestamp to 0 to allow the new time provider to work correctly
            lastTimestampField.SetValue(null, 0L);
        }
    }
}
