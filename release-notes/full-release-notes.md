# Full Release Notes

## Version 3.0.3

Date: Wednesday, 12 March, 2025 at 22:29:37 +00:00

### Dependencies

- All targets:
- .NET 9.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions
## Version 3.0.2

Date: Sunday, 23 February, 2025 at 22:45:56 +00:00

### Bug Fixes

- #190: Fix issue with timestamps being out of order.

### Dependencies

- All targets:
- .NET 8.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 8.0.3
- .NET 9.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 9.0.2
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
## Version 2.2.1

Date: Tuesday, 27 February, 2024 at 21:50:08 +00:00

### Bug Fixes

- #160: Improve the readability of Shouldly messages by adding a `ToString` override to the LogEntry class.

### Maintenance

- #162: Upgrade from deprecated GitHub Actions to new versions.

## Version 2.2.0

Date: Wednesday, 15 November, 2023 at 14:09:34 +00:00

### Miscellaneous

- #151: Added support for .NET 8.0

## Version 2.1.0

Date: Monday, 31 July, 2023 at 23:49:18 +01:00

### Features

- #138: Add Render Logs Extensions for XUnit.

### Miscellaneous

- #139: Create documentation at https://stravaig-projects.github.io/Stravaig.Extensions.Logging.Diagnostics/
## Version 2.0.1

Date: Tuesday, 13 June, 2023 at 22:19:18 +01:00

### Dependencies

- .NET 6.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 6.0.4
- .NET 7.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 7.0.1



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

## Version 1.4.1

Date: Tuesday, 6 December, 2022 at 09:43:58 +00:00

### Bugs

### Features

### Miscellaneous

### Dependencies

- All targets:
- .NET Standard 2.0 targets:
- .NET 5.0 targets:
- .NET 6.0 targets:



## Version 1.4.0

Date: Sunday, 13 November, 2022 at 14:36:18 +00:00

### Dependencies

- All targets:
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.31
- .NET 6.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 6.0.3
- .NET 7.0 targets:
  - Introduce Microsoft.Extensions.Logging.Abstractions at 7.0.0 


## Version 1.3.3

Date: Wednesday, 14 September, 2022 at 21:12:36 +01:00

### Dependencies

- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.29
- .NET 6.0 targets:
    - Bump Microsoft.Extensions.Logging.Abstractions to 6.0.2



## Version 1.3.2

Date: Tuesday, 9 August, 2022 at 21:28:46 +01:00

### Miscellaneous

