namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 413 Payload Too Large
/// </summary>
public class PayloadTooLargeException(object metaData = null)
    : Exception("PayloadTooLarge")
{
    public object MetaData { get; } = metaData;
}
