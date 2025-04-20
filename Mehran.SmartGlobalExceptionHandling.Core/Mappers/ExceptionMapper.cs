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

            case PaymentRequiredException paymentRequiredException:
                result.StatusCode = 402;
                result.Message = GetLocalizedMessage("PaymentRequired");
                if (_options.ShowDetails)
                    result.Details = paymentRequiredException.InnerException?.Message ?? paymentRequiredException.Message;
                break;

            case MethodNotAllowedException methodNotAllowedException:
                result.StatusCode = 405;
                result.Message = GetLocalizedMessage("MethodNotAllowed");
                if (_options.ShowDetails)
                    result.Details = methodNotAllowedException.InnerException?.Message ?? methodNotAllowedException.Message;
                break;

            case NotAcceptableException notAcceptableException:
                result.StatusCode = 406;
                result.Message = GetLocalizedMessage("NotAcceptable");
                if (_options.ShowDetails)
                    result.Details = notAcceptableException.InnerException?.Message ?? notAcceptableException.Message;
                break;

            case ProxyAuthenticationRequiredException proxyAuthenticationRequiredException:
                result.StatusCode = 407;
                result.Message = GetLocalizedMessage("ProxyAuthenticationRequired");
                if (_options.ShowDetails)
                    result.Details = proxyAuthenticationRequiredException.InnerException?.Message ?? proxyAuthenticationRequiredException.Message;
                break;

            case GoneException goneException:
                result.StatusCode = 410;
                result.Message = GetLocalizedMessage("Gone");
                if (_options.ShowDetails)
                    result.Details = goneException.InnerException?.Message ?? goneException.Message;
                break;

            case LengthRequiredException lengthRequiredException:
                result.StatusCode = 411;
                result.Message = GetLocalizedMessage("LengthRequired");
                if (_options.ShowDetails)
                    result.Details = lengthRequiredException.InnerException?.Message ?? lengthRequiredException.Message;
                break;

            case PreconditionFailedException preconditionFailedException:
                result.StatusCode = 412;
                result.Message = GetLocalizedMessage("PreconditionFailed");
                if (_options.ShowDetails)
                    result.Details = preconditionFailedException.InnerException?.Message ?? preconditionFailedException.Message;
                break;

            case PayloadTooLargeException payloadTooLargeException:
                result.StatusCode = 413;
                result.Message = GetLocalizedMessage("PayloadTooLarge");
                if (_options.ShowDetails)
                    result.Details = payloadTooLargeException.InnerException?.Message ?? payloadTooLargeException.Message;
                break;

            case UriTooLongException uriTooLongException:
                result.StatusCode = 414;
                result.Message = GetLocalizedMessage("UriTooLong");
                if (_options.ShowDetails)
                    result.Details = uriTooLongException.InnerException?.Message ?? uriTooLongException.Message;
                break;

            case UnsupportedMediaTypeException unsupportedMediaTypeException:
                result.StatusCode = 415;
                result.Message = GetLocalizedMessage("UnsupportedMediaType");
                if (_options.ShowDetails)
                    result.Details = unsupportedMediaTypeException.InnerException?.Message ?? unsupportedMediaTypeException.Message;
                break;

            case RangeNotSatisfiableException rangeNotSatisfiableException:
                result.StatusCode = 416;
                result.Message = GetLocalizedMessage("RangeNotSatisfiable");
                if (_options.ShowDetails)
                    result.Details = rangeNotSatisfiableException.InnerException?.Message ?? rangeNotSatisfiableException.Message;
                break;

            case ExpectationFailedException expectationFailedException:
                result.StatusCode = 417;
                result.Message = GetLocalizedMessage("ExpectationFailed");
                if (_options.ShowDetails)
                    result.Details = expectationFailedException.InnerException?.Message ?? expectationFailedException.Message;
                break;

            case ImATeapotException imATeapotException:
                result.StatusCode = 418;
                result.Message = GetLocalizedMessage("ImATeapot");
                if (_options.ShowDetails)
                    result.Details = imATeapotException.InnerException?.Message ?? imATeapotException.Message;
                break;

            case AuthenticationTimeoutException authenticationTimeoutException:
                result.StatusCode = 419;
                result.Message = GetLocalizedMessage("AuthenticationTimeout");
                if (_options.ShowDetails)
                    result.Details = authenticationTimeoutException.InnerException?.Message ?? authenticationTimeoutException.Message;
                break;

            case MisdirectedRequestException misdirectedRequestException:
                result.StatusCode = 421;
                result.Message = GetLocalizedMessage("MisdirectedRequest");
                if (_options.ShowDetails)
                    result.Details = misdirectedRequestException.InnerException?.Message ?? misdirectedRequestException.Message;
                break;

            case UnprocessableEntityException unprocessableEntityException:
                result.StatusCode = 422;
                result.Message = GetLocalizedMessage("UnprocessableEntity");
                if (_options.ShowDetails)
                    result.Details = unprocessableEntityException.InnerException?.Message ?? unprocessableEntityException.Message;
                break;

            case LockedException lockedException:
                result.StatusCode = 423;
                result.Message = GetLocalizedMessage("Locked");
                if (_options.ShowDetails)
                    result.Details = lockedException.InnerException?.Message ?? lockedException.Message;
                break;

            case FailedDependencyException failedDependencyException:
                result.StatusCode = 424;
                result.Message = GetLocalizedMessage("FailedDependency");
                if (_options.ShowDetails)
                    result.Details = failedDependencyException.InnerException?.Message ?? failedDependencyException.Message;
                break;

            case UpgradeRequiredException upgradeRequiredException:
                result.StatusCode = 426;
                result.Message = GetLocalizedMessage("UpgradeRequired");
                if (_options.ShowDetails)
                    result.Details = upgradeRequiredException.InnerException?.Message ?? upgradeRequiredException.Message;
                break;

            case PreconditionRequiredException preconditionRequiredException:
                result.StatusCode = 428;
                result.Message = GetLocalizedMessage("PreconditionRequired");
                if (_options.ShowDetails)
                    result.Details = preconditionRequiredException.InnerException?.Message ?? preconditionRequiredException.Message;
                break;

            case RequestHeaderFieldsTooLargeException requestHeaderFieldsTooLargeException:
                result.StatusCode = 431;
                result.Message = GetLocalizedMessage("RequestHeaderFieldsTooLarge");
                if (_options.ShowDetails)
                    result.Details = requestHeaderFieldsTooLargeException.InnerException?.Message ?? requestHeaderFieldsTooLargeException.Message;
                break;

            case UnavailableForLegalReasonsException unavailableForLegalReasonsException:
                result.StatusCode = 451;
                result.Message = GetLocalizedMessage("UnavailableForLegalReasons");
                if (_options.ShowDetails)
                    result.Details = unavailableForLegalReasonsException.InnerException?.Message ?? unavailableForLegalReasonsException.Message;
                break;

            case ClientClosedRequestException clientClosedRequestException:
                result.StatusCode = 499;
                result.Message = GetLocalizedMessage("ClientClosedRequest");
                if (_options.ShowDetails)
                    result.Details = clientClosedRequestException.InnerException?.Message ?? clientClosedRequestException.Message;
                break;

            case NotImplementedHttpException notImplementedHttpException:
                result.StatusCode = 501;
                result.Message = GetLocalizedMessage("NotImplemented");
                if (_options.ShowDetails)
                    result.Details = notImplementedHttpException.InnerException?.Message ?? notImplementedHttpException.Message;
                break;

            case BadGatewayException badGatewayException:
                result.StatusCode = 502;
                result.Message = GetLocalizedMessage("BadGateway");
                if (_options.ShowDetails)
                    result.Details = badGatewayException.InnerException?.Message ?? badGatewayException.Message;
                break;

            case ServiceUnavailableException serviceUnavailableException:
                result.StatusCode = 503;
                result.Message = GetLocalizedMessage("ServiceUnavailable");
                if (_options.ShowDetails)
                    result.Details = serviceUnavailableException.InnerException?.Message ?? serviceUnavailableException.Message;
                break;

            case GatewayTimeoutException gatewayTimeoutException:
                result.StatusCode = 504;
                result.Message = GetLocalizedMessage("GatewayTimeout");
                if (_options.ShowDetails)
                    result.Details = gatewayTimeoutException.InnerException?.Message ?? gatewayTimeoutException.Message;
                break;

            case HttpVersionNotSupportedException httpVersionNotSupportedException:
                result.StatusCode = 505;
                result.Message = GetLocalizedMessage("HttpVersionNotSupported");
                if (_options.ShowDetails)
                    result.Details = httpVersionNotSupportedException.InnerException?.Message ?? httpVersionNotSupportedException.Message;
                break;

            case VariantAlsoNegotiatesException variantAlsoNegotiatesException:
                result.StatusCode = 506;
                result.Message = GetLocalizedMessage("VariantAlsoNegotiates");
                if (_options.ShowDetails)
                    result.Details = variantAlsoNegotiatesException.InnerException?.Message ?? variantAlsoNegotiatesException.Message;
                break;

            case InsufficientStorageException insufficientStorageException:
                result.StatusCode = 507;
                result.Message = GetLocalizedMessage("InsufficientStorage");
                if (_options.ShowDetails)
                    result.Details = insufficientStorageException.InnerException?.Message ?? insufficientStorageException.Message;
                break;

            case LoopDetectedException loopDetectedException:
                result.StatusCode = 500;
                result.Message = GetLocalizedMessage("LoopDetected");
                if (_options.ShowDetails)
                    result.Details = loopDetectedException.InnerException?.Message ?? loopDetectedException.Message;
                break;

            case NotExtendedException notExtendedException:
                result.StatusCode = 510;
                result.Message = GetLocalizedMessage("NotExtended");
                if (_options.ShowDetails)
                    result.Details = notExtendedException.InnerException?.Message ?? notExtendedException.Message;
                break;

            case NetworkAuthenticationRequiredException networkAuthenticationRequiredException:
                result.StatusCode = 511;
                result.Message = GetLocalizedMessage("NetworkAuthenticationRequired");
                if (_options.ShowDetails)
                    result.Details = networkAuthenticationRequiredException.InnerException?.Message ?? networkAuthenticationRequiredException.Message;
                break;

            default:
                result.Message = GetLocalizedMessage("Unhandled");
                break;
        }

        return result;
    }
}


