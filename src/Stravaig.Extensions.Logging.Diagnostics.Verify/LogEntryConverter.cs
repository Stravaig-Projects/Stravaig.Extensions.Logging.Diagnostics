using System;
using System.Collections.Generic;
using System.Linq;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

internal class LogEntryConverter : WriteOnlyJsonConverter<IEnumerable<LogEntry>>
{
    private readonly LoggingCaptureVerifySettings _settings;

    public LogEntryConverter()
        : this(LoggingCaptureVerifySettings.Default)
    {
    }
    
    public LogEntryConverter(LoggingCaptureVerifySettings settings)
    {
        _settings = settings;
    }

    private class Context
    {
        private LoggingCaptureVerifySettings Settings { get; }
        internal VerifyJsonWriter Writer { get; }
        private int _sequence = -1;
        private int _cadenceOffset;
        private bool _isFirst = true;

        internal int CurrentSequence(LogEntry logEntry) =>
            Settings.Sequence == Sequence.ShowAsCadence
                ? logEntry.Sequence - _cadenceOffset
                : _sequence; 

        public Context(VerifyJsonWriter writer, LoggingCaptureVerifySettings settings)
        {
            Settings = settings;
            Writer = writer;
        }

        internal void MoveToNextLogEntry(LogEntry logEntry)
        {
            _sequence++;
            if (_isFirst)
            {
                _cadenceOffset = logEntry.Sequence;
                _isFirst = false;
            }
        }

        internal bool IsWritingSequence => Settings.Sequence is Sequence.ShowAsConsecutive or Sequence.ShowAsCadence;
    }
    
    public override void Write(VerifyJsonWriter writer, IEnumerable<LogEntry> logEntries)
    {
        var ctx = new Context(writer, _settings);
        writer.WriteStartArray();
        foreach (var logEntry in logEntries)
        {
            ctx.MoveToNextLogEntry(logEntry);
            writer.WriteStartObject();
            WriteSequence(ctx, logEntry);
            WriteLogLevel(ctx, logEntry);
            WriteCategoryName(ctx, logEntry);
            WriteMessage(ctx, logEntry);
            WriteProperties(ctx, logEntry);
            WriteException(ctx, logEntry);
            writer.WriteEndObject();
        }
        
        writer.WriteEndArray();
    }
    
    private static void WriteSequence(Context ctx, LogEntry logEntry)
    {
        if (ctx.IsWritingSequence)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.Sequence));
            ctx.Writer.WriteValue(ctx.CurrentSequence(logEntry));
        }
    }
    
    private void WriteLogLevel(Context ctx, LogEntry logEntry)
    {
        if (_settings.LogLevel)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.LogLevel));
            ctx.Writer.WriteValue(logEntry.LogLevel.ToString());
        }
    }
    
    private void WriteCategoryName(Context ctx, LogEntry logEntry)
    {
        if (_settings.CategoryName)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.CategoryName));
            ctx.Writer.WriteValue(logEntry.CategoryName);
        }
    }
    
    private void WriteMessage(Context ctx, LogEntry logEntry)
    {
        switch (_settings.Message)
        {
            case MessageSetting.Formatted:
                ctx.Writer.WritePropertyName("FormattedMessage");
                ctx.Writer.WriteValue(logEntry.FormattedMessage);
                break;
            case MessageSetting.Template:
                ctx.Writer.WritePropertyName("MessageTemplate");
                ctx.Writer.WriteValue(logEntry.OriginalMessage);
                break;
            case MessageSetting.None:
            default:
                break;
        }
    }

    private void WriteProperties(Context ctx, LogEntry logEntry)
    {
        if (_settings.Properties.HasFlag(PropertySetting.Verify) && logEntry.Properties.Count > 0)
        {
            ctx.Writer.WritePropertyName(nameof(_settings.Properties));
            ctx.Writer.WriteStartObject();

            bool redactNondeterministic = _settings.Properties.HasFlag(PropertySetting.RedactNonDeterministic);
            IEnumerable<KeyValuePair<string, object>> properties = logEntry.Properties;
            if (redactNondeterministic == false)
                properties = properties.Where(p => !_settings.IsNondeterministic(p.Key));
            properties = properties.OrderBy(p => p.Key);
            foreach (var property in properties)
            {
                ctx.Writer.WritePropertyName(property.Key);
                ctx.Writer.WriteValue(_settings.IsNondeterministic(property.Key)
                    ? _settings.NondeterministicPropertySubstitute
                    : property.Value);
            }
            
            ctx.Writer.WriteEndObject();
        }
    }

    private void WriteException(Context ctx, LogEntry logEntry)
    {
        var ex = logEntry.Exception;
        if (ex != null && _settings.Exception != ExceptionSetting.None)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.Exception));
            WriteException(ctx, ex);
        }
    }

    private void WriteException(Context ctx, Exception ex)
    {
        ctx.Writer.WriteStartObject();
        if (_settings.Exception.HasFlag(ExceptionSetting.Type))
        {
            ctx.Writer.WritePropertyName(nameof(Type));
            ctx.Writer.WriteValue(ex.GetType().FullName);
        }
        
        if (_settings.Exception.HasFlag(ExceptionSetting.Message))
        {
            ctx.Writer.WritePropertyName(nameof(ex.Message));
            ctx.Writer.WriteValue(ex.Message);
        }

        if (_settings.Exception.HasFlag(ExceptionSetting.StackTrace))
        {
            ctx.Writer.WritePropertyName(nameof(ex.StackTrace));
            ctx.Writer.WriteValue(ex.StackTrace);
        }
        
        if (ex.InnerException != null && _settings.Exception.HasFlag(ExceptionSetting.IncludeInnerExceptions))
        {
            ctx.Writer.WritePropertyName(nameof(ex.InnerException));
            WriteException(ctx, ex.InnerException);
        }

        ctx.Writer.WriteEndObject();
    }
}