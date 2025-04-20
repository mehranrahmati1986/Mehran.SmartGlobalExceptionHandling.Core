namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 501 Not Implemented
/// </summary>
public class NotImplementedHttpException(object metaData = null)
    : Exception("NotImplemented")
{
    public object MetaData { get; } = metaData;
}
