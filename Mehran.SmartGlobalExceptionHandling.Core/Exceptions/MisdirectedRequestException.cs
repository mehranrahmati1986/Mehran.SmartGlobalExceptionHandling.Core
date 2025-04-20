namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 421 Misdirected Request
/// </summary>
public class MisdirectedRequestException(object metaData = null)
    : Exception("MisdirectedRequest")
{
    public object MetaData { get; } = metaData;
}
