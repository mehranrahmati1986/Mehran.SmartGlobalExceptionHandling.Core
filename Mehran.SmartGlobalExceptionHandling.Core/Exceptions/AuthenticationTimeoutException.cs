namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 419 Authentication Timeout (non-standard)
/// </summary>
public class AuthenticationTimeoutException(object metaData = null)
    : Exception("AuthenticationTimeout")
{
    public object MetaData { get; } = metaData;
}
