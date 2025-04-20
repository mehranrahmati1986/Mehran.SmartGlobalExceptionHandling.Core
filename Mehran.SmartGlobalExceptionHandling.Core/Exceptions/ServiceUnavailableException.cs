namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 503 Service Unavailable
/// </summary>
public class ServiceUnavailableException(object metaData = null)
    : Exception("ServiceUnavailable")
{
    public object MetaData { get; } = metaData;
}
