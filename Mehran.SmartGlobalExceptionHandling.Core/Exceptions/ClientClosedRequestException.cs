namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 499 Client Closed Request (NGINX)
/// </summary>
public class ClientClosedRequestException(object metaData = null)
    : Exception("ClientClosedRequest")
{
    public object MetaData { get; } = metaData;
}
