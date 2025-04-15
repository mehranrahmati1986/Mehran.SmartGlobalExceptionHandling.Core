using Mehran.SmartGlobalExceptionHandling.Core.Enums;
using Mehran.SmartGlobalExceptionHandling.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;

// میدلویر برای تنظیم خودکار زبان براساس درخواست
public class RequestLocalizationMiddleware(IOptions<ExceptionHandlingOption> options)
{
    // زبان‌های پشتیبانی‌شده
    private static readonly List<string> _supportedCultures = ["fa", "en", "ar"];
    private readonly SupportedLanguage _userLanguage = options.Value.Language;

    public async Task Invoke(HttpContext context, RequestDelegate next)
    {
        // اگر کاربر زبان خاصی رو انتخاب نکرده باشد، زبان پیش‌فرض استفاده می‌شود.
        var lang = _userLanguage.ToString().ToLower();

        if (!_supportedCultures.Contains(lang))
        {
            lang = SupportedLanguage.Fa.ToString().ToLower(); // پیش‌فرض فارسی
        }

        var cultureInfo = new CultureInfo(lang);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await next(context);
    }
}


