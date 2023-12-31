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
    /// Indicates that the exception is to be written out, if it exists.
    /// </summary>
    Exception = 0x0000_0020,
    
    /// <summary>
    /// Indicates that the exception type is to be written out.
    /// </summary>
    ExceptionType = 0x0000_0040,
    
    /// <summary>
    /// Indicates that the exception message is to be written out.
    /// </summary>
    ExceptionMessage = 0x0000_0080,
    
    /// <summary>
    /// Indicates that inner exceptions are to be written out.
    /// </summary>
    InnerException = 0x0000_0100,
    
    StackTrace = 0x0000_0200,
    
    MessageTemplate = 0x0000_0400,
    
    Properties = 0x0000_0800,
    
    HideNonDeterministicProperties = 0x0000_1000,
    
    /// <summary>
    /// Indicates the default settings are to be used.
    /// </summary>
    Default = Sequence |
              LogLevel |
              CategoryName |
              FormattedMessage |
              Exception |
              ExceptionType |
              ExceptionMessage |
              InnerException,
}