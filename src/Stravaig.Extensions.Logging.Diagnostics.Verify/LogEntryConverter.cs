using System;
using System.Collections.Generic;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

internal class LogEntryConverter : WriteOnlyJsonConverter<IEnumerable<LogEntry>>
{
    private readonly Settings _settings;

    public LogEntryConverter()
        : this(Settings.Default)
    {
        
    }
    
    public LogEntryConverter(Settings settings)
    {
        _settings = settings;
    }

    private class Context
    {
        internal Settings Settings { get; }
        internal VerifyJsonWriter Writer { get; }
        private int _sequence;
        internal int CadenceOffset { get; set; }
        
        internal bool IsWritingSequence => Using(Settings.Sequence);
        internal bool IsWritingLogLevel => Using(Settings.LogLevel);
        internal bool IsWritingCategoryName => Using(Settings.CategoryName);
        internal bool IsWritingFormattedMessage => Using(Settings.FormattedMessage);
        internal bool IsWritingException => Using(Settings.Exception);
        internal bool IsWritingExceptionMessage => Using(Settings.ExceptionMessage);
        internal bool IsWritingExceptionType => Using(Settings.ExceptionType);
        internal bool IsWritingInnerException => Using(Settings.InnerException);
        internal bool IsWritingStackTrace => Using(Settings.StackTrace);

        internal int CurrentSequence(LogEntry logEntry) =>
            Using(Settings.KeepSequenceCadence)
                ? logEntry.Sequence - CadenceOffset
                : _sequence; 

        public Context(VerifyJsonWriter writer, Settings settings)
        {
            _sequence = 0;
            Settings = settings;
            Writer = writer;
        }

        internal void MoveToNextLogEntry()
        {
            _sequence++;
        }
        
        private bool Using(Settings setting) => (Settings & setting) != 0;
        
    }
    
    public override void Write(VerifyJsonWriter writer, IEnumerable<LogEntry> logEntries)
    {
        var ctx = new Context(writer, _settings);
        bool isFirst = true;
        writer.WriteStartArray();
        foreach (var logEntry in logEntries)
        {
            if (isFirst)
            {
                ctx.CadenceOffset = logEntry.Sequence;
                isFirst = false;
            }
            
            writer.WriteStartObject();

            WriteSequence(ctx, logEntry);
            WriteLogLevel(ctx, logEntry);
            WriteCategoryName(ctx, logEntry);
            WriteFormattedMessage(ctx, logEntry);
            WriteException(ctx, logEntry);

            writer.WriteEndObject();
            ctx.MoveToNextLogEntry();
        }
        
        writer.WriteEndArray();
    }

    private static void WriteException(Context ctx, LogEntry logEntry)
    {
        var ex = logEntry.Exception;
        if (ex != null && ctx.IsWritingException)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.Exception));
            WriteException(ctx, ex);
        }
    }

    private static void WriteException(Context ctx, Exception ex)
    {
        ctx.Writer.WriteStartObject();
        if (ctx.IsWritingExceptionMessage)
        {
            ctx.Writer.WritePropertyName(nameof(ex.Message));
            ctx.Writer.WriteValue(ex.Message);
        }

        if (ctx.IsWritingExceptionType)
        {
            ctx.Writer.WritePropertyName(nameof(Type));
            ctx.Writer.WriteValue(ex.GetType().FullName);
        }

        if (ctx.IsWritingStackTrace)
        {
            ctx.Writer.WritePropertyName(nameof(ex.StackTrace));
            ctx.Writer.WriteValue(ex.StackTrace);
        }
        
        if (ex.InnerException != null && ctx.IsWritingInnerException)
        {
            ctx.Writer.WritePropertyName(nameof(ex.InnerException));
            WriteException(ctx, ex.InnerException);
        }

        ctx.Writer.WriteEndObject();
    }

    private static void WriteFormattedMessage(Context ctx, LogEntry logEntry)
    {
        if (ctx.IsWritingFormattedMessage)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.FormattedMessage));
            ctx.Writer.WriteValue(logEntry.FormattedMessage);
        }
    }

    private static void WriteCategoryName(Context ctx, LogEntry logEntry)
    {
        if (ctx.IsWritingCategoryName)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.CategoryName));
            ctx.Writer.WriteValue(logEntry.CategoryName);
        }
    }

    private static void WriteLogLevel(Context ctx, LogEntry logEntry)
    {
        if (ctx.IsWritingLogLevel)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.LogLevel));
            ctx.Writer.WriteValue(logEntry.LogLevel.ToString());
        }
    }

    private static void WriteSequence(Context ctx, LogEntry logEntry)
    {
        if (ctx.IsWritingSequence)
        {
            ctx.Writer.WritePropertyName(nameof(logEntry.Sequence));
            ctx.Writer.WriteValue(ctx.CurrentSequence(logEntry));
        }
    }
}