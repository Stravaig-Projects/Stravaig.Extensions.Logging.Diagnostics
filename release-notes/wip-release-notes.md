# Release Notes

## Version X

Date: ???

### Breaking Changes

- Feature #123 changes the `TestCaptureLogger<T>` class to encapsulate an instance of `TestCaptureLogger` rather than inherit from it. If you're code relied on `TestCaptureLogger<T>` inheriting from `TestCaptureLogger` then it will likely break.
- Feature #172 drops support for .NET 7.0. Use .NET 6.0 LTS or .NET 8.0 LTS.

### Bugs

### Features

- #123: `TestCaptureLoggerProvider.CreateLogger<T>()`
  - Potential breaking change: `TestCaptureLogger<T>` no longer inherits from `TestCaptureLogger`.
  - Add `ITestCaptureLogger` and have `TestCaptureLogger` and `TestCaptureLogger<T>` be concrete implementations of the interface so you can reference the interface and not care which concrete implementation you have.
- #170: Additional xunit extension methods to write out all log messages to the `ITestOutputHelper`.
  - `ITestOutputHelper.WriteLogs(ITestCaptureLogger...)`
  - `ITestOutputHelper.WriteLogs(TestCaptureLoggerProvider...)`

### Miscellaneous

- #164: Update pipeline.
- #172: Drop support and package targeting for .NET 7.0.

### Dependencies

- #166 Update package references:
  - .NET 8.0 targets:
    - Bump Microsoft.Extensions.Logging.Abstractions to 8.0.1
  - .NET 7.0 targets:
    - Dropped
