namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 431 Request Header Fields Too Large
/// </summary>
public class RequestHeaderFieldsTooLargeException(object metaData = null)
    : Exception("RequestHeaderFieldsTooLarge")
{
    public object MetaData { get; } = metaData;
}
