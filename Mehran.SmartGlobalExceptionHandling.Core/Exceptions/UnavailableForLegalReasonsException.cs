namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 451 Unavailable For Legal Reasons
/// </summary>
public class UnavailableForLegalReasonsException(object metaData = null)
    : Exception("UnavailableForLegalReasons")
{
    public object MetaData { get; } = metaData;
}
