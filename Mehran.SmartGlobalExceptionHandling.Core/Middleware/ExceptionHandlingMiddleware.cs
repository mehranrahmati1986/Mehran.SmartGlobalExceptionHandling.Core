using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;
using System.Text.Json;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;



public class ExceptionHandlingMiddleware(RequestDelegate next, IErrorMessageLocalizer errorMessageLocalizer)
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

    private Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var traceId = Activity.Current?.Id ?? context.TraceIdentifier;

        object metaData = null;

        switch (exception)
        {
            case BusinessException businessEx:
                metaData = businessEx.MetaData;
                break;

            case ConflictException conflictEx:
                metaData = conflictEx.MetaData;
                break;

            case DatabaseUpdateException databaseUpdateExceptionEx:
                metaData = databaseUpdateExceptionEx.MetaData;
                break;
            
            case ForbiddenException forbiddenExceptionEx:
                metaData = forbiddenExceptionEx.MetaData;
                break;

            case NotFoundException notFoundExceptionEx:
                metaData = notFoundExceptionEx.MetaData;
                break;

            case RequestTimeoutException requestTimeoutExceptionEx:
                metaData = requestTimeoutExceptionEx.MetaData;
                break;
            
            case TooManyRequestsException tooManyRequestsExceptionEx:
                metaData = tooManyRequestsExceptionEx.MetaData;
                break;
            
            case UnauthorizedException unauthorizedExceptionEx:
                metaData = unauthorizedExceptionEx.MetaData;
                break;
            
            
            case ValidationException validationExceptionEx:
                metaData = validationExceptionEx.MetaData;
                break;
        }

        var errorResponse = new ErrorResponse<object>
        {
            StatusCode = StatusCodes.Status500InternalServerError,
            Message = errorMessageLocalizer.Get("UnexpectedError"),
            Details = exception.Message, 
            TraceId = traceId,
            Timestamp = DateTime.UtcNow,
            MetaData = metaData,
        };


        if (exception is ValidationException validationException)
        {
            errorResponse.Errors = [.. validationException.Errors.Select(e => new ValidationError
            {
                Field = e.Field,
                Message = e.Message
            })];
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = errorResponse.StatusCode;

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        return context.Response.WriteAsync(jsonResponse);
    }
}




