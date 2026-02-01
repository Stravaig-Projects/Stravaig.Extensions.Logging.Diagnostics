using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Microsoft.Extensions.Logging;

#if NET9_0_OR_GREATER
using System.Threading;
#endif

namespace Stravaig.Extensions.Logging.Diagnostics;

/// <summary>
/// A representation of an item that was logged.
/// </summary>
[DebuggerDisplay("{" + nameof(DebuggerDisplayString) + "}")]
public class LogEntry : IComparable<LogEntry>
{
    private const string OriginalMessagePropertyName = "{OriginalFormat}";

#if NET9_0_OR_GREATER
    private static readonly Lock SequenceSyncLock = new();
#else
    private static readonly object SequenceSyncLock = new();
#endif

    private static TimeProvider _timeProvider = TimeProvider.System;
    private static long _lastTimestampUtc;
    private static int _sequence;

    private IReadOnlyList<KeyValuePair<string, object?>>? _propertiesCache;
    private IReadOnlyDictionary<string, object?>? _propertyDictionaryCache;


    /// <summary>
    /// The <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> that the item was logged at.
    /// </summary>
    public LogLevel LogLevel { get; }

    /// <summary>
    /// An <see cref="T:Microsoft.Extensions.Logging.EventId"/> that identifies a logging event.
    /// </summary>
    public EventId EventId { get; }

    /// <summary>
    /// The entry to be written. Can be also an object.
    /// </summary>
    public object? State { get; }

    /// <summary>
    /// The <see cref="T:System.Exception"/> that was attached to the log.
    /// </summary>
    public Exception? Exception { get; }

    /// <summary>
    /// The formatted message.
    /// </summary>
    public string FormattedMessage { get; }

    /// <summary>
    /// The sequence number of the log message.
    /// </summary>
    /// <remarks>In a multithreaded environment there may be gaps between adjacent log messages.</remarks>
    public int Sequence { get; }

    /// <summary>
    /// The time the log entry was created in UTC.
    /// </summary>
    public DateTime TimestampUtc { get; }

    /// <summary>
    /// Ths time the log entry was created in the system's local time
    /// </summary>
    public DateTimeOffset TimestampLocal
    {
        get
        {
            TimeSpan offset = _timeProvider.LocalTimeZone.GetUtcOffset(TimestampUtc);
            long localTicks = TimestampUtc.Ticks + offset.Ticks;
            return new DateTimeOffset(localTicks, offset);
        }
    }

    public ImmutableArray<object?> ScopeStates { get; }

    /// <summary>
    /// The properties, if any, for the log entry.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, object?>> Properties
    {
        get
        {
            if (_propertiesCache is not null)
                return _propertiesCache;

            _propertiesCache = BuildProperties(State);
            return _propertiesCache;
        }
    }

    /// <summary>
    /// The properties, if any, for the log entry.
    /// </summary>
    public IReadOnlyDictionary<string, object?> PropertyDictionary
    {
        get
        {
            if (_propertyDictionaryCache is not null)
                return _propertyDictionaryCache;

            _propertyDictionaryCache = BuildDictionary(Properties);
            return _propertyDictionaryCache;
        }
    }

    /// <summary>
    /// The original message template, if available, for the log entry.
    /// </summary>
    public string? OriginalMessage =>
        (string?) Properties
            .FirstOrDefault(p => p.Key == OriginalMessagePropertyName)
            .Value;

    /// <summary>
    /// The category name of the log entry, if available.
    /// </summary>
    public string CategoryName { get; }

    /// <summary>
    /// Initialises a <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> that was logged.</param>
    /// <param name="eventId">An <see cref="T:Microsoft.Extensions.Logging.EventId"/> that identifies the logging event.</param>
    /// <param name="state">The entry that was written. Can be also an object.</param>
    /// <param name="exception">The <see cref="T:System.Exception"/> that was attached to the log.</param>
    /// <param name="formattedMessage">The formatted message.</param>
    /// <param name="categoryName">The source or category name.</param>
    public LogEntry(LogLevel logLevel, EventId eventId, object? state, Exception? exception, string formattedMessage, string categoryName)
        : this(logLevel, eventId, state, exception, formattedMessage, categoryName, [state])
    {
    }

