namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 508 Loop Detected
/// </summary>
public class LoopDetectedException(object metaData = null)
    : Exception("LoopDetected")
{
    public object MetaData { get; } = metaData;
}
