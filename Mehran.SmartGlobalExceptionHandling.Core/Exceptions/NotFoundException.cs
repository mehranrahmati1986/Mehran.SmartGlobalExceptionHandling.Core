namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای عدم یافتن
/// </summary>
/// <param name="message"></param>
public class NotFoundException(object metaData = null) : Exception("NotFound")
{
    /// <summary>
    /// جزئیات خطا
    /// </summary>
    public string Detail { get; }

    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}


