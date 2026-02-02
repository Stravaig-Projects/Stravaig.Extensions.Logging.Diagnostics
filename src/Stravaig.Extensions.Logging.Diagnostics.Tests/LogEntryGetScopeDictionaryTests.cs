#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

[TestFixture]
public class LogEntryGetScopeDictionaryTests
{
    [Test]
    public void SingleScope_ReturnsScope()
    {
        TestCaptureLogger logger = new();
        logger.LogInformation("Hello, {Name}!", "World");

        var log = logger.GetLogs()[0];
        var scopeDictionary = log.GetScopeDictionary(0);
        Console.WriteLine(string.Join(Environment.NewLine, scopeDictionary.Select(p => $"{p.Key}: {p.Value}")));
        scopeDictionary.Count.ShouldBe(2);
        scopeDictionary.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var propDictionary = log.PropertyDictionary;
        propDictionary.Count.ShouldBe(scopeDictionary.Count);

        foreach (var key in scopeDictionary.Keys)
        {
            propDictionary.ContainsKey(key).ShouldBeTrue();
            propDictionary[key].ShouldBe(scopeDictionary[key]);
        }
    }

    [Test]
    public void TwoScopes_ReturnsScopeDictionaryForEach()
    {
        TestCaptureLogger logger = new();
        using (logger.BeginScope<KeyValuePair<string, object>[]>([
                   new KeyValuePair<string, object>("OuterScope", "Value")
               ]))
        {
            logger.LogInformation("Hello, {Name}!", "World");
        }

        var log = logger.GetLogs()[0];
        var outerScopeDictionary = log.GetScopeDictionary(0);
        outerScopeDictionary.Count.ShouldBe(1);
        outerScopeDictionary.Keys.Single().ShouldBe("OuterScope");
        outerScopeDictionary.Values.Single().ShouldBe("Value");

        var scopeDictionary = log.GetScopeDictionary(1);
        Console.WriteLine(string.Join(Environment.NewLine, scopeDictionary.Select(p => $"{p.Key}: {p.Value}")));
        scopeDictionary.Count.ShouldBe(2);
        scopeDictionary.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var propDictionary = log.PropertyDictionary;
        propDictionary.Count.ShouldBe(scopeDictionary.Count);

        foreach (var key in scopeDictionary.Keys)
        {
            propDictionary.ContainsKey(key).ShouldBeTrue();
            propDictionary[key].ShouldBe(scopeDictionary[key]);
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
        var outerScopeDictionary = log.GetScopeDictionary(0);
        outerScopeDictionary.Count.ShouldBe(1);
        outerScopeDictionary.ShouldContain(kvp => kvp.Key == "OuterScope" && (string?)kvp.Value == "OuterValue");

        var middleScopeDictionary = log.GetScopeDictionary(1);
        middleScopeDictionary.Count.ShouldBe(1);
        middleScopeDictionary.ShouldContain(kvp => kvp.Key == "MiddleScope" && (string?)kvp.Value == "MiddleValue");

        var innerScopeDictionary = log.GetScopeDictionary(2);
        Console.WriteLine(string.Join(Environment.NewLine, innerScopeDictionary.Select(p => $"{p.Key}: {p.Value}")));
        innerScopeDictionary.Count.ShouldBe(2);
        innerScopeDictionary.ShouldSatisfyAllConditions(
            p => p.ShouldContain(kvp => kvp.Key == "Name" && (string?)kvp.Value == "World"),
            p => p.ShouldContain(kvp => kvp.Key == "{OriginalFormat}" && (string?)kvp.Value == "Hello, {Name}!"));

        var propDictionary = log.PropertyDictionary;
        propDictionary.Count.ShouldBe(innerScopeDictionary.Count);

        foreach (var key in innerScopeDictionary.Keys)
        {
            propDictionary.ContainsKey(key).ShouldBeTrue();
            propDictionary[key].ShouldBe(innerScopeDictionary[key]);
        }
    }
}
