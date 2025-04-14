using Mehran.SmartGlobalExceptionHandling.Core.Config;
using Mehran.SmartGlobalExceptionHandling.Core.Mappers;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    ILogger<ExceptionHandlingMiddleware> logger,
    IExceptionMapper mapper,
    IOptions<ExceptionHandlingOptions> options)
{
    private readonly ExceptionHandlingOptions _options = options.Value;

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            var response = mapper.Map(ex);

            context.Response.StatusCode = response.StatusCode;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsync(JsonSerializer.Serialize(response));

            if (_options.LogExceptions)
            {
                logger.LogError(ex, "Unhandled exception occurred.");
            }
        }
    }
}


