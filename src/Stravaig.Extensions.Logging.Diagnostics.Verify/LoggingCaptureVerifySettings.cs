using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using VerifyTests;

#if NET8_0_OR_GREATER
using System.Collections.Frozen;
#else
using System.Collections.Immutable;
#endif

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

/// <summary>
/// The settings for verifying logs
/// </summary>
public class LoggingCaptureVerifySettings
{
    /// <summary>
    /// The default settings for verifying captured logs. This is a reasonable
    /// set of defaults that should not contain deterministic information.
    /// </summary>
    public static readonly LoggingCaptureVerifySettings Default = new();

    private readonly PropertySetting _properties = PropertySetting.None;
    
#if NET8_0_OR_GREATER
    private FrozenSet<string> _nondeterministicPropertyNames = FrozenSet<string>.Empty;
#else
    private ImmutableHashSet<string> _nondeterministicPropertyNames = [];
#endif
    /// <summary>
    /// Gets how the Sequence number is to be verified.
    /// Default is ShowAsConsecutive.
    /// </summary>
    public Sequence Sequence { get; init; } = Sequence.ShowAsConsecutive;

    /// <summary>
    /// Gets whether the log leve is to be verified.
    /// Default is true.
    /// </summary>
    public bool LogLevel { get; init; } = true;

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
    public ExceptionSetting Exception { get; init; } = ExceptionSetting.Type | ExceptionSetting.IncludeInnerExceptions;

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
    /// Default is an empty set.
    /// </summary>
    public IEnumerable<string> NondeterministicPropertyNames
    {
        get => _nondeterministicPropertyNames;
        init => BuildNondeterministicPropertySet(value);
    }

    /// <summary>
    /// Gets whether the given named property has a nondeterministic value.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A Boolean, true if nondeterministic, false if deterministic.</returns>
    public bool IsNondeterministic(string propertyName) => _nondeterministicPropertyNames.Contains(propertyName);
    
    /// <summary>
    /// Gets the value to replace nondeterministic properties with.
    /// Default is "*** Nondeterministic ***".
    /// </summary>
    public string NondeterministicPropertySubstitute { get; init; } = "*** Nondeterministic ***";
    
    private void BuildNondeterministicPropertySet(IEnumerable<string> values)
    {
        var stringComparer = Properties.HasFlag(PropertySetting.CaseInsensitivePropertyMatch)
            ? StringComparer.OrdinalIgnoreCase
            : StringComparer.Ordinal;
#if NET8_0_OR_GREATER
        _nondeterministicPropertyNames = values.ToFrozenSet(stringComparer);
#else
        _nondeterministicPropertyNames = values.ToImmutableHashSet(stringComparer);
#endif
    }
}