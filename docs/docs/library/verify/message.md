---
layout: default
title: Verify Logs - Message Settings
---

# Verify the log message

Typically, this is the most important part of the log entry. By default the message template is verified.

There are three settings:
* `None`: Does not verify the log message.
* `Formatted`: Verifies the formatted log message with the values filled in. Depending on what the values are this may produce nondeterminisitc output.
* `Template`: Verifies the log template.

The default value is `Template`.

```csharp
public async Task VerifyWarningLogsAreEmittedCorrectlyTestAsync()
{
    // ... Code that produces logs
    
    VerifySettings verifySettings = new VerifySettings()
        .AddCapturedLogs(new LoggingCaptureVerifySettings()
        {
            Message = MessageSetting.Formatted,
        });

    await Verifier.Verify(logs, verifySettings);
}
```

## Example Output

### None

```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyMessageSettingTests
  }
]
```

### Formatted
```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyMessageSettingTests,
    FormattedMessage: This is a test of the Formatted message setting.
  }
]
```

### Template

```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyMessageSettingTests,
    MessageTemplate: This is a test of the {SettingName} message setting.
  }
]
```