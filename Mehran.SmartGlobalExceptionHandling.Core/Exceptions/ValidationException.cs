using Mehran.SmartGlobalExceptionHandling.Core.Models;

namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای اعتبارسنجی
/// </summary>
/// <param name="errors"></param>
public class ValidationException(List<ValidationError> errors, object metaData = null) : Exception()
{
    /// <summary>
    /// لیست خطاها
    /// </summary>
    public List<ValidationError> Errors { get; } = errors;

    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}