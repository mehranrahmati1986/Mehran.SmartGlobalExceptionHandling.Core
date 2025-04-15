using Mehran.SmartGlobalExceptionHandling.Core.Enums;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Mappers;
using Mehran.SmartGlobalExceptionHandling.Core.Options;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware.Extensions;

/// <summary>
/// اکستنشن برای افزودن سرویس‌های مدیریت خطا به DI
/// </summary>
public static class ExceptionHandlingExtensions
{
    /// <summary>
    /// افزودن سرویس‌های مدیریت خطا به DI
    /// </summary>
    /// <param name="services">سرویس کالکشن برنامه</param>
    /// <param name="configure">تنظیمات اختیاری برای پیکربندی</param>
    /// <returns></returns>
    public static IServiceCollection AddMehranExceptionHandling(this IServiceCollection services, Action<ExceptionHandlingOption> configure = null)
    {
        // اضافه کردن سرویس‌های وابسته
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IErrorMessageLocalizer, LocalizedErrorMessageLocalizer>();
        services.AddSingleton<IExceptionMapper, ExceptionMapper>();

        // تنظیمات اختیاری برای پیکربندی
        if (configure != null)
        {
            // اگر تنظیمات اختیاری ارائه شده باشد، اعمال می‌شود
            services.Configure(configure);
        }
        else
        {
            // در غیر این صورت تنظیمات پیش‌فرض اعمال می‌شود
            services.Configure<ExceptionHandlingOption>(options =>
            {
                options.Language = SupportedLanguage.Fa;  // زبان پیش‌فرض فارسی
            });
        }

        // ثبت ExceptionHandlingOption به عنوان Singleton در DI
        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<ExceptionHandlingOption>>().Value
        );

        return services;
    }

    /// <summary>
    ///  افزودن میدلور مدیریت خطا به pipeline
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
