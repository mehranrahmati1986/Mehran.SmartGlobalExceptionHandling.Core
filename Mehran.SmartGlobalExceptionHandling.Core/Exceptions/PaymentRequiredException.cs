namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 402 Payment Required
/// </summary>
public class PaymentRequiredException(object metaData = null) : Exception("PaymentRequired")
{
    public object MetaData { get; } = metaData;
}
