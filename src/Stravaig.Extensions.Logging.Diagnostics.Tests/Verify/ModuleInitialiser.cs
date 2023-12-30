using System.Runtime.CompilerServices;
using DiffEngine;
using Stravaig.Extensions.Logging.Diagnostics.Verify;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Verify;

public static class ModuleInitialiser
{
    [ModuleInitializer]
    public static void InitVerifyStravaigLoggingCapture()
    {
        DiffTools.UseOrder(DiffTool.VisualStudioCode, DiffTool.Rider);
        VerifyStravaigLoggingCapture.Initialise();
    }
}