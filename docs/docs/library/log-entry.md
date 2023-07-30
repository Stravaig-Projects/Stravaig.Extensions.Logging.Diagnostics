---
layout: default
title: LogEntry class
---

# LogEntry class

The `LogEntry` class represents a single entry in the log.

## Properties

* `LogLevel` which is a `Microsoft.Extensions.Logging.LogLevel` enum.
* `EventId` is a `Microsoft.Extensions.Logging.EventId` struct.
* `State` is any additional state on attached to the log.
* `Exception` contains any exception that was passed into the logger.
* `FormattedMessage` is the formatted message with the placeholders filled in.
* `OriginalMessage` is the original message template without the placeholder values filled in.
* `Sequence` is the sequence number of the log entry. Note: In a multi-threaded test the sequence number may not necessarily be deterministic as each thread may reach its logging call at different times.
* `TimestampUtc` is the timestamp of the log entry in UTC.
* `TimestampLocal` is the timestamp of the log entry in the local time of the machine.
* `Properties` is an `IReadOnlyList<KeyValuePair<string, object>>` derived from the `State`.
* `PropertyDictionary` is an `IReadOnlyDictionary<string, object>` derived from the `State`.
* `CategoryName` is the category name of th log entry. When the log entry is created from an `TestCaptureLogger<T>` the name will be derived from the type.