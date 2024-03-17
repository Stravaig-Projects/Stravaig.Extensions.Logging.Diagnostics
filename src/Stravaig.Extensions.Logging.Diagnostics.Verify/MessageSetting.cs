namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public enum MessageSetting
{
    /// <summary>
    /// The message is not to be verified.
    /// </summary>
    None,
    
    /// <summary>
    /// The formatted message is to be verified.
    /// </summary>
    /// <remarks>Formatted messages may contain non-deterministic information,
    /// in which case this should not be used to prevent brittle tests.</remarks>
    Formatted,
    
    /// <summary>
    /// The message template is to be verified.
    /// </summary>
    Template,
}