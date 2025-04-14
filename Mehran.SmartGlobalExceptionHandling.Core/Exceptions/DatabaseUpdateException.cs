namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای بروزرسانی پایگاه داده
/// </summary>
/// <param name="message"></param>
/// <param name="entityName"></param>
/// <param name="key"></param>
public class DatabaseUpdateException(
    string entityName = null,
    object key = null) : Exception("DbUpdate")
{
    /// <summary>
    /// نام موجودیت
    /// </summary>
    public string EntityName { get; } = entityName;

    /// <summary>
    /// کلید رکورد
    /// </summary>
    public object Key { get; } = key;
}