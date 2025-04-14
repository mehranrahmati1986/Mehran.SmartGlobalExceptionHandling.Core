namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای عدم احراز هویت
/// </summary>
/// <param name="message"></param>
public class UnauthorizedException(object metaData = null) : Exception("Unauthorized")
{
    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}
