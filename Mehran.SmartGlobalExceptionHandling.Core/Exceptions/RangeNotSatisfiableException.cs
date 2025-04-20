namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 416 Range Not Satisfiable
/// </summary>
public class RangeNotSatisfiableException(object metaData = null)
    : Exception("RangeNotSatisfiable")
{
    public object MetaData { get; } = metaData;
}
