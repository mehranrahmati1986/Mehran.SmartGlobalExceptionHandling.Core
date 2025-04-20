namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 422 Unprocessable Entity
/// </summary>
public class UnprocessableEntityException(object metaData = null)
    : Exception("UnprocessableEntity")
{
    public object MetaData { get; } = metaData;
}
