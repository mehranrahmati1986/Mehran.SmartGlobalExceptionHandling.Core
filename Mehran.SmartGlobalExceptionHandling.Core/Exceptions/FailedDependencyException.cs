namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 424 Failed Dependency
/// </summary>
public class FailedDependencyException(object metaData = null)
    : Exception("FailedDependency")
{
    public object MetaData { get; } = metaData;
}
