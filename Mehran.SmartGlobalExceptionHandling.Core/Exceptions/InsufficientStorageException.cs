namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 507 Insufficient Storage
/// </summary>
public class InsufficientStorageException(object metaData = null)
    : Exception("InsufficientStorage")
{
    public object MetaData { get; } = metaData;
}
