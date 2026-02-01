#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

[TestFixture]
public class LogEntryGetScopePropertiesTests
{
    [Test]
    public void SingleScope_ReturnsScope()
    {
        TestCaptureLogger logger = new();
        logger.LogInformation("Hello, {Name}!", "World");

        var log = logger.GetLogs()[0];
        var scopeProperties = log.GetScopeProperties(0);
        Console.WriteLine(string.Join(Environment.NewLine, scopeProperties.Select(p => $"{p.Key}: {p.Value}")));
        scopeProperties.Count.ShouldBe(2);
        scopeProperties.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var properties = log.Properties;
        properties.Count.ShouldBe(scopeProperties.Count);

        for (int i = 0; i < scopeProperties.Count; i++)
        {
            properties[i].Key.ShouldBe(scopeProperties[i].Key);
            properties[i].Value.ShouldBe(scopeProperties[i].Value);
        }
    }

    [Test]
    public void TwoScopes_ReturnsScopePropertiesForEach()
    {
        TestCaptureLogger logger = new();
        using (logger.BeginScope<KeyValuePair<string, object>[]>([
                   new KeyValuePair<string, object>("OuterScope", "Value")
               ]))
        {
            logger.LogInformation("Hello, {Name}!", "World");
        }

        var log = logger.GetLogs()[0];
        var outerScopeProperties = log.GetScopeProperties(0);
        outerScopeProperties.Count.ShouldBe(1);
        outerScopeProperties.ShouldContain(kvp => kvp.Key == "OuterScope" && (string?)kvp.Value == "Value");

        var scopeProperties = log.GetScopeProperties(1);
        Console.WriteLine(string.Join(Environment.NewLine, scopeProperties.Select(p => $"{p.Key}: {p.Value}")));
        scopeProperties.Count.ShouldBe(2);
        scopeProperties.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var properties = log.Properties;
        properties.Count.ShouldBe(scopeProperties.Count);

        for (int i = 0; i < scopeProperties.Count; i++)
        {
            properties[i].Key.ShouldBe(scopeProperties[i].Key);
            properties[i].Value.ShouldBe(scopeProperties[i].Value);
        }
    }

    [Test]
    public void ThreeScopes_ReturnsScopePropertiesForEach()
    {
        TestCaptureLogger logger = new();
        using (logger.BeginScope<KeyValuePair<string, object>[]>([
                   new KeyValuePair<string, object>("OuterScope", "OuterValue")
               ]))
        {
            using (logger.BeginScope<KeyValuePair<string, object>[]>([
                       new KeyValuePair<string, object>("MiddleScope", "MiddleValue")
                   ]))
            {
                logger.LogInformation("Hello, {Name}!", "World");
            }
        }

        var log = logger.GetLogs()[0];
        var outerScopeProperties = log.GetScopeProperties(0);
        outerScopeProperties.Count.ShouldBe(1);
        outerScopeProperties.ShouldContain(kvp => kvp.Key == "OuterScope" && (string?)kvp.Value == "OuterValue");

        var middleScopeProperties = log.GetScopeProperties(1);
        middleScopeProperties.Count.ShouldBe(1);
        middleScopeProperties.ShouldContain(kvp => kvp.Key == "MiddleScope" && (string?)kvp.Value == "MiddleValue");

        var scopeProperties = log.GetScopeProperties(2);
        Console.WriteLine(string.Join(Environment.NewLine, scopeProperties.Select(p => $"{p.Key}: {p.Value}")));
        scopeProperties.Count.ShouldBe(2);
        scopeProperties.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var properties = log.Properties;
        properties.Count.ShouldBe(scopeProperties.Count);

        for (int i = 0; i < scopeProperties.Count; i++)
        {
            properties[i].Key.ShouldBe(scopeProperties[i].Key);
            properties[i].Value.ShouldBe(scopeProperties[i].Value);
        }
    }
}
