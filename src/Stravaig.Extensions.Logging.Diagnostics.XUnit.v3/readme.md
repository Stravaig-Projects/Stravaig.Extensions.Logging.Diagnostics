# About

XUnit v3 extensions for [Stravaig Log Capture](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics/) add helper extension methods for outputting the captured logs via XUnit v3's `ITestOutputHelper`.

## Version & Framework support

* v4.x: Supports .NET 8.0, 9.0 and 10.0 onwards (XUnit v3)

For XUnit v2 support, use the `Stravaig.Extensions.Logging.Diagnostics.XUnit` package.

## How to use

For simple tests you can pass the logger into the class under test like this:

```csharp
public class MyTestClass
{
    private readonly ITestOutputHelper _output;
    
    public MyTestClass(ITestOutputHelper output)
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
