using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Mehran.SmartGlobalExceptionHandling.Core.Logging;

/// <summary>
/// پیاده‌سازی پیش‌فرض برای لاگ‌کردن خطاها با استفاده از ILogger
/// </summary>
public class DefaultExceptionLogger(
    ILogger<DefaultExceptionLogger> logger,
    IHttpContextAccessor accessor) : IExceptionLogger
{
    /// <summary>
    /// لاگ‌کردن خطای غیرقابل کنترل با سطح Error
    /// </summary>
    public void Log(Exception exception)
    {
        var traceId = accessor.HttpContext?.TraceIdentifier ?? Guid.NewGuid().ToString();

        using (logger.BeginScope(new Dictionary<string, object> { ["TraceId"] = traceId }))
        {
            logger.LogError(exception, "Unhandled exception occurred. TraceId: {TraceId}", traceId);
        }
    }
}