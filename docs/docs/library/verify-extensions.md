---
layout: default
title: Verify extensions
---

# Verify Extensions

**Available from v2.3 in package [Stravaig.Extensions.Logging.Diagnostics.Verify](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics.Verify)**

## What is verify?

From the [Verify](https://github.com/VerifyTests/Verify) docs:
> Verify is a snapshot tool that simplifies the assertion of complex data models and documents.
>
> Verify is called on the test result during the assertion phase. It serializes that result and stores it in a file that matches the test name. On the next test execution, the result is again serialized and compared to the existing file. The test will fail if the two snapshots do not match: either the change is unexpected, or the reference snapshot needs to be updated to the new result.

## How does it work?

When you capture logs you can verify they are as you expect in the assertion phase of your test. Normally, a log may contain nondeterministic data such as date stamps, generated guids and the like. You can configure verify to capture and verify only the parts of the log you are interested, redacting or omitting the nondeterministic parts.

A simple set up might look like this:
```csharp
public async Task MyTestAsync()
{
    var loggingProvider = new TestCaptureLoggerProvider();
    // ... Call code that uses the logger ...

    var capturedLogs = loggingProvider.GetLogs();

    // Set up Verify with the default settings for verifying logs.
    var settings = new VerifySettings().AddCapturedLogs();
    await Verifier.Verify(logger.GetLogs(), settings);
}
```

The verified output might look like this:
```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is the first default log message.
  },
  {
    Sequence: 1,
    LogLevel: Warning,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is a warning.
  },
  {
    Sequence: 2,
    LogLevel: Error,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: An unexpected error occurred.,
    Exception: {
      Type: System.ApplicationException
    }
  }
]
```

The default settings emit 5 things: 
* The Sequence number,
* The log level,
* The category name (usually the name of the class that generated the log),
* The message template, and
* the exception if it exists.

The reson it doesn't emit the formatted message or exception message by default is that can be nondeterministic. Nondeterministic values in the verified file produce brittle tests, so the parts of the log most likely to be brittle are ommited by default. Therefore, things like the formatted message or exception messages are not shown unless explicitly requested.

## How do I set up to verify the things I want?

* Configure the [Sequence](verify/sequence.md)
* Verify the [LogLevel](verify/log-level.md)
* Verify the [CategoryName](verify/category-name.md)
