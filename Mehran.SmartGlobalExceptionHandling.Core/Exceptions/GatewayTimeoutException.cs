namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 504 Gateway Timeout
/// </summary>
public class GatewayTimeoutException(object metaData = null)
    : Exception("GatewayTimeout")
{
    public object MetaData { get; } = metaData;
}
