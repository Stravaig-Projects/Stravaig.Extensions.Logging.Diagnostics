using System;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

[Flags]
public enum Settings
{
    /// <summary>
    /// Indicates that the sequence number is to be written out. By default
    /// consecutive sequence numbers are written, starting at zero.
    /// </summary>
    /// <remarks>The sequence numbers are resequenced to be consecutive starting
    /// at zero so they are deterministic for the test.
    /// e.g. Original sequence is 25, 27, 32, written sequence number are 0, 1, 2.
    /// </remarks>
    Sequence = 0x0000_0001,
    
    /// <summary>
    /// When writing a sequence number, keeps the cadence of the original. Note:
    /// This may not be suitable if tests or code under test are running in
    /// parallel.
    /// </summary>
    /// <remarks>When the cadence is kept then the gaps between sequence numbers
    /// are kept rather than being compressed to consecutive number.
    /// e.g. Original sequence is 25, 27, 32, written sequence number are 0, 2, 7.
    /// </remarks>
    KeepSequenceCadence = 0x0000_0002,
    
    /// <summary>
    /// Indicates that the log level is to be written out.
    /// </summary>
    LogLevel = 0x0000_0004,
    
    /// <summary>
    /// Indicates that the category name is to be written out.
    /// </summary>
    CategoryName = 0x0000_0008,
    
    /// <summary>
    /// Indicates that the formatted message is to be written out.
    /// </summary>
    FormattedMessage = 0x0000_0010,
    
    /// <summary>
    /// Indicates the default settings are to be used.
    /// </summary>
    Default = Sequence | LogLevel | CategoryName | FormattedMessage,
}

internal static class SettingsExtensions
{
    internal static bool Use(this Settings settings, Settings setting) => (settings & setting) != 0;

    internal static bool WriteSequence(this Settings settings) => settings.Use(Settings.Sequence);
    internal static bool WriteLogLevel(this Settings settings) => settings.Use(Settings.LogLevel);
    internal static bool WriteCategoryName(this Settings settings) => settings.Use(Settings.CategoryName);
    internal static bool WriteFormattedMessage(this Settings settings) => settings.Use(Settings.FormattedMessage);
}