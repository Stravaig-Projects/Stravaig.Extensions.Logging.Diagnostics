# Release Notes

## Version 2.0.0

Date: Saturday, 11 February, 2023 at 23:00:02 +00:00

### Breaking Changes

- #115: Drop support for .NET Core 3.1 (LTS) and .NET 5.0 (STS)
- Remove obsolete code
  - `TestCaptureLogger`.`Logs` property has been removed. Use `GetLogs()` method instead.
  - `LogEntry`.ctor override without the `categoryName` parameter has been removed. A category name must be supplied.

### Miscellaneous

- #117: Enable nullable types in the project.
- #118: Update TypeNameHelper.

