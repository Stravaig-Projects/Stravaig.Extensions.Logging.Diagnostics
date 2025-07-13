# About

Stravaig Log Capture is a way to capture the logs in a test and examine them to ensure they are being generated correctly.

## Version & Framework support

* v1.x: Supports .NET Core 3.1 & .NET 5
* v2.x: Supports .NET 6.0, 7.0 & 8.0
* v3.x: Supports .NET 6.0, 8.0 & 9.0
* v4.x: Supports .NET 8.0 & 9.0

## Why do I want to test my logs?

Checking logs in test can be beneficial for a number of reasons.
For example, in a background service the log is effectively its user
interface (the user being the developer or system admin whose job it
is to monitor the correct running of background services). By verifying
the logs in tests, you can ensure that appropriate information is being
delivered.

Logging may also exist to output audit trails for the application, and you
need to check in tests that it is being produced correctly.

## How to use

For simple tests you can pass the logger into the class under test like this:

```csharp
[Test]
public void EnsureSomeFunctionalityWorks()
{
    // Arrange
    var logger = new TestCaptureLogger<ServiceClass>();
    var service = new ServiceClass(logger);
    
    // Act
    service.DoSomething();
    
    // Assert
    var logs = logger.GetLogs();
    logs.Count.ShouldBe(1);
    logs[0].OriginalMessage.ShouldBe("Did something.");
}
```

## See also

* [Full Documentation](https://stravaig-projects.github.io/Stravaig.Extensions.Logging.Diagnostics/)
* [Release Notes](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/releases)
