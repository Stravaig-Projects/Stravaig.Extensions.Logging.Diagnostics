---
layout: default
title: Stravaig Logging Diagnostics Documentation Home
---

A logger for use in tests so that the messages logged can be examined in tests.

## Why test logs?

This was originally developed to test that background services were emitting logs in certain scenarios. Since the logs are the one of the primary views of how a background service is working it is essentially a first class output of the service. The user interface of the service, if you prefer, where the user is the developer or support technician attempting a diagnose an issue.

It can also be useful to test that structured logs emit the correct values so that values used when querying a logging sinks (such as ElasticSearch or New Relic) are available and correct.

### Other reasons to capture logs

This library also supports rendering the logs it captures, filtered in any way you see fit. This can be useful in scenarios, such as calling an ASP.NET application that will emit a deluge of logs that you are unlikely to be interested as it spins up the hosted server within the test.

# Usage

This package is designed to hook into the .NET logging framework so that the logger can be easily injected into units of code, or set up in the dependecy injection for larger integrated tests.

## Scenarios

* [Injecting a logger to the class under test](docs/scenarios/inject-logger-to-class-under-test.md)
* [Injecing a logger factory to the class under test](docs/scenarios/inject-logger-factory-to-class-under-test.md)
* [Using a WebApplicationFactory](docs/scenarios/using-a-web-application-factory.md)

## Library

* [LogEntry](docs/library/log-entry.md)
* [TestCaptureLogger](docs/library/test-capture-logger.md)
* [TestCaptureLoggerProvider](docs/library/test-capture-logger-provider.md)

#### Extensions

* [Rendering](docs/library/log-entry-renderer-extensions.md)
* [XUnit](docs/library/xunit-extensions.md)

# Project details

## Packages

* ![Nuget](https://img.shields.io/nuget/v/Stravaig.Extensions.Logging.Diagnostics?color=004880&label=nuget%20stable&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)
* ![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/Stravaig.Extensions.Logging.Diagnostics?color=ffffff&label=nuget%20latest&logo=nuget) [View on NuGet](https://www.nuget.org/packages/Stravaig.Extensions.Logging.Diagnostics)

## Supported .NET Versions

Current version supports: 6.0, 7.0

.NET Core 3.1 and .NET 5.0 support was dropped as of v2.0. Use v1.x when targeting .NET Core 3.1 or .NET 5.0.

## Other Stuff

* [Contributors](contributors.md)
* [Release Notes](release-notes/index.md)
