---
layout: default
title: Using the logger with a WebApplicationFactory
---

### Using the logger With a `WebApplicationFactory`

The `WithWebHostBuilder` method on the factory allows you to configure the logging. Before setting up the web host, create a `TestCaptureLoggerProvider` in a place accessible from your test, then use in in the `ConfigureLogging` method, and run your test. After the test is run you can use the `TestCaptureLoggerProvider` to `GetLogEntriesFor<T>()` method to access the log entries that were made in the code under test.

e.g.

```csharp
[Test]
public void TestTheLogs()
{
  // Arrange: You need to set up the logProvider & add it to the WebApplicationFactory
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

  // Assert : Check the logs for the conditions you expect
  var entries = logProvider.GetLogEntriesFor<T>();
  // entries[0].FormattedMessage.ShouldBe(...);
  // etc.
}
```

