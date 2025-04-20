namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 511 Network Authentication Required
/// </summary>
public class NetworkAuthenticationRequiredException(object metaData = null)
    : Exception("NetworkAuthenticationRequired")
{
    public object MetaData { get; } = metaData;
}
