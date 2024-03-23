---
layout: default
title: Verify Logs - Category Name
---

# Verifying the category name of log entries

Typically the category name will be the name of the type where the logger is used. e.g.

```csharp
namespace MyApp.Services;

public class MyService
{
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }

    public void DoTheThing()
    {
        _logger.LogInformation("Doing the thing.");
    }
}
```

For the above code the category name would be `MyApp.Services.MyService`.

## Configuring verifying the Category Name

The default value is `true`.

```csharp
public async Task VerifyWarningLogsAreEmittedCorrectlyTestAsync()
{
    // ... Code that produces logs
    
    VerifySettings verifySettings = new VerifySettings()
        .AddCapturedLogs(new LoggingCaptureVerifySettings()
        {
            CategoryName = true,
        });

    await Verifier.Verify(logs, verifySettings);
}
```

### Example output

#### CategoryName = true

```
[
  {
    Sequence: 0,
    LogLevel: Information,
    CategoryName: Stravaig.Extensions.Logging.Diagnostics.Tests.Verify.VerifyCategoryNameTests,
    MessageTemplate: This is the first log. It is from the test class's category.
  },
  {
    Sequence: 1,
    LogLevel: Information,
    CategoryName: custom-category-name,
    MessageTemplate: This is the second log. It has a custom category name.
  }
]
```

#### CategoryName = false

```
[
  {
    Sequence: 0,
    LogLevel: Information,
    MessageTemplate: This is the first log. It is from the test class's category.
  },
  {
    Sequence: 1,
    LogLevel: Information,
    MessageTemplate: This is the second log. It has a custom category name.
  }
]
```