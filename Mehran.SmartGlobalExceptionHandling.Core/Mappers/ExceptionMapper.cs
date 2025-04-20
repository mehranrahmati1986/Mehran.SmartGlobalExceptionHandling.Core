using Mehran.SmartGlobalExceptionHandling.Core.Exceptions;
using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Mehran.SmartGlobalExceptionHandling.Core.Options;
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
            TraceId = traceId
        };

        switch (ex)
        {
            case ValidationException validationException:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("Validation");
                result.Details = _options.ShowDetails ? validationException.Message : null;
                result.Errors = validationException.Errors;
                result.StackTrace = _options.StackTrace ? validationException.StackTrace : null;
                result.MetaData = validationException.MetaData;
                break;
            case BusinessException bex:
                result.StatusCode = 400;
                result.Message = !string.IsNullOrWhiteSpace(bex.Code)
                    ? GetLocalizedMessage(bex.Code)
                    : GetLocalizedMessage("Business");
                result.Details = _options.ShowDetails ? ex.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = bex.MetaData;

                break;
            case NotFoundException notFoundException:
                result.StatusCode = 404;
                result.Message = GetLocalizedMessage("NotFound");
                result.Details = _options.ShowDetails ? notFoundException.Detail : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = notFoundException.MetaData;
                break;
            case ArgumentNullException or ArgumentException or ArgumentOutOfRangeException:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("InvalidArgument");
                result.Details = _options.ShowDetails ? ex.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = ex.Data;
                break;
            case UnauthorizedException unauthorizedException:
                result.StatusCode = 401;
                result.Message = GetLocalizedMessage("Unauthorized");
                result.Details = _options.ShowDetails ? unauthorizedException.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = unauthorizedException.MetaData;
                break;
            case ForbiddenException forbiddenException:
                result.StatusCode = 403;
                result.Message = GetLocalizedMessage("Forbidden");
                result.Details = _options.ShowDetails ? ex.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = forbiddenException.MetaData;
                break;
            case ConflictException conflictException:
                result.StatusCode = 409;
                result.Message = GetLocalizedMessage("Conflict");
                result.Details = _options.ShowDetails ? ex.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = conflictException.MetaData;
                break;
            case PaymentRequiredException paymentRequiredException:
                result.StatusCode = 402;
                result.Message = GetLocalizedMessage("PaymentRequired");
                if (_options.ShowDetails)
                    result.Details = paymentRequiredException.InnerException?.Message ?? paymentRequiredException.Message;

                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = paymentRequiredException.MetaData;

                break;
            case TooManyRequestsException tooManyRequestsException:
                result.StatusCode = 429;
                result.Message = GetLocalizedMessage("TooManyRequests");
                result.Details = _options.ShowDetails ? tooManyRequestsException.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = tooManyRequestsException.MetaData;
                break;
            case RequestTimeoutException requestTimeoutException:
                result.StatusCode = 408;
                result.Message = GetLocalizedMessage("Timeout");
                result.Details = _options.ShowDetails ? requestTimeoutException.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = requestTimeoutException.MetaData;
                break;
            case InvalidOperationException invalidOperationException:
                result.StatusCode = 400;
                result.Message = GetLocalizedMessage("InvalidOperation");
                result.Details = _options.ShowDetails ? invalidOperationException.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = ex.Data;
                break;
            case DatabaseUpdateException dbex:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("DbUpdate");
                result.Details = _options.ShowDetails ? $"{dbex.EntityName} => {dbex.Key}" : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = ex.Data;
                break;
            case DbUpdateException efex:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("DbUpdate");
                result.Details = _options.ShowDetails ? efex.InnerException?.Message : null;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = ex.Data;
                break;
            case MethodNotAllowedException methodNotAllowedException:
                result.StatusCode = 405;
                result.Message = GetLocalizedMessage("MethodNotAllowed");
                if (_options.ShowDetails)
                    result.Details = methodNotAllowedException.InnerException?.Message ?? methodNotAllowedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = methodNotAllowedException.MetaData;
                break;
            case NotAcceptableException notAcceptableException:
                result.StatusCode = 406;
                result.Message = GetLocalizedMessage("NotAcceptable");
                if (_options.ShowDetails)
                    result.Details = notAcceptableException.InnerException?.Message ?? notAcceptableException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = notAcceptableException.MetaData;
                break;
            case ProxyAuthenticationRequiredException proxyAuthenticationRequiredException:
                result.StatusCode = 407;
                result.Message = GetLocalizedMessage("ProxyAuthenticationRequired");
                if (_options.ShowDetails)
                    result.Details = proxyAuthenticationRequiredException.InnerException?.Message ?? proxyAuthenticationRequiredException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = proxyAuthenticationRequiredException.MetaData;
                break;
            case GoneException goneException:
                result.StatusCode = 410;
                result.Message = GetLocalizedMessage("Gone");
                if (_options.ShowDetails)
                    result.Details = goneException.InnerException?.Message ?? goneException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = goneException.MetaData;
                break;
            case LengthRequiredException lengthRequiredException:
                result.StatusCode = 411;
                result.Message = GetLocalizedMessage("LengthRequired");
                if (_options.ShowDetails)
                    result.Details = lengthRequiredException.InnerException?.Message ?? lengthRequiredException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = lengthRequiredException.MetaData;
                break;
            case PreconditionFailedException preconditionFailedException:
                result.StatusCode = 412;
                result.Message = GetLocalizedMessage("PreconditionFailed");
                if (_options.ShowDetails)
                    result.Details = preconditionFailedException.InnerException?.Message ?? preconditionFailedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = preconditionFailedException.MetaData;
                break;
            case PayloadTooLargeException payloadTooLargeException:
                result.StatusCode = 413;
                result.Message = GetLocalizedMessage("PayloadTooLarge");
                if (_options.ShowDetails)
                    result.Details = payloadTooLargeException.InnerException?.Message ?? payloadTooLargeException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = payloadTooLargeException.MetaData;
                break;
            case UriTooLongException uriTooLongException:
                result.StatusCode = 414;
                result.Message = GetLocalizedMessage("UriTooLong");
                if (_options.ShowDetails)
                    result.Details = uriTooLongException.InnerException?.Message ?? uriTooLongException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = uriTooLongException.MetaData;
                break;
            case UnsupportedMediaTypeException unsupportedMediaTypeException:
                result.StatusCode = 415;
                result.Message = GetLocalizedMessage("UnsupportedMediaType");
                if (_options.ShowDetails)
                    result.Details = unsupportedMediaTypeException.InnerException?.Message ?? unsupportedMediaTypeException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = unsupportedMediaTypeException.MetaData;
                break;
            case RangeNotSatisfiableException rangeNotSatisfiableException:
                result.StatusCode = 416;
                result.Message = GetLocalizedMessage("RangeNotSatisfiable");
                if (_options.ShowDetails)
                    result.Details = rangeNotSatisfiableException.InnerException?.Message ?? rangeNotSatisfiableException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = rangeNotSatisfiableException.MetaData;
                break;
            case ExpectationFailedException expectationFailedException:
                result.StatusCode = 417;
                result.Message = GetLocalizedMessage("ExpectationFailed");
                if (_options.ShowDetails)
                    result.Details = expectationFailedException.InnerException?.Message ?? expectationFailedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = expectationFailedException.MetaData;
                break;
            case ImATeapotException imATeapotException:
                result.StatusCode = 418;
                result.Message = GetLocalizedMessage("ImATeapot");
                if (_options.ShowDetails)
                    result.Details = imATeapotException.InnerException?.Message ?? imATeapotException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = imATeapotException.MetaData;
                break;
            case AuthenticationTimeoutException authenticationTimeoutException:
                result.StatusCode = 419;
                result.Message = GetLocalizedMessage("AuthenticationTimeout");
                if (_options.ShowDetails)
                    result.Details = authenticationTimeoutException.InnerException?.Message ?? authenticationTimeoutException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = authenticationTimeoutException.MetaData;
                break;
            case MisdirectedRequestException misdirectedRequestException:
                result.StatusCode = 421;
                result.Message = GetLocalizedMessage("MisdirectedRequest");
                if (_options.ShowDetails)
                    result.Details = misdirectedRequestException.InnerException?.Message ?? misdirectedRequestException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = misdirectedRequestException.MetaData;
                break;
            case UnprocessableEntityException unprocessableEntityException:
                result.StatusCode = 422;
                result.Message = GetLocalizedMessage("UnprocessableEntity");
                if (_options.ShowDetails)
                    result.Details = unprocessableEntityException.InnerException?.Message ?? unprocessableEntityException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = unprocessableEntityException.MetaData;
                break;
            case LockedException lockedException:
                result.StatusCode = 423;
                result.Message = GetLocalizedMessage("Locked");
                if (_options.ShowDetails)
                    result.Details = lockedException.InnerException?.Message ?? lockedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = lockedException.MetaData;
                break;
            case FailedDependencyException failedDependencyException:
                result.StatusCode = 424;
                result.Message = GetLocalizedMessage("FailedDependency");
                if (_options.ShowDetails)
                    result.Details = failedDependencyException.InnerException?.Message ?? failedDependencyException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = failedDependencyException.MetaData;
                break;
            case UpgradeRequiredException upgradeRequiredException:
                result.StatusCode = 426;
                result.Message = GetLocalizedMessage("UpgradeRequired");
                if (_options.ShowDetails)
                    result.Details = upgradeRequiredException.InnerException?.Message ?? upgradeRequiredException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = upgradeRequiredException.MetaData;
                break;
            case PreconditionRequiredException preconditionRequiredException:
                result.StatusCode = 428;
                result.Message = GetLocalizedMessage("PreconditionRequired");
                if (_options.ShowDetails)
                    result.Details = preconditionRequiredException.InnerException?.Message ?? preconditionRequiredException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = preconditionRequiredException.MetaData;
                break;
            case RequestHeaderFieldsTooLargeException requestHeaderFieldsTooLargeException:
                result.StatusCode = 431;
                result.Message = GetLocalizedMessage("RequestHeaderFieldsTooLarge");
                if (_options.ShowDetails)
                    result.Details = requestHeaderFieldsTooLargeException.InnerException?.Message ?? requestHeaderFieldsTooLargeException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = requestHeaderFieldsTooLargeException.MetaData;
                break;
            case UnavailableForLegalReasonsException unavailableForLegalReasonsException:
                result.StatusCode = 451;
                result.Message = GetLocalizedMessage("UnavailableForLegalReasons");
                if (_options.ShowDetails)
                    result.Details = unavailableForLegalReasonsException.InnerException?.Message ?? unavailableForLegalReasonsException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = unavailableForLegalReasonsException.MetaData;
                break;
            case ClientClosedRequestException clientClosedRequestException:
                result.StatusCode = 499;
                result.Message = GetLocalizedMessage("ClientClosedRequest");
                if (_options.ShowDetails)
                    result.Details = clientClosedRequestException.InnerException?.Message ?? clientClosedRequestException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = clientClosedRequestException.MetaData;
                break;
            case NotImplementedHttpException notImplementedHttpException:
                result.StatusCode = 501;
                result.Message = GetLocalizedMessage("NotImplemented");
                if (_options.ShowDetails)
                    result.Details = notImplementedHttpException.InnerException?.Message ?? notImplementedHttpException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = notImplementedHttpException.MetaData;
                break;
            case BadGatewayException badGatewayException:
                result.StatusCode = 502;
                result.Message = GetLocalizedMessage("BadGateway");
                if (_options.ShowDetails)
                    result.Details = badGatewayException.InnerException?.Message ?? badGatewayException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = badGatewayException.MetaData;
                break;
            case ServiceUnavailableException serviceUnavailableException:
                result.StatusCode = 503;
                result.Message = GetLocalizedMessage("ServiceUnavailable");
                if (_options.ShowDetails)
                    result.Details = serviceUnavailableException.InnerException?.Message ?? serviceUnavailableException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = serviceUnavailableException.MetaData;
                break;
            case GatewayTimeoutException gatewayTimeoutException:
                result.StatusCode = 504;
                result.Message = GetLocalizedMessage("GatewayTimeout");
                if (_options.ShowDetails)
                    result.Details = gatewayTimeoutException.InnerException?.Message ?? gatewayTimeoutException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = gatewayTimeoutException.MetaData;
                break;
            case HttpVersionNotSupportedException httpVersionNotSupportedException:
                result.StatusCode = 505;
                result.Message = GetLocalizedMessage("HttpVersionNotSupported");
                if (_options.ShowDetails)
                    result.Details = httpVersionNotSupportedException.InnerException?.Message ?? httpVersionNotSupportedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = httpVersionNotSupportedException.MetaData;
                break;
            case VariantAlsoNegotiatesException variantAlsoNegotiatesException:
                result.StatusCode = 506;
                result.Message = GetLocalizedMessage("VariantAlsoNegotiates");
                if (_options.ShowDetails)
                    result.Details = variantAlsoNegotiatesException.InnerException?.Message ?? variantAlsoNegotiatesException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = variantAlsoNegotiatesException.MetaData;
                break;
            case InsufficientStorageException insufficientStorageException:
                result.StatusCode = 507;
                result.Message = GetLocalizedMessage("InsufficientStorage");
                if (_options.ShowDetails)
                    result.Details = insufficientStorageException.InnerException?.Message ?? insufficientStorageException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = insufficientStorageException.MetaData;
                break;
            case LoopDetectedException loopDetectedException:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("LoopDetected");
                if (_options.ShowDetails)
                    result.Details = loopDetectedException.InnerException?.Message ?? loopDetectedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = loopDetectedException.MetaData;
                break;
            case NotExtendedException notExtendedException:
                result.StatusCode = 510;
                result.Message = GetLocalizedMessage("NotExtended");
                if (_options.ShowDetails)
                    result.Details = notExtendedException.InnerException?.Message ?? notExtendedException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = notExtendedException.MetaData;
                break;
            case NetworkAuthenticationRequiredException networkAuthenticationRequiredException:
                result.StatusCode = 511;
                result.Message = GetLocalizedMessage("NetworkAuthenticationRequired");
                if (_options.ShowDetails)
                    result.Details = networkAuthenticationRequiredException.InnerException?.Message ?? networkAuthenticationRequiredException.Message;
                result.StackTrace = _options.StackTrace ? ex.StackTrace : null;
                result.MetaData = networkAuthenticationRequiredException.MetaData;
                break;

            default:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("Unhandled");
                break;
        }

        return result;
    }
}


