namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 406 Not Acceptable
/// </summary>
public class NotAcceptableException(object metaData = null)
    : Exception("NotAcceptable")
{
    public object MetaData { get; } = metaData;
}
