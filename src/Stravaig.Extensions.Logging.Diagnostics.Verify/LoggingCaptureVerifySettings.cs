using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Microsoft.Extensions.Logging;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public class LoggingCaptureVerifySettings
{
    private const string DefaultNonDeterministicPropertyValueSubstitute = "*** NONDETERMINISTIC ***";

    /// <summary>
    /// The default settings for verifying captured logs. This is a reasonable
    /// set of defaults that should not contain deterministic information.
    /// </summary>
    public static readonly LoggingCaptureVerifySettings Default = new();

    private readonly PropertySetting _properties = PropertySetting.None;
    private ImmutableHashSet<string> _nondeterministicPropertyNames = [];

    /// <summary>
    /// Gets how the Sequence number is to be verified.
    /// Default is ShowAsConsecutive.
    /// </summary>
    public Sequence Sequence { get; init; } = Sequence.ShowAsConsecutive;

    /// <summary>
    /// Gets the minimum log level to be verified.
    /// Default is to verify all (i.e. Trace and above.)
    /// </summary>
    public LogLevel MinLogLevel { get; init; } = LogLevel.Trace;

    /// <summary>
    /// Gets whether the category name is to be verified.
    /// Default is true.
    /// </summary>
    public bool CategoryName { get; init; } = true;

    /// <summary>
    /// Gets how the message is to be verified.
    /// Default is Template.
    /// </summary>
    public MessageSetting Message { get; init; } = MessageSetting.Template;

    /// <summary>
    /// Gets the parts of the exception to be verified if it exists.
    /// Default is Type.
    /// </summary>
    public ExceptionSetting Exception { get; init; } = ExceptionSetting.Type;

    /// <summary>
    /// Gets how the properties from the log are to be verified.
    /// Default is None.
    /// </summary>
    public PropertySetting Properties
    {
        get => _properties;
        init
        {
            _properties = value;
            BuildNondeterministicPropertySet(_nondeterministicPropertyNames);
        }
    }

    /// <summary>
    /// A list of property names that contain non-deterministic information.
    /// </summary>
    public IEnumerable<string> NondeterministicPropertyNames
    {
        get => _nondeterministicPropertyNames;
        init => BuildNondeterministicPropertySet(value);
    }
    
    /// <summary>
    /// Gets the value to replace nondeterministic properties with.
    /// </summary>
    public string NondeterministicPropertySubstitute { get; init; } = "*** Nondeterministic ***";
    
    private void BuildNondeterministicPropertySet(IEnumerable<string> values)
    {
        _nondeterministicPropertyNames = values.ToImmutableHashSet((Properties & PropertySetting.CaseInsensitivePropertyMatch) == PropertySetting.CaseInsensitivePropertyMatch ? StringComparer.OrdinalIgnoreCase : StringComparer.Ordinal);
    }

}