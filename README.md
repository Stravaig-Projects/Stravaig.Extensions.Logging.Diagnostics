# Stravaig.Extensions.Logging.Diagnostics

A logger for use in tests so that the messages logged can be examined in tests.

* ![Build Stravaig.Extensions.Logging.Diagnostics](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/workflows/Build%20Stravaig.Extensions.Logging.Diagnostics/badge.svg)
* ![Nuget](https://img.shields.io/nuget/v/Stravaig.Extensions.Logging.Diagnostics?color=004880&label=nuget%20stable&logo=nuget)
* ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Stravaig.Extensions.Logging.Diagnostics?color=ffffff&label=nuget%20latest&logo=nuget)

## Usage

### Simply injecting a logger

If you just need to inject a logger into the class you are testing then you can simply create an instance of a `TestCaptureLogger` and inject it in.

For example, say you need to test a service class that takes a logger. The class might look something like this:

```
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

```
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

```
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

```
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

