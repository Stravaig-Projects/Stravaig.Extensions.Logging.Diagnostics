using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public static class VerifySettingsExtensions
{
    /// <summary>
    /// Adds the settings required for verifying captured log entries using Stravaig Logging Diagnostics.
    /// </summary>
    /// <param name="verifySettings">The verify settings object to modify.</param>
    /// <param name="settings">The settings for verifying the tests.</param>
    /// <returns>The verify Settings object.</returns>
    public static VerifySettings AddStravaigTests(this VerifySettings verifySettings, Settings settings = Settings.Default)
    {
        verifySettings.AddExtraSettings(jsonSettings =>
        {
            // Remove the default version if it exists.
            jsonSettings.Converters.RemoveAll(c => c is LogEntryConverter);
            
            // Create replacement converter.
            var logEntryConverter = new LogEntryConverter(settings);
            jsonSettings.Converters.Add(logEntryConverter);
        });
        return verifySettings;
    }
}