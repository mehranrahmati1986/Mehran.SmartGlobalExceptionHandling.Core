namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطاهای بیزینسی
/// </summary>
/// <param name="message"></param>
/// <param name="code"></param>
public class BusinessException(string code = null) : Exception("Business")
{
    /// <summary>
    /// کد خطا
    /// </summary>
    public string Code { get; } = code;
}
