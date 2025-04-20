namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// HTTP 418 I'm a teapot
/// </summary>
public class ImATeapotException(object metaData = null)
    : Exception("ImATeapot")
{
    public object MetaData { get; } = metaData;
}
