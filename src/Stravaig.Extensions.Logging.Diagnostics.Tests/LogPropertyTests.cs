using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Shouldly;

namespace Stravaig.Extensions.Logging.Diagnostics.Tests;

public class LogPropertyTests
{
    [Test]
    public void CheckExistingPropertyDetailsAreAvailableToTest()
    {
        var logger = new TestCaptureLogger<LogPropertyTests>();

        logger.LogInformation(
            "String Property {StringProperty};",
            "Some string value");

        var log = logger.GetLogs()[0];

        log["StringProperty"].Name.ShouldBe("StringProperty");
        log["StringProperty"].Exists.ShouldBeTrue();
        log["StringProperty"].IsOfType<string>().ShouldBeTrue();
        log["StringProperty"].HasValue.ShouldBeTrue();
        log["StringProperty"].GetValue<string>().Length.ShouldBe(17);
        log["StringProperty"].Value.ShouldBe("Some string value");

        // Alternative way of expressing the previous asserts, but this will
        // list all the failures rather than stop at the first one.
        log["StringProperty"].ShouldSatisfyAllConditions(
            lp => lp.Name.ShouldBe("StringProperty"),
            lp => lp.Exists.ShouldBeTrue(),
            lp => lp.IsOfType<string>().ShouldBeTrue(),
            lp => lp.HasValue.ShouldBeTrue(),
            lp => lp.Value.ShouldBe("Some string value"));
    }

    [Test]
    public void CheckNonexistentPropertyRespondsAppropriately()
    {
        var logger = new TestCaptureLogger<LogPropertyTests>();
        logger.LogInformation("Some log message");

        var log = logger.GetLogs()[0];

        log["Nonexistent"].Name.ShouldBe("Nonexistent");
        log["Nonexistent"].Exists.ShouldBeFalse();
        log["Nonexistent"].IsOfType<string>().ShouldBeFalse();
        Should.Throw<LogPropertyException>(() => log["Nonexistent"].Value)
            .Message.ShouldBe("Cannot get the value for the log property 'Nonexistent' as it does not exist.");
    }

}
