using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Mehran.SmartGlobalExceptionHandling.Core.Options;
using Microsoft.AspNetCore.Http;
using System.Text.Json;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    IErrorMessageLocalizer errorMessageLocalizer,
    ExceptionHandlingOption option)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    public async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = context.TraceIdentifier;
        var metaData = new Dictionary<string, object>();

        var errorResponse = new ErrorResponse<object>
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = errorMessageLocalizer.Get("UnexpectedError"),
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            MetaData = metaData
        };

        switch (exception)
        {
            case ValidationException validationException:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = errorMessageLocalizer.Get("Validation");
                errorResponse.Details = option.ShowDetails ? validationException.Message : null;
                errorResponse.Errors = [.. validationException.Errors.Select(e => new ValidationError
                {
                    Field = e.Field,
                    Message = e.Message
                })];
                errorResponse.MetaData = validationException.MetaData;
                break;

            case BusinessException businessException:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = !string.IsNullOrWhiteSpace(businessException.Code)
                    ? errorMessageLocalizer.Get(businessException.Code)
                    : errorMessageLocalizer.Get("Business");
                errorResponse.Details = option.ShowDetails ? businessException.Message : null;
                errorResponse.MetaData = businessException.MetaData;
                break;

            case NotFoundException notFoundException:
                errorResponse.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Message = errorMessageLocalizer.Get("NotFound");
                errorResponse.Details = option.ShowDetails ? notFoundException.Message : null;
                errorResponse.MetaData = notFoundException.MetaData;
                break;

            // مثال‌های اضافی از خطاهای دیگر:
            case ArgumentNullException argumentNullException:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = errorMessageLocalizer.Get("ArgumentNull");
                errorResponse.Details = option.ShowDetails ? argumentNullException.Message : null;
                errorResponse.MetaData = argumentNullException.Data;
                break;

            case UnauthorizedAccessException unauthorizedAccessException:
                errorResponse.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse.Message = errorMessageLocalizer.Get("Unauthorized");
                errorResponse.Details = option.ShowDetails ? unauthorizedAccessException.Message : null;
                errorResponse.MetaData = unauthorizedAccessException.Data;
                break;

            // خطای پیش‌فرض برای سایر استثناها
            default:
                errorResponse.Message = errorMessageLocalizer.Get("Unhandled");
                errorResponse.Details = option.ShowDetails ? exception.ToString() : null;
                errorResponse.MetaData = exception.Data;
                break;
        }

        context.Response.StatusCode = errorResponse.StatusCode;
        context.Response.ContentType = "application/json";

        var responseJson = JsonSerializer.Serialize(errorResponse, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = false
        });

        await context.Response.WriteAsync(responseJson);
    }
}





