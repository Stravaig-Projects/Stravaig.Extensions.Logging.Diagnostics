---
layout: default
title: Verify Logs - Eceptions
---

# Verifying Exeptions

There are three settings:
* `None`: Does not verify any exceptions.
* `Message`: Verifies the exception message. This can cause brittle tests as the message may contain nondeterministic information.
* `Type`: Verifies the exception type.
* `StackTrace`: Verifies the stack trace of the exception. This can cause brittle tests as code moves around, is refactored, packages updated, etc.
* `IncludeInnerExceptions`: Recursively verifies the inner exceptions.

Each of these settings are flags and can be joined together like the example below.

The default value is `Type` and `IncludeInnerExceptions`.

```csharp
public async Task VerifyWarningLogsAreEmittedCorrectlyTestAsync()
{
    // ... Code that produces logs
    
    VerifySettings verifySettings = new VerifySettings()
        .AddCapturedLogs(new LoggingCaptureVerifySettings()
        {
            Exception = ExceptionSetting.Message | ExceptionSetting.Type | ExceptionSetting.IncludeInnerExceptions,
        });

    await Verifier.Verify(logs, verifySettings);
}
```

## Example output

### None

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: Am error happened that resulted in an exception being thrown.
  }
]
```

### Message

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: Am error happened that resulted in an exception being thrown.,
    Exception: {
      Message: This is the application exception, it contains an inner exception.
    }
  }
]
```

### Type

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: An error happened that resulted in an exception being thrown.,
    Exception: {
      Type: System.ApplicationException
    }
  }
]
```

### StackTrace

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: An error happened that resulted in an exception being thrown.,
    Exception: {
      StackTrace:
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.DoTheThing() in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 60
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.VerifyMessageSettingAsync(ExceptionSetting setting) in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 33
    }
  }
]
```

Note here that Verify has detected the project directory in the output and replaced it with `{ProjectDirectoy}` so that it doesn't break on other team members' machines or on build servers.

### IncludeInnerExceptions

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: An error happened that resulted in an exception being thrown.,
    Exception: {
      InnerException: {}
    }
  }
]
```

This setting on its own isn't very useful.

### All: Type, Message, StackTrace, IncludeInnerExceptions

```
[
  {
    Sequence: 0,
    LogLevel: Error,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests,
    MessageTemplate: An error happened that resulted in an exception being thrown.,
    Exception: {
      Type: System.ApplicationException,
      Message: This is the application exception, it contains an inner exception.,
      StackTrace:
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.DoTheThing() in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 61
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.VerifyMessageSettingAsync(ExceptionSetting setting) in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 34,
      InnerException: {
        Type: System.InvalidOperationException,
        Message: Boo! You performed an invalid operation.,
        StackTrace:
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.PerformThePartOfTheTaskThatActuallyBreaks() in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 68
   at Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyExceptionSettingTests.DoTheThing() in {ProjectDirectory}Verify/VerifyExceptionSettingTests.cs:line 57
      }
    }
  }
]
```