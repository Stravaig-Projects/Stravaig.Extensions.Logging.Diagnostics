using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

[TestFixture]
public class LogEntryScopeTests
{
    [Test]
    public void ScopesCaptureNestedScopesAndFlattenedPropertiesPreferInnerMost()
    {
        var logger = new TestCaptureLogger("MyCategory");

        using (logger.BeginScope(new[] { new KeyValuePair<string, object?>("CorrelationId", "outer") }))
        using (logger.BeginScope(new[] { new KeyValuePair<string, object?>("CorrelationId", "inner") }))
        {
            logger.LogInformation("Hello {Name}", "World");
        }

        var log = logger.GetLogs()[0];
        log.ScopeLevels.ShouldBe(3);
        log.Scopes[log.ScopeLevels - 1].ShouldBeSameAs(log.State);

        log.GetScopePropertyDictionary(0)["CorrelationId"].ShouldBe("outer");
        log.GetScopePropertyDictionary(1)["CorrelationId"].ShouldBe("inner");
        log.GetScopePropertyDictionary(log.ScopeLevels - 1)["Name"].ShouldBe("World");

        int correlationIdCount = 0;
        foreach (var kvp in log.GetFlattenedScopeProperties())
        {
            if (kvp.Key == "CorrelationId")
                correlationIdCount++;
        }

        correlationIdCount.ShouldBe(2);
        log.GetFlattenedScopePropertyDictionary()["CorrelationId"].ShouldBe("inner");
    }
}
