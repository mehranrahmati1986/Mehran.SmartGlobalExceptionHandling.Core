using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.FluentValidations;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Mehran.SmartGlobalExceptionHandling.Core.Options;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware;

public class ExceptionHandlingMiddleware(
    RequestDelegate next,
    IErrorMessageLocalizer errorMessageLocalizer,
    ILogger<ExceptionHandlingMiddleware> logger,
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
            if (option.LogExceptions)
            {
                logger.LogError(ex, "Unhandled exception occurred.");
            }

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
                errorResponse.StackTrace = option.StackTrace ? validationException.StackTrace : null;
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
                errorResponse.StackTrace = option.StackTrace ? businessException.StackTrace : null;
                errorResponse.MetaData = businessException.MetaData;
                break;
            case NotFoundException notFoundException:
                errorResponse.StatusCode = StatusCodes.Status404NotFound;
                errorResponse.Message = errorMessageLocalizer.Get("NotFound");
                errorResponse.Details = notFoundException.GetType().Name;
                errorResponse.StackTrace = option.StackTrace ? notFoundException.StackTrace : null;
                errorResponse.MetaData = notFoundException.MetaData;
                break;
            case ArgumentNullException argumentNullException:
                errorResponse.StatusCode = StatusCodes.Status400BadRequest;
                errorResponse.Message = errorMessageLocalizer.Get("ArgumentNull");
                errorResponse.Details = option.ShowDetails ? argumentNullException.Message : null;
                errorResponse.StackTrace = option.StackTrace ? argumentNullException.StackTrace : null;
                errorResponse.MetaData = argumentNullException.Data;
                break;
            case UnauthorizedAccessException unauthorizedAccessException:
                errorResponse.StatusCode = StatusCodes.Status401Unauthorized;
                errorResponse.Message = errorMessageLocalizer.Get("Unauthorized");
                errorResponse.Details = option.ShowDetails ? unauthorizedAccessException.Message : null;
                errorResponse.StackTrace = option.StackTrace ? unauthorizedAccessException.StackTrace : null;
                errorResponse.MetaData = unauthorizedAccessException.Data;
                break;
            case FluentValidation.ValidationException fluentValidationException:
                {
                    // تنظیم زبان فقط زمانی که گزینه فعال باشه
                    if (option.ConfigureFluentValidationLanguage)
                    {
                        FluentValidationConfigurator.ConfigureLanguage(option.Language);
                    }

                    if (option.HandleFluentValidationErrors)
                    {
                        var errors = fluentValidationException.Errors
                            .GroupBy(e => e.PropertyName)
                            .ToDictionary(
                                g => g.Key,
                                g => g.Select(e => e.ErrorMessage).ToArray());

                        errorResponse.StatusCode = StatusCodes.Status422UnprocessableEntity;
                        errorResponse.Message = errorMessageLocalizer.Get("Validation");
                        errorResponse.FluentValidationErrors = errors;
                        errorResponse.Details = option.ShowDetails ? fluentValidationException.ToString() : null;
                        errorResponse.StackTrace = option.StackTrace ? fluentValidationException.StackTrace : null;
                        errorResponse.MetaData = fluentValidationException.Data;
                    }

                    break;
                }
            case PaymentRequiredException paymentRequiredException:
                errorResponse.StatusCode = 402;
                errorResponse.Message = errorMessageLocalizer.Get("PaymentRequired");
                if (option.ShowDetails)
                    errorResponse.Details = paymentRequiredException.InnerException?.Message ?? paymentRequiredException.Message;

                errorResponse.StackTrace = option.StackTrace ? paymentRequiredException.StackTrace : null;
                errorResponse.MetaData = paymentRequiredException.MetaData ?? paymentRequiredException.Data;

                break;
            case TooManyRequestsException tooManyRequestsException:
                errorResponse.StatusCode = 429;
                errorResponse.Message = errorMessageLocalizer.Get("TooManyRequests");
                errorResponse.Details = option.ShowDetails ? tooManyRequestsException.Message : null;
                errorResponse.StackTrace = option.StackTrace ? tooManyRequestsException.StackTrace : null;
                errorResponse.MetaData = tooManyRequestsException.MetaData ?? tooManyRequestsException.Data;
                break;
            case RequestTimeoutException requestTimeoutException:
                errorResponse.StatusCode = 408;
                errorResponse.Message = errorMessageLocalizer.Get("Timeout");
                errorResponse.Details = option.ShowDetails ? requestTimeoutException.Message : null;
                errorResponse.StackTrace = option.StackTrace ? requestTimeoutException.StackTrace : null;
                errorResponse.MetaData = requestTimeoutException.MetaData ?? requestTimeoutException.Data;
                break;
            case InvalidOperationException invalidOperationException:
                errorResponse.StatusCode = 400;
                errorResponse.Message = errorMessageLocalizer.Get("InvalidOperation");
                errorResponse.Details = option.ShowDetails ? invalidOperationException.Message : null;
                errorResponse.StackTrace = option.StackTrace ? invalidOperationException.StackTrace : null;
                errorResponse.MetaData = invalidOperationException.Data;
                break;
            case DatabaseUpdateException dbex:
                errorResponse.StatusCode = 500;
                errorResponse.Message = errorMessageLocalizer.Get("DbUpdate");
                errorResponse.Details = option.ShowDetails ? $"{dbex.EntityName} => {dbex.Key}" : null;
                errorResponse.StackTrace = option.StackTrace ? dbex.StackTrace : null;
                errorResponse.MetaData = dbex.MetaData ?? dbex.Data;
                break;
            case DbUpdateException efex:
                errorResponse.StatusCode = 500;
                errorResponse.Message = errorMessageLocalizer.Get("DbUpdate");
                errorResponse.Details = option.ShowDetails ? efex.InnerException?.Message : null;
                errorResponse.StackTrace = option.StackTrace ? efex.StackTrace : null;
                errorResponse.MetaData = efex.Data;
                break;
            case MethodNotAllowedException methodNotAllowedException:
                errorResponse.StatusCode = 405;
                errorResponse.Message = errorMessageLocalizer.Get("MethodNotAllowed");
                if (option.ShowDetails)
                    errorResponse.Details = methodNotAllowedException.InnerException?.Message ?? methodNotAllowedException.Message;
                errorResponse.StackTrace = option.StackTrace ? methodNotAllowedException.StackTrace : null;
                errorResponse.MetaData = methodNotAllowedException.MetaData ?? methodNotAllowedException.Data;
                break;
            case NotAcceptableException notAcceptableException:
                errorResponse.StatusCode = 406;
                errorResponse.Message = errorMessageLocalizer.Get("NotAcceptable");
                if (option.ShowDetails)
                    errorResponse.Details = notAcceptableException.InnerException?.Message ?? notAcceptableException.Message;
                errorResponse.StackTrace = option.StackTrace ? notAcceptableException.StackTrace : null;
                errorResponse.MetaData = notAcceptableException.MetaData ?? notAcceptableException.Data;
                break;
            case ProxyAuthenticationRequiredException proxyAuthenticationRequiredException:
                errorResponse.StatusCode = 407;
                errorResponse.Message = errorMessageLocalizer.Get("ProxyAuthenticationRequired");
                if (option.ShowDetails)
                    errorResponse.Details = proxyAuthenticationRequiredException.InnerException?.Message ?? proxyAuthenticationRequiredException.Message;
                errorResponse.StackTrace = option.StackTrace ? proxyAuthenticationRequiredException.StackTrace : null;
                errorResponse.MetaData = proxyAuthenticationRequiredException.MetaData ?? proxyAuthenticationRequiredException.Data;
                break;
            case GoneException goneException:
                errorResponse.StatusCode = 410;
                errorResponse.Message = errorMessageLocalizer.Get("Gone");
                if (option.ShowDetails)
                    errorResponse.Details = goneException.InnerException?.Message ?? goneException.Message;
                errorResponse.StackTrace = option.StackTrace ? goneException.StackTrace : null;
                errorResponse.MetaData = goneException.MetaData ?? goneException.Data;
                break;
            case LengthRequiredException lengthRequiredException:
                errorResponse.StatusCode = 411;
                errorResponse.Message = errorMessageLocalizer.Get("LengthRequired");
                if (option.ShowDetails)
                    errorResponse.Details = lengthRequiredException.InnerException?.Message ?? lengthRequiredException.Message;
                errorResponse.StackTrace = option.StackTrace ? lengthRequiredException.StackTrace : null;
                errorResponse.MetaData = lengthRequiredException.MetaData ?? lengthRequiredException.Data;
                break;
            case PreconditionFailedException preconditionFailedException:
                errorResponse.StatusCode = 412;
                errorResponse.Message = errorMessageLocalizer.Get("PreconditionFailed");
                if (option.ShowDetails)
                    errorResponse.Details = preconditionFailedException.InnerException?.Message ?? preconditionFailedException.Message;
                errorResponse.StackTrace = option.StackTrace ? preconditionFailedException.StackTrace : null;
                errorResponse.MetaData = preconditionFailedException.MetaData ?? preconditionFailedException.Data;
                break;
            case PayloadTooLargeException payloadTooLargeException:
                errorResponse.StatusCode = 413;
                errorResponse.Message = errorMessageLocalizer.Get("PayloadTooLarge");
                if (option.ShowDetails)
                    errorResponse.Details = payloadTooLargeException.InnerException?.Message ?? payloadTooLargeException.Message;
                errorResponse.StackTrace = option.StackTrace ? payloadTooLargeException.StackTrace : null;
                errorResponse.MetaData = payloadTooLargeException.MetaData ?? payloadTooLargeException.Data;
                break;
            case UriTooLongException uriTooLongException:
                errorResponse.StatusCode = 414;
                errorResponse.Message = errorMessageLocalizer.Get("UriTooLong");
                if (option.ShowDetails)
                    errorResponse.Details = uriTooLongException.InnerException?.Message ?? uriTooLongException.Message;
                errorResponse.StackTrace = option.StackTrace ? uriTooLongException.StackTrace : null;
                errorResponse.MetaData = uriTooLongException.MetaData ?? uriTooLongException.Data;
                break;
            case UnsupportedMediaTypeException unsupportedMediaTypeException:
                errorResponse.StatusCode = 415;
                errorResponse.Message = errorMessageLocalizer.Get("UnsupportedMediaType");
                if (option.ShowDetails)
                    errorResponse.Details = unsupportedMediaTypeException.InnerException?.Message ?? unsupportedMediaTypeException.Message;
                errorResponse.StackTrace = option.StackTrace ? unsupportedMediaTypeException.StackTrace : null;
                errorResponse.MetaData = unsupportedMediaTypeException.MetaData ?? unsupportedMediaTypeException.Data;
                break;
            case RangeNotSatisfiableException rangeNotSatisfiableException:
                errorResponse.StatusCode = 416;
                errorResponse.Message = errorMessageLocalizer.Get("RangeNotSatisfiable");
                if (option.ShowDetails)
                    errorResponse.Details = rangeNotSatisfiableException.InnerException?.Message ?? rangeNotSatisfiableException.Message;
                errorResponse.StackTrace = option.StackTrace ? rangeNotSatisfiableException.StackTrace : null;
                errorResponse.MetaData = rangeNotSatisfiableException.MetaData ?? rangeNotSatisfiableException.Data;
                break;
            case ExpectationFailedException expectationFailedException:
                errorResponse.StatusCode = 417;
                errorResponse.Message = errorMessageLocalizer.Get("ExpectationFailed");
                if (option.ShowDetails)
                    errorResponse.Details = expectationFailedException.InnerException?.Message ?? expectationFailedException.Message;
                errorResponse.StackTrace = option.StackTrace ? expectationFailedException.StackTrace : null;
                errorResponse.MetaData = expectationFailedException.MetaData ?? expectationFailedException.Data;
                break;
            case ImATeapotException imATeapotException:
                errorResponse.StatusCode = 418;
                errorResponse.Message = errorMessageLocalizer.Get("ImATeapot");
                if (option.ShowDetails)
                    errorResponse.Details = imATeapotException.InnerException?.Message ?? imATeapotException.Message;
                errorResponse.StackTrace = option.StackTrace ? imATeapotException.StackTrace : null;
                errorResponse.MetaData = imATeapotException.MetaData ?? imATeapotException.Data;
                break;
            case AuthenticationTimeoutException authenticationTimeoutException:
                errorResponse.StatusCode = 419;
                errorResponse.Message = errorMessageLocalizer.Get("AuthenticationTimeout");
                if (option.ShowDetails)
                    errorResponse.Details = authenticationTimeoutException.InnerException?.Message ?? authenticationTimeoutException.Message;
                errorResponse.StackTrace = option.StackTrace ? authenticationTimeoutException.StackTrace : null;
                errorResponse.MetaData = authenticationTimeoutException.MetaData ?? authenticationTimeoutException;
                break;
            case MisdirectedRequestException misdirectedRequestException:
                errorResponse.StatusCode = 421;
                errorResponse.Message = errorMessageLocalizer.Get("MisdirectedRequest");
                if (option.ShowDetails)
                    errorResponse.Details = misdirectedRequestException.InnerException?.Message ?? misdirectedRequestException.Message;
                errorResponse.StackTrace = option.StackTrace ? misdirectedRequestException.StackTrace : null;
                errorResponse.MetaData = misdirectedRequestException.MetaData ?? misdirectedRequestException.Data;
                break;
            case UnprocessableEntityException unprocessableEntityException:
                errorResponse.StatusCode = 422;
                errorResponse.Message = errorMessageLocalizer.Get("UnprocessableEntity");
                if (option.ShowDetails)
                    errorResponse.Details = unprocessableEntityException.InnerException?.Message ?? unprocessableEntityException.Message;
                errorResponse.StackTrace = option.StackTrace ? unprocessableEntityException.StackTrace : null;
                errorResponse.MetaData = unprocessableEntityException.MetaData ?? unprocessableEntityException.Data;
                break;
            case LockedException lockedException:
                errorResponse.StatusCode = 423;
                errorResponse.Message = errorMessageLocalizer.Get("Locked");
                if (option.ShowDetails)
                    errorResponse.Details = lockedException.InnerException?.Message ?? lockedException.Message;
                errorResponse.StackTrace = option.StackTrace ? lockedException.StackTrace : null;
                errorResponse.MetaData = lockedException.MetaData ?? lockedException.Data;
                break;
            case FailedDependencyException failedDependencyException:
                errorResponse.StatusCode = 424;
                errorResponse.Message = errorMessageLocalizer.Get("FailedDependency");
                if (option.ShowDetails)
                    errorResponse.Details = failedDependencyException.InnerException?.Message ?? failedDependencyException.Message;
                errorResponse.StackTrace = option.StackTrace ? failedDependencyException.StackTrace : null;
                errorResponse.MetaData = failedDependencyException.MetaData ?? failedDependencyException.Data;
                break;
            case UpgradeRequiredException upgradeRequiredException:
                errorResponse.StatusCode = 426;
                errorResponse.Message = errorMessageLocalizer.Get("UpgradeRequired");
                if (option.ShowDetails)
                    errorResponse.Details = upgradeRequiredException.InnerException?.Message ?? upgradeRequiredException.Message;
                errorResponse.StackTrace = option.StackTrace ? upgradeRequiredException.StackTrace : null;
                errorResponse.MetaData = upgradeRequiredException.MetaData ?? upgradeRequiredException.Data;
                break;
            case PreconditionRequiredException preconditionRequiredException:
                errorResponse.StatusCode = 428;
                errorResponse.Message = errorMessageLocalizer.Get("PreconditionRequired");
                if (option.ShowDetails)
                    errorResponse.Details = preconditionRequiredException.InnerException?.Message ?? preconditionRequiredException.Message;
                errorResponse.StackTrace = option.StackTrace ? preconditionRequiredException.StackTrace : null;
                errorResponse.MetaData = preconditionRequiredException.MetaData ?? preconditionRequiredException.Data;
                break;
            case RequestHeaderFieldsTooLargeException requestHeaderFieldsTooLargeException:
                errorResponse.StatusCode = 431;
                errorResponse.Message = errorMessageLocalizer.Get("RequestHeaderFieldsTooLarge");
                if (option.ShowDetails)
                    errorResponse.Details = requestHeaderFieldsTooLargeException.InnerException?.Message ?? requestHeaderFieldsTooLargeException.Message;
                errorResponse.StackTrace = option.StackTrace ? requestHeaderFieldsTooLargeException.StackTrace : null;
                errorResponse.MetaData = requestHeaderFieldsTooLargeException.MetaData?? requestHeaderFieldsTooLargeException;
                break;
            case UnavailableForLegalReasonsException unavailableForLegalReasonsException:
                errorResponse.StatusCode = 451;
                errorResponse.Message = errorMessageLocalizer.Get("UnavailableForLegalReasons");
                if (option.ShowDetails)
                    errorResponse.Details = unavailableForLegalReasonsException.InnerException?.Message ?? unavailableForLegalReasonsException.Message;
                errorResponse.StackTrace = option.StackTrace ? unavailableForLegalReasonsException.StackTrace : null;
                errorResponse.MetaData = unavailableForLegalReasonsException.MetaData ?? unavailableForLegalReasonsException.Data;
                break;
            case ClientClosedRequestException clientClosedRequestException:
                errorResponse.StatusCode = 499;
                errorResponse.Message = errorMessageLocalizer.Get("ClientClosedRequest");
                if (option.ShowDetails)
                    errorResponse.Details = clientClosedRequestException.InnerException?.Message ?? clientClosedRequestException.Message;
                errorResponse.StackTrace = option.StackTrace ? clientClosedRequestException.StackTrace : null;
                errorResponse.MetaData = clientClosedRequestException.MetaData ?? clientClosedRequestException.Data;
                break;
            case NotImplementedHttpException notImplementedHttpException:
                errorResponse.StatusCode = 501;
                errorResponse.Message = errorMessageLocalizer.Get("NotImplemented");
                if (option.ShowDetails)
                    errorResponse.Details = notImplementedHttpException.InnerException?.Message ?? notImplementedHttpException.Message;
                errorResponse.StackTrace = option.StackTrace ? notImplementedHttpException.StackTrace : null;
                errorResponse.MetaData = notImplementedHttpException.MetaData ?? notImplementedHttpException;
                break;
            case BadGatewayException badGatewayException:
                errorResponse.StatusCode = 502;
                errorResponse.Message = errorMessageLocalizer.Get("BadGateway");
                if (option.ShowDetails)
                    errorResponse.Details = badGatewayException.InnerException?.Message ?? badGatewayException.Message;
                errorResponse.StackTrace = option.StackTrace ? badGatewayException.StackTrace : null;
                errorResponse.MetaData = badGatewayException.MetaData ?? badGatewayException.Data;
                break;
            case ServiceUnavailableException serviceUnavailableException:
                errorResponse.StatusCode = 503;
                errorResponse.Message = errorMessageLocalizer.Get("ServiceUnavailable");
                if (option.ShowDetails)
                    errorResponse.Details = serviceUnavailableException.InnerException?.Message ?? serviceUnavailableException.Message;
                errorResponse.StackTrace = option.StackTrace ? serviceUnavailableException.StackTrace : null;
                errorResponse.MetaData = serviceUnavailableException.MetaData ?? serviceUnavailableException.Data;
                break;
            case GatewayTimeoutException gatewayTimeoutException:
                errorResponse.StatusCode = 504;
                errorResponse.Message = errorMessageLocalizer.Get("GatewayTimeout");
                if (option.ShowDetails)
                    errorResponse.Details = gatewayTimeoutException.InnerException?.Message ?? gatewayTimeoutException.Message;
                errorResponse.StackTrace = option.StackTrace ? gatewayTimeoutException.StackTrace : null;
                errorResponse.MetaData = gatewayTimeoutException.MetaData ?? gatewayTimeoutException.Data;
                break;
            case HttpVersionNotSupportedException httpVersionNotSupportedException:
                errorResponse.StatusCode = 505;
                errorResponse.Message = errorMessageLocalizer.Get("HttpVersionNotSupported");
                if (option.ShowDetails)
                    errorResponse.Details = httpVersionNotSupportedException.InnerException?.Message ?? httpVersionNotSupportedException.Message;
                errorResponse.StackTrace = option.StackTrace ? httpVersionNotSupportedException.StackTrace : null;
                errorResponse.MetaData = httpVersionNotSupportedException.MetaData ?? httpVersionNotSupportedException;
                break;
            case VariantAlsoNegotiatesException variantAlsoNegotiatesException:
                errorResponse.StatusCode = 506;
                errorResponse.Message = errorMessageLocalizer.Get("VariantAlsoNegotiates");
                if (option.ShowDetails)
                    errorResponse.Details = variantAlsoNegotiatesException.InnerException?.Message ?? variantAlsoNegotiatesException.Message;
                errorResponse.StackTrace = option.StackTrace ? variantAlsoNegotiatesException.StackTrace : null;
                errorResponse.MetaData = variantAlsoNegotiatesException.MetaData ?? variantAlsoNegotiatesException.Data;
                break;
            case InsufficientStorageException insufficientStorageException:
                errorResponse.StatusCode = 507;
                errorResponse.Message = errorMessageLocalizer.Get("InsufficientStorage");
                if (option.ShowDetails)
                    errorResponse.Details = insufficientStorageException.InnerException?.Message ?? insufficientStorageException.Message;
                errorResponse.StackTrace = option.StackTrace ? insufficientStorageException.StackTrace : null;
                errorResponse.MetaData = insufficientStorageException.MetaData ?? insufficientStorageException.Data;
                break;
            case LoopDetectedException loopDetectedException:
                errorResponse.StatusCode = 500;
                errorResponse.Message = errorMessageLocalizer.Get("LoopDetected");
                if (option.ShowDetails)
                    errorResponse.Details = loopDetectedException.InnerException?.Message ?? loopDetectedException.Message;
                errorResponse.StackTrace = option.StackTrace ? loopDetectedException.StackTrace : null;
                errorResponse.MetaData = loopDetectedException.MetaData ?? loopDetectedException;
                break;
            case NotExtendedException notExtendedException:
                errorResponse.StatusCode = 510;
                errorResponse.Message = errorMessageLocalizer.Get("NotExtended");
                if (option.ShowDetails)
                    errorResponse.Details = notExtendedException.InnerException?.Message ?? notExtendedException.Message;
                errorResponse.StackTrace = option.StackTrace ? notExtendedException.StackTrace : null;
                errorResponse.MetaData = notExtendedException.MetaData ?? notExtendedException.Data;
                break;
            case NetworkAuthenticationRequiredException networkAuthenticationRequiredException:
                errorResponse.StatusCode = 511;
                errorResponse.Message = errorMessageLocalizer.Get("NetworkAuthenticationRequired");
                if (option.ShowDetails)
                    errorResponse.Details = networkAuthenticationRequiredException.InnerException?.Message ?? networkAuthenticationRequiredException.Message;
                errorResponse.StackTrace = option.StackTrace ? networkAuthenticationRequiredException.StackTrace : null;
                errorResponse.MetaData = networkAuthenticationRequiredException.MetaData ?? networkAuthenticationRequiredException.Data;
                break;

            default:
                errorResponse.Message = errorMessageLocalizer.Get("Unhandled");
                errorResponse.Details = option.ShowDetails ? exception.ToString() : null;
                errorResponse.StackTrace = option.StackTrace ? exception.StackTrace : null;
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
