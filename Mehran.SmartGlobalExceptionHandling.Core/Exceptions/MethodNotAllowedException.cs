namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 405 Method Not Allowed
/// </summary>
public class MethodNotAllowedException(object metaData = null)
    : Exception("MethodNotAllowed")
{
    public object MetaData { get; } = metaData;
}
