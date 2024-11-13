#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace Stravaig.Extensions.Logging.Diagnostics;

public class LogProperty
{
    private readonly object? _value;

    public string Name { get; }

    public bool Exists { get; }

    public LogProperty(string name, object? value)
    {
        _value = value;
        Exists = true;
        Name = name;
    }

    public LogProperty(string name)
    {
        _value = null;
        Exists = false;
        Name = name;
    }


    public bool IsOfType<T>() => _value is T;

    public bool HasValue => _value != null;

    public object? Value
    {
        get
        {
            if (Exists)
                return _value;
            throw new LogPropertyException($"Cannot get the value for the log property '{Name}' as it does not exist.");
        }
    }

    public T? GetValue<T>()
    {
        return (T?)Value;
    }
}
