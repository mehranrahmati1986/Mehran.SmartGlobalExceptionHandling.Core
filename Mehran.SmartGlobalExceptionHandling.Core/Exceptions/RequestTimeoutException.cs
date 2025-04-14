namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای پایان مهلت درخواست
/// </summary>
/// <param name="message"></param>
public class RequestTimeoutException(object metaData = null) : Exception("Timeout")
{
    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}
