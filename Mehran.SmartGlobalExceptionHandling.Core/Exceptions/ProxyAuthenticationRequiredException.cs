namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 407 Proxy Authentication Required
/// </summary>
public class ProxyAuthenticationRequiredException(object metaData = null)
    : Exception("ProxyAuthenticationRequired")
{
    public object MetaData { get; } = metaData;
}
