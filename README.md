# Stravaig.Extensions.Logging.Diagnostics

A logger for use in tests so that the messages logged can be examined in tests.

* ![Build Stravaig.Extensions.Logging.Diagnostics](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/workflows/Build%20Stravaig.Extensions.Logging.Diagnostics/badge.svg) [View Workflows](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/actions)
* ![Nuget](https://img.shields.io/nuget/v/Stravaig.Extensions.Logging.Diagnostics?color=004880&label=nuget%20stable&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)
* ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Stravaig.Extensions.Logging.Diagnostics?color=ffffff&label=nuget%20latest&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)

## Usage

### Simply injecting a logger

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
  logger.Logs.Count.ShouldBe(1);
  logger.Logs[0].FormattedMessage.ShouldContain("The work is done.");
}
```

Note: The above example uses [NUnit](https://www.nuget.org/packages/NUnit/) as the test framework, and [Shouldly](https://www.nuget.org/packages/Shouldly/3.0.2) as the assetion framework.

### With a logger factory

You can also inject a logger factory that has been suitably configured. You must remember to keep a hold of the TestCaptureProvider in your test so that you can access the logs afterwards. Something like this would work:

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

#### Alternative

Because the `ILoggerFactory` provides a method `CreateLogger(Type)` override, the `TestCaptureLoggerProvider` also provides a corresponding `GetLogEntriesFor(Type)` override. While generally it would be expected that the generic version for `CreateLogger<T>()` and `GetLogEntriesFor<T>()` would be used, the override that takes a `Type` is useful for when the type is a `static` class as you can't use static classes in generics, or in scenarios where the exact type is unknown at compile time. e.g. Code in a base class may use `GetType()` in the call to `CreateLogger()` and `GetLogEntriesFor()` to get the logger/entries for the concrete class.

### With a `WebApplicationFactory`

The `WithWebHostBuilder` method on the factory allows you to configure the logging. Before setting up the web host, create a `TestCaptureLoggerProvider` in a place accessible from your test, then use in in the `ConfigureLogging` method, and run your test. After the test is run you can use the `TestCaptureLoggerProvider` to `GetLogEntriesFor<T>()` method to access the log entries that were made in the code under test.

e.g.

```csharp
// Arrange or SetUp portion of test
var logProvider = new TestCaptureLoggerProvider();
using var factory = new WebApplicationFactory<Startup>()
  .WithWebHostBuilder(builder =>
  {
    builder.ConfigureLogging(loggingBuilder =>
    {
       loggingBuilder.AddProvider(logProvider);
    });
  });

// Act : Do what ever you need to run your test

// Assert
var entries = logProvider.GetLogEntriesFor<T>();
// Assert the entries from the logger.
```

### Example project

In this repository there is an example project showing how this may be used in NUnit tests.

* [Example Solution](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/tree/main/Example)
