namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 417 Expectation Failed
/// </summary>
public class ExpectationFailedException(object metaData = null)
    : Exception("ExpectationFailed")
{
    public object MetaData { get; } = metaData;
}
