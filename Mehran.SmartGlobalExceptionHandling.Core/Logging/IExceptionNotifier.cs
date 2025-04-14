using Mehran.SmartGlobalExceptionHandling.Core.Models;

namespace Mehran.SmartGlobalExceptionHandling.Core.Logging;

/// <summary>
/// اینترفیس برای ارسال نوتیفیکیشن یا ایمیل در صورت وقوع خطا
/// </summary>
public interface IExceptionNotifier
{
    Task NotifyAsync(Exception exception, ErrorResponse response);
}