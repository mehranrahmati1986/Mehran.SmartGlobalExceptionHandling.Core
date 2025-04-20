namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 506 Variant Also Negotiates
/// </summary>
public class VariantAlsoNegotiatesException(object metaData = null)
    : Exception("VariantAlsoNegotiates")
{
    public object MetaData { get; } = metaData;
}
