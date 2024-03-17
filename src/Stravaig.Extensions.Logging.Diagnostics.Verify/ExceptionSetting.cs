using System;

namespace Stravaig.Extensions.Logging.Diagnostics.Verify;

[Flags]
public enum ExceptionSetting
{
    None = 0x0,
    
    Type = 0x1,
    
    Message = 0x2,
    
    StackTrace = 0x4,
    
    IncludeInnerExceptions = 0x8,
}