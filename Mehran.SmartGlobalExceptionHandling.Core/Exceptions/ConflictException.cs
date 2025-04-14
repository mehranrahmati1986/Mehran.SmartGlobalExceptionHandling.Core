namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای تعارض داده‌ها
/// </summary>
/// <param name="message"></param>
public class ConflictException(object metaData = null) : Exception("Conflict") {

    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}