using System;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

/// <summary>
/// The settings for verifying properties.
/// </summary>
[Flags]
public enum PropertySetting
{
    /// <summary>
    /// The properties are not to be rendered.
    /// </summary>
    None = 0x0,
    
    /// <summary>
    /// All property information is shown.
    /// </summary>
    /// <remarks>Any nondeterministic properties are ignored, unless
    /// RedactNonDeterministic is also set.</remarks>
    Verify = 0x1,
    
    /// <summary>
    /// Nondeterministic properties are shown in a redacted form.
    /// </summary>
    /// <remarks>This will have no effect unless Verify is also set.</remarks>
    RedactNonDeterministic = 0x2,
    
    /// <summary>
    /// When determining property matched against the collection of
    /// nondeterministic properties, use a case-insensitive check.
    /// </summary>
    CaseInsensitivePropertyMatch = 0x4,
}