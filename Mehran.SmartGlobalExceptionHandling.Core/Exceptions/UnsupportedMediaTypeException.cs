namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 415 Unsupported Media Type
/// </summary>
public class UnsupportedMediaTypeException(object metaData = null)
    : Exception("UnsupportedMediaType")
{
    public object MetaData { get; } = metaData;
}
