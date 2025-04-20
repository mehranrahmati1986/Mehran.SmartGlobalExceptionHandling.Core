namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 505 HTTP Version Not Supported
/// </summary>
public class HttpVersionNotSupportedException(object metaData = null)
    : Exception("HttpVersionNotSupported")
{
    public object MetaData { get; } = metaData;
}
