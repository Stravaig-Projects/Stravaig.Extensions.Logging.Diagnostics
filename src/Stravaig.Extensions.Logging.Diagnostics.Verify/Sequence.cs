namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

/// <summary>
/// Indicates the ways that a sequence can be rendered.
/// </summary>
public enum Sequence
{
    /// <summary>
    /// Hides the sequence number.
    /// </summary>
    Hide,
    
    /// <summary>
    /// Shows the sequence of the logs as a zero based consecutive sequence
    /// (i.e. 0, 1, 2, 3...) regardless of the actual sequence number of the log
    /// entry.
    /// </summary>
    ShowAsConsecutive,
    
    /// <summary>
    /// Shows the sequence of the logs as a zero based sequence retaining the
    /// cadence of the original logs. (e.g. the original sequence is 25, 27, 32
    /// then it will be written as 0, 2, 7)
    /// </summary>
    ShowAsCadence,
}