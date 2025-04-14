using Mehran.SmartGlobalExceptionHandling.Core.Models;

namespace Mehran.SmartGlobalExceptionHandling.Core.Logging;

/// <summary>
/// اینترفیس برای لاگ‌کردن خطاها
/// </summary>
public interface IExceptionLogger
{
    /// <summary>
    /// متد لاگ خطا
    /// </summary>
    /// <param name="exception">شیء اکسپشن</param>
    void Log(Exception exception);
}