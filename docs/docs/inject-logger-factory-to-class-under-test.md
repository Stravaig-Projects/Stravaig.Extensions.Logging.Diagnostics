---
layout: default
title: Injecting a logger factory to the class under test
---

# Injecting a logger factory to the class under test

You can inject a logger factory that has been suitably configured. You must remember to keep a hold of the TestCaptureProvider in your test so that you can access the logs afterwards. Something like this would work:

```csharp
public class MyService
{
  private readonly ILogger<MyService> _logger;

  public MyService(ILoggerFactory loggerFactory)
  {
    _logger = loggerFactory.CreateLogger<MyService>();
  }

  public void DoStuff()
  {
    // Do whatever the service needs to do.
    _logger.LogInformation("The work is done.");
  }
}
```

And the corresponding test might look like this:

```csharp
[Test]
public void TestServiceLoggerCalled()
{
  // Arrange
  var logProvider = new TestCaptureLoggerProvider();
  var factory = LoggerFactory.Create(builder =>
  {
    builder.AddProvider(logProvider);
  });
  var service = new MyService(factory);

  // Act
  service.DoStuff();

  // Assert
  var logs = logProvider.GetLogEntriesFor<MyService>();
  logs.Count.ShouldBe(1);
  logs[0].FormattedMessage.ShouldContain("The work is done.");
}
```

## Alternative

Because the `ILoggerFactory` provides a method `CreateLogger(Type)` override, the `TestCaptureLoggerProvider` also provides a corresponding `GetLogEntriesFor(Type)` override. While generally it would be expected that the generic version for `CreateLogger<T>()` and `GetLogEntriesFor<T>()` would be used, the override that takes a `Type` is useful for when the type is a `static` class as you can't use static classes in generics, or in scenarios where the exact type is unknown at compile time. e.g. Code in a base class may use `GetType()` in the call to `CreateLogger()` and `GetLogEntriesFor()` to get the logger/entries for the concrete class.