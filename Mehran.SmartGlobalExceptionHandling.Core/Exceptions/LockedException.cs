namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 423 Locked
/// </summary>
public class LockedException(object metaData = null)
    : Exception("Locked")
{
    public object MetaData { get; } = metaData;
}
