namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 410 Gone
/// </summary>
public class GoneException(object metaData = null)
    : Exception("Gone")
{
    public object MetaData { get; } = metaData;
}
