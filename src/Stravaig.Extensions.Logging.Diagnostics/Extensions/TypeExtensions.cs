using System;
using Stravaig.Extensions.Logging.Diagnostics.ExternalHelpers;

namespace Stravaig.Extensions.Logging.Diagnostics.Extensions;

internal static class TypeExtensions
{
    internal static string AsCategoryName(this Type type)
    {
        return TypeNameHelper.GetTypeDisplayName(type, includeGenericParameters: false, nestedTypeDelimiter: '.');
    }
}
