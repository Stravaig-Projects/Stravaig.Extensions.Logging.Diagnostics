﻿using System;
using Argon;
using VerifyTests;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

public static class VerifyStravaigLoggingCapture
{
    public static bool Initialised { get; private set; }

    public static void Initialise()
    {
        ThrowIfInitialised();
        
        InnerVerifier.ThrowIfVerifyHasBeenRun();
        
        Initialised = true;

        VerifierSettings.AddExtraSettings(LogEntrySettings);
    }

    private static void LogEntrySettings(JsonSerializerSettings settings)
    {
        var defaultLogEntryConverter = new LogEntryConverter();
        settings.Converters.Add(defaultLogEntryConverter);
    }
    
    private static void ThrowIfInitialised()
    {
        if (Initialised)
            throw new InvalidOperationException("Stravaig.Extensions.Logging.Diagnostics.Verify has been initialised.");
    }
}