- Add deterministic builds to further support [SourceLink](https://github.com/dotnet/sourcelink). See https://nuget.info/packages/Stravaig.Extensions.Logging.Diagnostics for an indicator that the build was deterministic.



## Version 1.3.1

Date: Saturday, 6 August, 2022 at 17:34:55 +01:00

### Miscellaneous

- Add [Source Link](https://github.com/dotnet/sourcelink) to allow easier debugging of the package in consuming applications. This will allow developers to step into the source code of this package when debugging their own code that uses this package.

### Dependencies

- .NET Core 3.1
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.27
## Version 1.3.0

Date: Monday, 23 May, 2022 at 13:29:25 +01:00

### Features

- #102: Add a reset function to the TestCaptureLoggerProvider to allow it to be reused.


## Version 1.2.3

Date: Wednesday, 11 May, 2022 at 14:11:47 +01:00
### Dependencies

- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.25


## Version 1.2.2

Date: Tuesday, 12 April, 2022 at 22:03:14 +01:00

### Dependencies

- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.24



## Version 1.2.1

Date: Wednesday, 9 March, 2022 at 22:43:47 +00:00

### Dependencies

- All targets:
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.23
- .NET 6.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions to 6.0.1



## Version 1.2.0

Date: Sunday, 6 February, 2022 at 16:33:20 +00:00

### Bugs

### Features

- #60: Add a log rendered.
- #82: Category Name is recorded in the log entry.
- #89: The log entry exposes the properties as an `IReadOnlyDictionary`

### Miscellaneous

- #74: Introduce support .NET 6.0

### Admin
- #83: Add pull request template for the repo

### Dependencies

- All targets:
  - Bump NUnit3TestAdapter from 4.2.0 to 4.2.1
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging to 3.1.22
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.22
- .NET 5.0 targets: No change
- .NET 6.0 targets:
  - Microsoft.Extensions.Logging to 6.0.0
  - Microsoft.Extensions.Logging.Abstractions to 6.0.0



## Version 1.1.4

Date: Saturday, 13 November, 2021 at 15:59:06 +00:00

### Miscellaneous

- #73: Add githooks to ensure that the issue number is attached to the commit message.

### Dependencies

- All targets:
  - Bump Microsoft.NET.Test.Sdk from 16.11.0 to 17.0.0
  - Bump NUnit3TestAdapter from 4.0.0 to 4.1.0
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging from 3.1.20 to 3.1.21
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.20 to 3.1.21
- .NET 5.0 targets:



## Version 1.1.3

Date: Tuesday, 12 October, 2021 at 20:40:53 +00:00

### Dependencies

- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging from 3.1.19 to 3.1.20
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.19 to 3.1.20

## Version 1.1.2

Date: Tuesday, 14 September, 2021 at 22:01:51 +00:00

### Dependencies

- All targets:
  - Bump Microsoft.NET.Test.Sdk from 16.10.0 to 16.11.0
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging from 3.1.18 to 3.1.19 
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.18 to 3.1.19

## Version 1.1.1

Date: Wednesday, 11 August, 2021 at 21:48:39 +00:00

### Dependencies

- All targets:
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging to 3.1.18
  - Bump Microsoft.Extensions.Logging.Abstractions to 3.1.18
- .NET 5.0 targets:

## Version 1.1.0

Date: Friday, 30 July, 2021 at 12:51:49 +00:00

### Features

- #52: TestCaptureLoggerProvider.GetAllLogEntries()
- #53: TestCaptureLogger(Provider).Get(All)LogsWithExceptions()
- #54: TestCaptureLoggerProvider.GetCategories()
- #56: TestCaptureLogger.GetLogs() (Logs property now marked as obsolete.)

## Version 1.0.1

Date: Wednesday, 14 July, 2021 at 11:45:22 +00:00

### Dependencies

- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging
  - Bump Microsoft.Extensions.Logging.Abstractions

## Version 1.0.0

Date: Saturday, 19 June, 2021 at 18:12:03 +00:00

### Bugs

- Bug #45: Ensure that the logger and provider are thread safe.

### Features

- #44: Add sequence number and datestamp to each log entry.

## Version 0.4.3

Date: Wednesday, 9 June, 2021 at 21:10:53 +00:00

### Dependencies

- All targets:
  - Bump Microsoft.NET.Test.Sdk from 16.9.4 to 16.10.0
  - Bump NUnit3TestAdapter from 3.17.0 to 4.0.0
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging from 3.1.15 to 3.1.16
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.15 to 3.1.16
- .NET 5.0 targets:

## Version 0.4.2

Date: Saturday, 15 May, 2021 at 12:04:44 +00:00

### Dependencies

- All targets:
  - Bump nunit from 3.13.1 to 3.13.2
- .NET Standard 2.0 targets:
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.14 to 3.1.15
  - Bump Microsoft.Extensions.Logging from 3.1.14 to 3.1.15

## Version 0.4.1

Date: Monday, 12 April, 2021 at 14:11:25 +00:00

### Dependencies

- All targets:
  - Bump Microsoft.NET.Test.Sdk from 16.9.1 to 16.9.4
- .NET Standard 2.0 target:
  - Bump Microsoft.Extensions.Logging from 3.1.13 to 3.1.14
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.13 to 3.1.14

## Version 0.4.0

Date: Saturday, 20 March, 2021 at 22:32:43 +00:00

### Miscellaneous

- #29 Dynamically define the end year in the copyright notice
- #30 Target .netstandard2.0 and .net 5.0.

### Dependabot

- netstandard2.0
  - Bump Microsoft.Extensions.Logging from 3.1.12 to 3.1.13
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.12 to 3.1.13
- .net 5.0
  - Bump Microsoft.Extensions.Logging from 3.1.12 to 5.0
  - Bump Microsoft.Extensions.Logging.Abstractions from 3.1.12 to 5.0
- General
  - Bump Microsoft.NET.Test.Sdk from 16.8.3 to 16.9.1

## Version 0.3.2

Date: Thursday, 11 February, 2021 at 20:54:19 +00:00

### Miscellaneous

- #24 : Update build process to mark releases using GitHub CLI, update the contributors script, and add a dependabot section to the wip-release notes when it is reset.

### Dependabot

- Bump Microsoft.Extensions.Logging from 3.1.11 to 3.1.12
- Bump Microsoft.Extensions.Logging.Abstractions from 3.1.11 to 3.1.12
- Bump nunit from 3.13.0 to 3.13.1

## Version 0.3.0

Date: Monday, 14 December, 2020 at 19:28:01 +00:00

### Features

* Issue #12 : Extract Original Message and Properties from the logger

### Miscellaneous

* Issue #13 : Update Build and Release process.

