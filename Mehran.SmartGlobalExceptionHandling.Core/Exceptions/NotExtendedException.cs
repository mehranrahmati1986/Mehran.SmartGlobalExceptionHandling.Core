namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 510 Not Extended
/// </summary>
public class NotExtendedException(object metaData = null)
    : Exception("NotExtended")
{
    public object MetaData { get; } = metaData;
}
