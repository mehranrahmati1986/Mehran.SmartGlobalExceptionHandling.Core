namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای تعداد زیاد درخواست
/// </summary>
/// <param name="message"></param>
public class TooManyRequestsException(object metaData = null) : Exception("TooManyRequests")
{
    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}
