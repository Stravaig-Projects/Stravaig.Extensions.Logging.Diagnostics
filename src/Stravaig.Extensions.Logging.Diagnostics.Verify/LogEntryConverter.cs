using System.Collections.Generic;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

internal class LogEntryConverter : WriteOnlyJsonConverter<IEnumerable<LogEntry>>
{
    private readonly Settings _settings;

    public LogEntryConverter() : this(Settings.Default)
    {
        
    }
    
    public LogEntryConverter(Settings settings)
    {
        _settings = settings;
    }
    
    
    public override void Write(VerifyJsonWriter writer, IEnumerable<LogEntry> logEntries)
    {
        int sequence = 0;
        int cadenceOffset = 0;
        bool isFirst = true;
        writer.WriteStartArray();
        foreach (var logEntry in logEntries)
        {
            if (isFirst)
            {
                cadenceOffset = logEntry.Sequence;
                isFirst = false;
            }
            
            writer.WriteStartObject();

            if (_settings.WriteSequence())
            {
                writer.WritePropertyName(nameof(logEntry.Sequence));
                if (_settings.Use(Settings.KeepSequenceCadence))
                    writer.WriteValue(logEntry.Sequence - cadenceOffset);
                else
                    writer.WriteValue(sequence);
            }

            if (_settings.WriteLogLevel())
            {
                writer.WritePropertyName(nameof(logEntry.LogLevel));
                writer.WriteValue(logEntry.LogLevel.ToString());
            }

            if (_settings.WriteCategoryName())
            {
                writer.WritePropertyName(nameof(logEntry.CategoryName));
                writer.WriteValue(logEntry.CategoryName);
            }

            if (_settings.WriteFormattedMessage())
            {
                writer.WritePropertyName(nameof(logEntry.FormattedMessage));
                writer.WriteValue(logEntry.FormattedMessage);
            }

            writer.WriteEndObject();
            sequence++;
        }
        
        writer.WriteEndArray();
    }
}