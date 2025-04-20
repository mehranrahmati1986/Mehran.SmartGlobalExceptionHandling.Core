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
        var defaultOptions = new ExceptionHandlingOption();

        // اضافه کردن سرویس‌های وابسته
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IErrorMessageLocalizer, LocalizedErrorMessageLocalizer>();
        services.AddSingleton<IExceptionMapper, ExceptionMapper>();

        // تنظیمات اختیاری برای پیکربندی
        if (configure != null)
        {
            services.Configure(configure);
        }
        else
        {
            // در صورت عدم ارائه تنظیمات سفارشی، از تنظیمات پیش‌فرض استفاده می‌شود
            services.Configure<ExceptionHandlingOption>(opt =>
            {
                opt.ShowDetails = defaultOptions.ShowDetails;
                opt.LogExceptions = defaultOptions.LogExceptions;
                opt.Language = defaultOptions.Language;
                opt.HandleFluentValidationErrors = defaultOptions.HandleFluentValidationErrors;
                opt.ConfigureFluentValidationLanguage = defaultOptions.ConfigureFluentValidationLanguage;
            });
        }

        services.AddSingleton(sp =>
            sp.GetRequiredService<IOptions<ExceptionHandlingOption>>().Value
        );

        // دریافت تنظیمات نهایی از DI به‌طور موقت
        using (var serviceProvider = services.BuildServiceProvider())
        {
            var finalOptions = serviceProvider.GetRequiredService<IOptions<ExceptionHandlingOption>>().Value;
            if (finalOptions.HandleFluentValidationErrors)
            {
                var errorLocalizer = serviceProvider.GetRequiredService<IErrorMessageLocalizer>();
                services.AddCustomApiBehavior(errorLocalizer);
            }
        }

        return services;
    }

    /// <summary>
    /// افزودن میدلور مدیریت خطا به pipeline
    /// </summary>
    /// <param name="app">اپلیکیشن بیلدر</param>
    /// <returns></returns>
    public static IApplicationBuilder UseCustomExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<ExceptionHandlingMiddleware>();
    }
}
