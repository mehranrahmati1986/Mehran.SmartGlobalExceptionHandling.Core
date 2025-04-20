namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 428 Precondition Required
/// </summary>
public class PreconditionRequiredException(object metaData = null)
    : Exception("PreconditionRequired")
{
    public object MetaData { get; } = metaData;
}
