using System;
using System.Collections.Generic;
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
    private static long _lastTimestampUtc;
    private static int _sequence;
#if NET9_0_OR_GREATER
    private static readonly Lock SequenceSyncLock = new();
#else
    private static readonly object SequenceSyncLock = new();
#endif

    private const string OriginalMessagePropertyName = "{OriginalFormat}";

    private readonly IReadOnlyList<object?> _scopeStates;
    private readonly Lazy<IReadOnlyList<object?>> _lazyScopes;
    private readonly Lazy<IReadOnlyList<KeyValuePair<string, object?>>> _lazyProperties;
    private readonly Lazy<IReadOnlyDictionary<string, object?>> _lazyPropertyDictionary;
    private IReadOnlyList<KeyValuePair<string, object?>>?[]? _scopePropertiesCache;
    private IReadOnlyDictionary<string, object?>?[]? _scopePropertyDictionaryCache;
    private IReadOnlyList<KeyValuePair<string, object?>>? _flattenedScopeProperties;
    private IReadOnlyDictionary<string, object?>? _flattenedScopePropertyDictionary;

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
            TimeSpan offset = TimeZoneInfo.Local.GetUtcOffset(TimestampUtc);
            long localTicks = TimestampUtc.Ticks + offset.Ticks;
            return new DateTimeOffset(localTicks, offset);
        }
    }

    /// <summary>
    /// The properties, if any, for the log entry.
    /// </summary>
    public IReadOnlyList<KeyValuePair<string, object?>> Properties => _lazyProperties.Value;

    /// <summary>
    /// The properties, if any, for the log entry.
    /// </summary>
    public IReadOnlyDictionary<string, object?> PropertyDictionary => _lazyPropertyDictionary.Value;

    /// <summary>
    /// The scopes captured for this log entry, outer most to inner most.
    /// </summary>
    public IReadOnlyList<object?> Scopes => _lazyScopes.Value;

    /// <summary>
    /// The number of scope levels captured for this log entry.
    /// </summary>
    public int ScopeLevels => _scopeStates.Count;

    /// <summary>
    /// The original message template, if available, for the log entry.
    /// </summary>
    public string OriginalMessage =>
        (string) Properties
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
    {
        CategoryName = categoryName;
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
        FormattedMessage = formattedMessage;
        _scopeStates = CreateDefaultScopeStates(state);
        _lazyScopes = new Lazy<IReadOnlyList<object?>>(() => _scopeStates.ToArray());
        _lazyProperties = new Lazy<IReadOnlyList<KeyValuePair<string, object?>>>(() => BuildProperties(State));
        _lazyPropertyDictionary = new Lazy<IReadOnlyDictionary<string, object?>>(() => BuildDictionary(Properties));
        lock (SequenceSyncLock)
        {
            Sequence = _sequence++;

            // Ensure monotonicity of the timestamp between log entries, even
            // in high-frequency logging scenarios.
            var now = DateTime.UtcNow.Ticks;
            _lastTimestampUtc = Math.Max(_lastTimestampUtc + 1, now);
            TimestampUtc = new DateTime(_lastTimestampUtc, DateTimeKind.Utc);
        }
    }

    internal LogEntry(LogLevel logLevel, EventId eventId, object? state, Exception? exception, string formattedMessage, string categoryName, IReadOnlyList<object?> scopeStates)
    {
        ArgumentNullException.ThrowIfNull(scopeStates);
        CategoryName = categoryName;
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
        FormattedMessage = formattedMessage;
        _scopeStates = scopeStates;
        _lazyScopes = new Lazy<IReadOnlyList<object?>>(() => _scopeStates.ToArray());
        _lazyProperties = new Lazy<IReadOnlyList<KeyValuePair<string, object?>>>(() => BuildProperties(State));
        _lazyPropertyDictionary = new Lazy<IReadOnlyDictionary<string, object?>>(() => BuildDictionary(Properties));
        lock (SequenceSyncLock)
        {
            Sequence = _sequence++;

            // Ensure monotonicity of the timestamp between log entries, even
            // in high-frequency logging scenarios.
            var now = DateTime.UtcNow.Ticks;
            _lastTimestampUtc = Math.Max(_lastTimestampUtc + 1, now);
            TimestampUtc = new DateTime(_lastTimestampUtc, DateTimeKind.Utc);
        }
    }

    internal LogEntry(LogLevel logLevel, EventId eventId, object? state, Exception? exception, string formattedMessage, string categoryName, int sequence, DateTime timestampUtc)
    {
        LogLevel = logLevel;
        EventId = eventId;
        State = state;
        Exception = exception;
        FormattedMessage = formattedMessage;
        Sequence = sequence;
        TimestampUtc = timestampUtc;
        CategoryName = categoryName;
        _scopeStates = CreateDefaultScopeStates(state);
        _lazyScopes = new Lazy<IReadOnlyList<object?>>(() => _scopeStates.ToArray());
        _lazyProperties = new Lazy<IReadOnlyList<KeyValuePair<string, object?>>>(() => BuildProperties(State));
        _lazyPropertyDictionary = new Lazy<IReadOnlyDictionary<string, object?>>(() => BuildDictionary(Properties));
    }

    /// <inheritdoc />
    public int CompareTo(LogEntry? other)
    {
        if (ReferenceEquals(this, other)) return 0;
        if (ReferenceEquals(null, other)) return 1;
        return Sequence.CompareTo(other.Sequence);
    }

    /// <summary>
    /// Gets the scope properties for a given scope level.
    /// </summary>
    /// <param name="level">The scope level, zero based from outer most to inner most.</param>
    /// <returns>A read only list of scope properties.</returns>
    public IReadOnlyList<KeyValuePair<string, object?>> GetScopeProperties(int level)
    {
        if (level < 0 || level >= _scopeStates.Count)
            throw new ArgumentOutOfRangeException(nameof(level));

        _scopePropertiesCache ??= new IReadOnlyList<KeyValuePair<string, object?>>?[_scopeStates.Count];
        var cached = _scopePropertiesCache[level];
        if (cached != null)
            return cached;

        IReadOnlyList<KeyValuePair<string, object?>> properties = level == _scopeStates.Count - 1
            ? _lazyProperties.Value
            : BuildProperties(_scopeStates[level]);

        _scopePropertiesCache[level] = properties;
        return properties;
    }

    /// <summary>
    /// Gets a scope property dictionary for a given scope level.
    /// </summary>
    /// <param name="level">The scope level, zero based from outer most to inner most.</param>
    /// <returns>A read only dictionary of scope properties.</returns>
    public IReadOnlyDictionary<string, object?> GetScopePropertyDictionary(int level)
    {
        if (level < 0 || level >= _scopeStates.Count)
            throw new ArgumentOutOfRangeException(nameof(level));

        _scopePropertyDictionaryCache ??= new IReadOnlyDictionary<string, object?>?[_scopeStates.Count];
        var cached = _scopePropertyDictionaryCache[level];
        if (cached != null)
            return cached;

        var dictionary = BuildDictionary(GetScopeProperties(level));
        _scopePropertyDictionaryCache[level] = dictionary;
        return dictionary;
    }

    /// <summary>
    /// Gets the flattened scope properties across all scope levels.
    /// </summary>
    /// <returns>A read only list of scope properties.</returns>
    public IReadOnlyList<KeyValuePair<string, object?>> GetFlattenedScopeProperties()
    {
        if (_flattenedScopeProperties != null)
            return _flattenedScopeProperties;

        var flattened = new List<KeyValuePair<string, object?>>();
        for (int i = 0; i < _scopeStates.Count; i++)
        {
            flattened.AddRange(GetScopeProperties(i));
        }

        _flattenedScopeProperties = flattened.ToArray();
        return _flattenedScopeProperties;
    }

    /// <summary>
    /// Gets the flattened scope properties as a dictionary.
    /// </summary>
    /// <returns>A read only dictionary of scope properties.</returns>
    public IReadOnlyDictionary<string, object?> GetFlattenedScopePropertyDictionary()
    {
        if (_flattenedScopePropertyDictionary != null)
            return _flattenedScopePropertyDictionary;

        _flattenedScopePropertyDictionary = BuildDictionary(GetFlattenedScopeProperties());
        return _flattenedScopePropertyDictionary;
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

    private string DebuggerDisplayString => $"[#{Sequence} @ {TimestampLocal:HH:mm:ss.fff zzz} {LogLevel} {CategoryName}] {FormattedMessage}";

    private static IReadOnlyList<object?> CreateDefaultScopeStates(object? state)
        => new object?[] { state };

    private static IReadOnlyList<KeyValuePair<string, object?>> BuildProperties(object? state)
    {
        if (state is null)
            return Array.Empty<KeyValuePair<string, object?>>();

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

        return new[] { new KeyValuePair<string, object?>(key, value) };
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
}