    /// <summary>
    /// Initialises a <see cref="T:Stravaig.Extensions.Logging.Diagnostics.LogEntry"/>.
    /// </summary>
    /// <param name="logLevel">The <see cref="T:Microsoft.Extensions.Logging.LogLevel"/> that was logged.</param>
    /// <param name="eventId">An <see cref="T:Microsoft.Extensions.Logging.EventId"/> that identifies the logging event.</param>
    /// <param name="state">The entry that was written. Can be also an object.</param>
    /// <param name="exception">The <see cref="T:System.Exception"/> that was attached to the log.</param>
    /// <param name="formattedMessage">The formatted message.</param>
    /// <param name="categoryName">The source or category name.</param>
    /// <param name="scopeStates">The scopes that were active at the time the log was written.</param>
    public LogEntry(LogLevel logLevel, EventId eventId, object? state, Exception? exception, string formattedMessage, string categoryName, ImmutableArray<object?> scopeStates)
    {
        CategoryName = categoryName;
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
        FormattedMessage = formattedMessage;
        ScopeStates = scopeStates;
        lock (SequenceSyncLock)
        {
            Sequence = _sequence++;

            // Ensure monotonicity of the timestamp between log entries, even
            // in high-frequency logging scenarios.
            var timeNow = _timeProvider.GetUtcNow();
            var now = timeNow.Ticks;
            _lastTimestampUtc = Math.Max(_lastTimestampUtc + 1, now);
            TimestampUtc = new DateTime(_lastTimestampUtc, DateTimeKind.Utc);
        }
    }

    /// <inheritdoc />
    public int CompareTo(LogEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Sequence.CompareTo(other.Sequence);
    }

    /// <summary>
    /// Renders the log entry in a friendly way.
    /// </summary>
    /// <returns>A string representation of the log entry.</returns>
    public override string ToString()
    {
        StringBuilder sb = new();
        sb.Append(nameof(LogEntry));
        sb.Append(": [");
        sb.Append(LogLevel);
        sb.Append(' ');
        sb.Append(CategoryName);
        if (EventId.Id != 0)
        {
            sb.Append(" #");
            sb.Append(EventId.Id);
            if (!string.IsNullOrEmpty(EventId.Name))
            {
                sb.Append('/');
                sb.Append(EventId.Name);
            }
        }
        if (Exception != null)
        {
            sb.Append(" $");
            sb.Append(Exception.GetType().Name);
        }
        sb.Append("] ");
        sb.Append(FormattedMessage);
        return sb.ToString();
    }

    private static IReadOnlyList<KeyValuePair<string, object?>> BuildProperties(object? state)
    {
        if (state is null)
            return [];

        if (state is IEnumerable<KeyValuePair<string, object?>> kvps)
            return kvps.ToArray();

        var stateType = state.GetType();
        var key = stateType.FullName ?? stateType.Name;
        object? value;
        try
        {
            value = state.ToString();
        }
        catch (Exception ex)
        {
            value = ex.ToString();
        }

        return [ new KeyValuePair<string, object?>(key, value) ];
    }

    private static IReadOnlyDictionary<string, object?> BuildDictionary(IEnumerable<KeyValuePair<string, object?>> properties)
    {
        var dictionary = new Dictionary<string, object?>(StringComparer.Ordinal);
        foreach (var kvp in properties)
        {
            dictionary[kvp.Key] = kvp.Value;
        }

        return dictionary;
    }

    private string DebuggerDisplayString => $"[#{Sequence} @ {TimestampLocal:HH:mm:ss.fff zzz} {LogLevel} {CategoryName}] {FormattedMessage}";
}
