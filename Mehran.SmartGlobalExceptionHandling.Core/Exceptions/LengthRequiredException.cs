namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 411 Length Required
/// </summary>
public class LengthRequiredException(object metaData = null)
    : Exception("LengthRequired")
{
    public object MetaData { get; } = metaData;
}
