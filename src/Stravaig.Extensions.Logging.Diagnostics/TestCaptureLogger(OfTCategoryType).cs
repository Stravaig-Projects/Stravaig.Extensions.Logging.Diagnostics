using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics
{
    public class TestCaptureLogger<TCategoryType>
        : TestCaptureLogger, ILogger<TCategoryType>
    {
    }
}