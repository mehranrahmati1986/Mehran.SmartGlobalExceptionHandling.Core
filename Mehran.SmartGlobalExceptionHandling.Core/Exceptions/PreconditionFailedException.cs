namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 412 Precondition Failed
/// </summary>
public class PreconditionFailedException(object metaData = null)
    : Exception("PreconditionFailed")
{
    public object MetaData { get; } = metaData;
}
