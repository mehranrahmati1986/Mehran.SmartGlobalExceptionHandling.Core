using Mehran.SmartGlobalExceptionHandling.Core.Config;
using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Mehran.SmartGlobalExceptionHandling.Core.Mappers;

public class ExceptionMapper(
    IErrorMessageLocalizer localizer,
    IHttpContextAccessor httpContextAccessor,
    IOptions<ExceptionHandlingOption> options) : IExceptionMapper
{
    private readonly ExceptionHandlingOption _options = options.Value;

    private string GetLocalizedMessage(string key)
    {
        if (_options.DefaultMessages.TryGetValue(key, out var msg))
            return msg;

        return localizer.Get(key);
    }

    public ErrorResponse<object> Map(Exception ex)
    {
        var traceId = httpContextAccessor.HttpContext?.TraceIdentifier;
        var result = new ErrorResponse<object>
        {
            StatusCode = 500,
            TraceId = traceId
        };

        switch (ex)
        {
            case NotFoundException:
                result.StatusCode = 404;
                result.Message = GetLocalizedMessage("NotFound");
                break;

            case ValidationException vex:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("Validation");
                result.Errors = vex.Errors;
                break;

            case UnauthorizedException:
                result.StatusCode = 401;
                result.Message = GetLocalizedMessage("Unauthorized");
                break;

            case ForbiddenException:
                result.StatusCode = 403;
                result.Message = GetLocalizedMessage("Forbidden");
                break;

            case ConflictException:
                result.StatusCode = 409;
                result.Message = GetLocalizedMessage("Conflict");
                break;

            case BusinessException bex:
                result.StatusCode = 400;
                result.Message = !string.IsNullOrWhiteSpace(bex.Code)
                    ? GetLocalizedMessage(bex.Code)
                    : GetLocalizedMessage("Business");

                if (_options.ShowDetails)
                    result.Details = bex.Message;
                break;

            case TooManyRequestsException:
                result.StatusCode = 429;
                result.Message = GetLocalizedMessage("TooManyRequests");
                break;

            case RequestTimeoutException:
                result.StatusCode = 408;
                result.Message = GetLocalizedMessage("Timeout");
                break;

            case ArgumentNullException or ArgumentException or ArgumentOutOfRangeException:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("InvalidArgument");
                if (_options.ShowDetails)
                    result.Details = ex.Message;
                break;

            case InvalidOperationException:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("InvalidOperation");
                break;

            case DatabaseUpdateException dbex:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("DbUpdate");
                if (_options.ShowDetails)
                    result.Details = $"{dbex.EntityName} => {dbex.Key}";
                break;

            case DbUpdateException efex:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("DbUpdate");
                if (_options.ShowDetails)
                    result.Details = efex.InnerException?.Message ?? efex.Message;
                break;

            default:
                result.Message = GetLocalizedMessage("Unhandled");
                break;
        }

        return result;
    }
}


