using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

/// <summary>
/// Extension methods for configuring the captured logs verify settings.
/// </summary>
public static class VerifySettingsExtensions
{
    /// <summary>
    /// Adds the settings required for verifying captured log entries using Stravaig Logging Diagnostics.
    /// </summary>
    /// <param name="verifySettings">The verify settings object to modify.</param>
    /// <param name="settings">The settings for verifying the captured logs.</param>
    public static VerifySettings AddCapturedLogs(this VerifySettings verifySettings, LoggingCaptureVerifySettings? settings = null)
    {
        settings ??= LoggingCaptureVerifySettings.Default;
        verifySettings.AddExtraSettings(jsonSettings =>
        {
            // Removes the default version if it exists.
            jsonSettings.Converters.RemoveAll(c => c is LogEntryConverter);

            var logEntryConverter = new LogEntryConverter(settings);
            jsonSettings.Converters.Add(logEntryConverter);
        });
        return verifySettings;
    }
}