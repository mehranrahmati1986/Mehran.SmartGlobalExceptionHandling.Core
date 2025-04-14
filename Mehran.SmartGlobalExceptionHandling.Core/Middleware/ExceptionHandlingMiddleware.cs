using Mehran.SmartGlobalExceptionHandling.Core.Config;
using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
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
                break;

            case BusinessException businessException:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = !string.IsNullOrWhiteSpace(businessException.Code)
                    ? errorMessageLocalizer.Get(businessException.Code)
                    : errorMessageLocalizer.Get("Business");
                errorResponse.Details = option.ShowDetails ? businessException.Message : null;
                break;

            case NotFoundException notFoundException:
                errorResponse.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Message = errorMessageLocalizer.Get("NotFound");
                errorResponse.Details = option.ShowDetails ? notFoundException.Message : null;
                break;

            // بقیه‌ی اکسپشن‌ها رو هم می‌تونی اینجا اضافه کنی

            default:
                errorResponse.Message = errorMessageLocalizer.Get("Unhandled");
                errorResponse.Details = option.ShowDetails ? exception.ToString() : null;
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




