---
layout: default
title: Verify Logs - Sequence
---

# Verifying the log level of log entries

The `LoggingCaptureVerifySettings` has a Boolean `LogLevel` property that defines if the log level is to be verified or not.

The default value is `true`.

```csharp
public async Task VerifyWarningLogsAreEmittedCorrectlyTestAsync()
{
    // ... Code that produces logs
    
    VerifySettings verifySettings = new VerifySettings()
        .AddCapturedLogs(new LoggingCaptureVerifySettings()
        {
            LogLevel = true,
        });

    await Verifier.Verify(logs, verifySettings);
}
```

## Example output

### LogLevel = false

```
[
  {
    Sequence: 0,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is the first default log message.
  },
  {
    Sequence: 1,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is a warning.
  },
  {
    Sequence: 2,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: An unexpected error occurred.,
    Exception: {
      Type: System.ApplicationException
    }
  },
]
```

### LogLevel = true

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
  },
]
```