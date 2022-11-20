using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests.Net7.Helpers;

#if NET7_0_OR_GREATER

public class SomeOpenService
{
    public SomeOpenService(ILoggerFactory logFactory)
    {
        var fileService = new SomeFileScopedService(logFactory);
    }
}


file class SomeFileScopedService
{
	public SomeFileScopedService(ILoggerFactory logFactory)
    {
        var logger = logFactory.CreateLogger<SomeFileScopedService>();
        logger.LogInformation("This is from a file scoped class!");
    }
}

#endif