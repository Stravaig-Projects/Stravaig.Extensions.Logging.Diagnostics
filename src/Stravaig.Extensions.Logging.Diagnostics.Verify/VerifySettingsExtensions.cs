using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public static class VerifySettingsExtensions
{
    /// <summary>
    /// Adds the settings required for verifying captured log entries using Stravaig Logging Diagnostics.
    /// </summary>
    /// <param name="verifySettings">The verify settings object to modify.</param>
    /// <param name="settings">The settings for verifying the tests.</param>
    /// <param name="nonDeterministicPropertyNames">A collection of property names to exclude from the output because their values are non-deterministic</param>
    /// <returns>The verify Settings object.</returns>
    
    public static VerifySettings AddStravaigTests(this VerifySettings verifySettings, Settings settings = Settings.Default, IEnumerable<string>? nonDeterministicPropertyNames = null, string nonDeterministicPropertyValueSubstitute = LogEntryConverter.DefaultNonDeterministicPropertyValueSubstitute)
    {
        var nonDeterministicPropertySet = ImmutableHashSet.Create(StringComparer.Ordinal, nonDeterministicPropertyNames?.ToArray() ?? Array.Empty<string>());
        verifySettings.AddExtraSettings(jsonSettings =>
        {
            // Remove the default version if it exists.
            jsonSettings.Converters.RemoveAll(c => c is LogEntryConverter);
            
            // Create replacement converter.
            var logEntryConverter = new LogEntryConverter(settings, nonDeterministicPropertySet, nonDeterministicPropertyValueSubstitute);
            jsonSettings.Converters.Add(logEntryConverter);
        });
        return verifySettings;
    }
}