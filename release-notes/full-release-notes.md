# Full Release Notes

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

