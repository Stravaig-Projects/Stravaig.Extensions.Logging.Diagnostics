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

## When is this useful?

`ShowAsConsecutive` is useful if you want to verify logs in a custom order, but still want to verify which sequence they were originally in.

`ShowAsCadence` is useful if you are only interested in verifying certain logs (e.g. Warnings and above), and want to show that there were some skipped logs in between those you are verifying. However, this may intruduce some brittleness to the verify process as it is easy to insert or remove logs that you may not wish to test for when diagnosing bugs or tidying up the application.

`Hide` is useful if you want to reduce the size of the verify files and you have the logs directly from one of the various `Get...Logs()` methods on the `TestCaptureLoggerProvider` or `TestCaptureLogger`, as they return the logs in the sequence in which they were logged.

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