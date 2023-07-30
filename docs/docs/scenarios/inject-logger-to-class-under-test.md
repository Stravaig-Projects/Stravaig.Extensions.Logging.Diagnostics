---
layout: default
title: Injecting a logger to the class under test
---

# Injecting a logger to the class under test

If you just need to inject a logger into the class you are testing then you can simply create an instance of a `TestCaptureLogger` and inject it in.

For example, say you need to test a service class that takes a logger. The class might look something like this:

```csharp
public class MyService
{
  private readonly ILogger<MyService> _logger;

  public MyService(ILogger<MyService> logger)
  {
    _logger = logger;
  }

  public void DoStuff()
  {
    // Do whatever the service needs to do.
    _logger.LogInformation("The work is done.");
  }
}
```

To test that the logger was called with the relevant message you can create a test like this:

```csharp
[Test]
public void TestServiceLoggerCalled()
{
  // Arrange
  var logger = new TestCaptureLogger<MyService>();
  var service = new MyService(logger);

  // Act
  service.DoStuff();

  // Assert
  var logs = logger.GetLogs();
  logs.Count.ShouldBe(1);
  logs[0].FormattedMessage.ShouldContain("The work is done.");
}
```

Note: The above example uses [NUnit](https://www.nuget.org/packages/NUnit/) as the test framework, and [Shouldly](https://www.nuget.org/packages/Shouldly/3.0.2) as the assertion framework.