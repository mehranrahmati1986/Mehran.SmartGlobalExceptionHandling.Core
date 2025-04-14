using Mehran.SmartGlobalExceptionHandling.Core.Config;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Mappers;
using Mehran.SmartGlobalExceptionHandling.Core.Middleware;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Mehran.SmartGlobalExceptionHandling.Core.Extensions;

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
    public static IServiceCollection AddExceptionHandling(this IServiceCollection services, Action<ExceptionHandlingOptions> configure = null)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IErrorMessageLocalizer, LocalizedErrorMessageLocalizer>();
        services.AddSingleton<IExceptionMapper, ExceptionMapper>();

        // افزودن مپ‌کننده‌ی اکسپشن‌ها به مدل پاسخ
        if (configure != null)
            services.Configure(configure);
        else
            services.Configure<ExceptionHandlingOptions>(_ => { });

        return services;
    }

    /// <summary>
    ///  افزودن میدلور مدیریت خطا به 
    ///  <br>pipeline</br>
    /// </summary>
    /// <param name="app"></param>
    /// <returns></returns>
    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}

