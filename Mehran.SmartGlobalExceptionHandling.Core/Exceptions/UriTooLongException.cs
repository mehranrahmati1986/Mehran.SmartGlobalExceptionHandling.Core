namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 414 URI Too Long
/// </summary>
public class UriTooLongException(object metaData = null)
    : Exception("UriTooLong")
{
    public object MetaData { get; } = metaData;
}
