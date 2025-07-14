# About

XUnit extensions for [Stravaig Log Capture](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics/) add helper extension methods for outputting the captured logs via XUnit's `IOutputTestHelper`.

## Version & Framework support

* v2.x: Supports .NET 6.0, 7.0 & 8.0
* v3.x: Supports .NET 6.0, 8.0 & 9.0
* v4.x: Supports .NET 8.0 & 9.0


## How to use

For simple tests you can pass the logger into the class under test like this:

```csharp
public class MyTestClass
{
    private readonly IOutputTestHelper _output;
    
    public MyTestClass(IOutputTestHelper output)
    {
        _output = output;
    }
    
    [Fact]
    public void EnsureSomeFunctionalityWorks()
    {
        // Arrange
        var logger = new TestCaptureLogger<ServiceClass>();
        var service = new ServiceClass(logger);
        
        // Act
        service.DoSomething();
        
        // Assert
        _output.WriteLogs(logger); // Writes all captured logs to the output.
        logger.GetLogs().Count.ShouldBe(1);
    }
}
```

## See also

* [Full Documentation](https://stravaig-projects.github.io/Stravaig.Extensions.Logging.Diagnostics/docs/library/xunit-extensions.html)
* [Release Notes](https://github.com/Stravaig-Projects/Stravaig.Extensions.Logging.Diagnostics/releases)
