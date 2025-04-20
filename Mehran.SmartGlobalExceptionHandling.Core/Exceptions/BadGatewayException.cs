namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 502 Bad Gateway
/// </summary>
public class BadGatewayException(object metaData = null)
    : Exception("BadGateway")
{
    public object MetaData { get; } = metaData;
}
