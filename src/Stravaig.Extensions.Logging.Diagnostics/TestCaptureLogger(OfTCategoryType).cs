using System;
using Microsoft.Extensions.Logging;
using Stravaig.Extensions.Logging.Diagnostics.ExternalHelpers;

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
        /// <summary>
        /// Initialises a new instance of the <see cref="T:Stravaig.Extensions.Logging.Diagnostics.TestCaptureLogger&lt;TCategoryType>"/> class.
        /// </summary>
        public TestCaptureLogger()
            : base(TypeNameHelper.GetTypeDisplayName(typeof(TCategoryType)))
        {
        }
    }
}