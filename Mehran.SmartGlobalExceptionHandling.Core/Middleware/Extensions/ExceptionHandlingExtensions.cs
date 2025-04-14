﻿using Mehran.SmartGlobalExceptionHandling.Core.Config;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Mappers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

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
    public static IServiceCollection AddMehranExceptionHandling(
        this IServiceCollection services,
        IConfiguration configuration,
        Action<ExceptionHandlingOption> configure = null)
    {
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IErrorMessageLocalizer, LocalizedErrorMessageLocalizer>();
        services.AddSingleton<IExceptionMapper, ExceptionMapper>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddSingleton<IErrorMessageLocalizer, LocalizedErrorMessageLocalizer>();
        services.AddSingleton<IExceptionMapper, ExceptionMapper>();

        services.Configure<ExceptionHandlingOption>(op =>
        {
            configuration.GetSection(nameof(ExceptionHandlingOption)).Bind(op);
        });

        // اعمال override اگر configure ارائه شده باشه
        if (configure != null)
        {
            services.Configure(configure);
        }

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