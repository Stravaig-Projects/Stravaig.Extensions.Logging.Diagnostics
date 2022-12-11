# Release Notes

## Version X

Date: ???

### Breaking Changes

- #115: Drop support for .NET Core 3.1 (LTS) and .NET 5.0 (STS)
- Remove obsolete code
  - `TestCaptureLogger`.`Logs` property has been removed. Use `GetLogs()` method instead.
  - `LogEntry`.ctor override without the `categoryName` parameter has been removed. A category name must be supplied.


