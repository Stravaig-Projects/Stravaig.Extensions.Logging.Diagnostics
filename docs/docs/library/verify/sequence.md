---
layout: default
title: Verify Logs - Sequence
---

# Verifying the sequence of log entries

The `LoggingCaptureVerifySettings` has a `Sequence` property that defines how the log sequence is verified. The values are:
* `Hide`: The sequnce is nog logged.
* `ShowAsConsecutive`: The sequence numbers increment consecutively in the verified files. i.e. 0, 1, 2, 3, 4, etc. regardless of whether any logs were filtered out.
* `ShowAsCadence`: The sequence numbers maintain their cadence in the verified files. This allows you to see that there are skipped logs in the verified files that you are not verifying. Say the the original sequence is 25, 27, 32 then it will be verified as 0, 2, 7.

```csharp
public async Task VerifyWarningLogsAreEmittedCorrectlyTestAsync()
{
    // ... Code that produces logs
    
    VerifySettings verifySettings = new VerifySettings()
        .AddCapturedLogs(new LoggingCaptureVerifySettings()
        {
            Sequence = Sequence.ShowAsCadence,
        });

    await Verifier.Verify(logs, verifySettings);
}
```

## Example output

Here are some examples of the verify output for difference `Sequence` settings.

### Sequence.Hide

```
[
  {
    LogLevel: Information,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is the first default log message.
  },
  {
    LogLevel: Warning,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is a warning.
  },
  {
    LogLevel: Error,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: An unexpected error occurred.,
    Exception: {
      Type: System.ApplicationException
    }
  },
]
```

### Sequnce.ShowAsConsecutive

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

### Sequence.ShowAsCadence

```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is the first default log message.
  },
  {
    Sequence: 5,
    LogLevel: Warning,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: This is a warning.
  },
  {
    Sequence: 12,
    LogLevel: Error,
    CategoryName: Stravaig.LoggingExample.Program,
    MessageTemplate: An unexpected error occurred.,
    Exception: {
      Type: System.ApplicationException
    }
  },
]
```