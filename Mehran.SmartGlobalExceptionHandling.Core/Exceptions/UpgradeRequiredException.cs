namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 426 Upgrade Required
/// </summary>
public class UpgradeRequiredException(object metaData = null)
    : Exception("UpgradeRequired")
{
    public object MetaData { get; } = metaData;
}
