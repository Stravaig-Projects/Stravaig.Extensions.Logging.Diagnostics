# Release Notes

## Version X

Date: ???

### Breaking Changes

- `TestCaptureLogger`.`Logs` property has been removed. Use `GetLogs()` method instead.
- `LogEntry`.ctor override without the `categoryName` parameter has been removed. A category name must be supplied.

### Miscellaneous

- #115: Drop support for .NET Core 3.1 (LTS) and .NET 5.0 (STS)

