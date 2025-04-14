using Microsoft.AspNetCore.Http;
using System.Globalization;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;

// میدلویر برای تنظیم خودکار زبان براساس درخواست
public class RequestLocalizationMiddleware(RequestDelegate next)
{
    private static readonly List<string> _supportedCultures = ["fa", "en", "ar"];

    public async Task Invoke(HttpContext context)
    {
        // ابتدا از هدر Accept-Language بخون
        var lang = context.Request.Headers["Accept-Language"].ToString()?.Split(',')?.FirstOrDefault()?.Trim();

        // در صورت نیاز از Query هم چک کن
        if (string.IsNullOrWhiteSpace(lang))
        {
            lang = context.Request.Query["lang"];
        }

        // بررسی صحت زبان و ست کردن فرهنگ
        if (!_supportedCultures.Contains(lang))
        {
            lang = "fa"; // پیش‌فرض فارسی
        }

        var cultureInfo = new CultureInfo(lang);
        CultureInfo.CurrentCulture = cultureInfo;
        CultureInfo.CurrentUICulture = cultureInfo;

        await next(context);
    }
}

