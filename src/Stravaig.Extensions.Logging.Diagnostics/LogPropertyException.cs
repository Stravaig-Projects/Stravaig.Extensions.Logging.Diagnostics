using System;

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
/// An exception that indicates an issue with the log property.
/// </summary>
public class LogPropertyException : Exception
{
    /// <summary>
    /// Creates an exception indicating an issue with the Log Property.
    /// </summary>
    /// <param name="message">Information detailing the issue with the log property.</param>
    public LogPropertyException(string message)
        : base(message)
    {
    }
}
