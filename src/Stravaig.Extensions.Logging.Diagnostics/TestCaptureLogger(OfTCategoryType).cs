using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    /// <summary>
    /// A logger that writes messages to a store that can later be examined
    /// programatically, such as in unit tests.
    /// </summary>
    /// <typeparam name="TCategoryType"></typeparam>
    public class TestCaptureLogger<TCategoryType>
        : TestCaptureLogger, ILogger<TCategoryType>
    {
    }
}