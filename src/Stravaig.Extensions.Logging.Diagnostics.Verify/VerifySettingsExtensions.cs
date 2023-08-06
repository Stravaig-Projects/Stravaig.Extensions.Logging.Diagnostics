using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public static class VerifySettingsExtensions
{
    public static VerifySettings AddStravaigTests(this VerifySettings verifySettings)
    {
        verifySettings.AddExtraSettings(jsonSettings =>
        {
            jsonSettings.Converters.Add(new LogEntryConverter());
        });
        return verifySettings;
    }
}