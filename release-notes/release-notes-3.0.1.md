# Release Notes

## Version 3.0.1

Date: Sunday, 1 December, 2024 at 22:12:02 +00:00

### Breaking Changes

- Feature #123 changes the `TestCaptureLogger<T>` class to encapsulate an instance of `TestCaptureLogger` rather than inherit from it. If your code relied on `TestCaptureLogger<T>` inheriting from `TestCaptureLogger` then it will likely break, however an implicit and explicit cast operators were added to mitigate this issue.
- Feature #172 drops support for .NET 7.0. Use .NET 6.0 LTS, .NET 8.0 LTS or .NET 9.0 STS.

### Features

- #123: `TestCaptureLoggerProvider.CreateLogger<T>()`
  - Potential breaking change: `TestCaptureLogger<T>` no longer inherits from `TestCaptureLogger`.
  - Add `ITestCaptureLogger` and have `TestCaptureLogger` and `TestCaptureLogger<T>` be concrete implementations of the interface so you can reference the interface and not care which concrete implementation you have.
- #170: Additional xunit extension methods to write out all log messages to the `ITestOutputHelper`.
  - `ITestOutputHelper.WriteLogs(ITestCaptureLogger...)`
  - `ITestOutputHelper.WriteLogs(TestCaptureLoggerProvider...)`
- #171: Add `GetLogs(predicate)` to `TestCaptureLogger` and `TestCaptureLoggerProvider`.
- #184: Add casting between `TestCaptureLogger` and `TestCaptureLogger<T>` to compensate for `TestCaptureLogger<T>` no longer inheriting from `TestCaptureLogger`
  - `TestCaptureLogger<T>` may be implicitly or explicitly cast to `TestCaptureLogger`
  - `TestCaptureLogger` must be explicitly cast to `TestCaptureLogger<T>`. The cast may fail if the category name used by the `TestCaptureLogger` does not match the type name of `T`.

### Miscellaneous

- #36: Add package readme.
- #164: Update pipeline.
- #172: Drop support and package targeting for .NET 7.0.
- #179: Update github pages pipeline.
- #181: Add support for .NET 9.0.
- #183: Update github action workflow to separate the build and deploy stages.
- #185: Add package readme to the XUnit extensions package

### Dependencies

- #166, #172 & #181 Update package references:
  - .NET 9.0 targets:
    - Added
  - .NET 8.0 targets:
    - Bump Microsoft.Extensions.Logging.Abstractions to 8.0.2
  - .NET 7.0 targets:
    - Dropped
