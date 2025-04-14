namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای دسترسی غیرمجاز
/// </summary>
/// <param name="message"></param>
public class ForbiddenException(object metaData = null) : Exception("Forbidden")
{
    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}
