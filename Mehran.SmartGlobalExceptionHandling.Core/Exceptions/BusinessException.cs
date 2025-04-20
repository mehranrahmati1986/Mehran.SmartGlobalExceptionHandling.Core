namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطاهای بیزینسی
/// </summary>
/// <param name="message"></param>
/// <param name="code"></param>
public class BusinessException(string code = null, object metaData = null) : Exception("Business")
{
    /// <summary>
    /// کد خطا
    /// </summary>
    public string Code { get; } = code;

    /// <summary>
    /// دیتای اضافی
    /// </summary>
    public object MetaData { get; } = metaData;
}


/// <summary>
/// HTTP 402 Payment Required
/// </summary>
public class PaymentRequiredException(object metaData = null)
    : Exception("PaymentRequired")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 405 Method Not Allowed
/// </summary>
public class MethodNotAllowedException(object metaData = null)
    : Exception("MethodNotAllowed")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 406 Not Acceptable
/// </summary>
public class NotAcceptableException(object metaData = null)
    : Exception("NotAcceptable")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 407 Proxy Authentication Required
/// </summary>
public class ProxyAuthenticationRequiredException(object metaData = null)
    : Exception("ProxyAuthenticationRequired")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 410 Gone
/// </summary>
public class GoneException(object metaData = null)
    : Exception("Gone")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 411 Length Required
/// </summary>
public class LengthRequiredException(object metaData = null)
    : Exception("LengthRequired")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 412 Precondition Failed
/// </summary>
public class PreconditionFailedException(object metaData = null)
    : Exception("PreconditionFailed")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 413 Payload Too Large
/// </summary>
public class PayloadTooLargeException(object metaData = null)
    : Exception("PayloadTooLarge")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 414 URI Too Long
/// </summary>
public class UriTooLongException(object metaData = null)
    : Exception("UriTooLong")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 415 Unsupported Media Type
/// </summary>
public class UnsupportedMediaTypeException(object metaData = null)
    : Exception("UnsupportedMediaType")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 416 Range Not Satisfiable
/// </summary>
public class RangeNotSatisfiableException(object metaData = null)
    : Exception("RangeNotSatisfiable")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 417 Expectation Failed
/// </summary>
public class ExpectationFailedException(object metaData = null)
    : Exception("ExpectationFailed")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 418 I'm a teapot
/// </summary>
public class ImATeapotException(object metaData = null)
    : Exception("ImATeapot")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 419 Authentication Timeout (non-standard)
/// </summary>
public class AuthenticationTimeoutException(object metaData = null)
    : Exception("AuthenticationTimeout")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 421 Misdirected Request
/// </summary>
public class MisdirectedRequestException(object metaData = null)
    : Exception("MisdirectedRequest")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 422 Unprocessable Entity
/// </summary>
public class UnprocessableEntityException(object metaData = null)
    : Exception("UnprocessableEntity")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 423 Locked
/// </summary>
public class LockedException(object metaData = null)
    : Exception("Locked")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 424 Failed Dependency
/// </summary>
public class FailedDependencyException(object metaData = null)
    : Exception("FailedDependency")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 426 Upgrade Required
/// </summary>
public class UpgradeRequiredException(object metaData = null)
    : Exception("UpgradeRequired")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 428 Precondition Required
/// </summary>
public class PreconditionRequiredException(object metaData = null)
    : Exception("PreconditionRequired")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 431 Request Header Fields Too Large
/// </summary>
public class RequestHeaderFieldsTooLargeException(object metaData = null)
    : Exception("RequestHeaderFieldsTooLarge")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 451 Unavailable For Legal Reasons
/// </summary>
public class UnavailableForLegalReasonsException(object metaData = null)
    : Exception("UnavailableForLegalReasons")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 499 Client Closed Request (NGINX)
/// </summary>
public class ClientClosedRequestException(object metaData = null)
    : Exception("ClientClosedRequest")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 501 Not Implemented
/// </summary>
public class NotImplementedHttpException(object metaData = null)
    : Exception("NotImplemented")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 502 Bad Gateway
/// </summary>
public class BadGatewayException(object metaData = null)
    : Exception("BadGateway")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 503 Service Unavailable
/// </summary>
public class ServiceUnavailableException(object metaData = null)
    : Exception("ServiceUnavailable")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 504 Gateway Timeout
/// </summary>
public class GatewayTimeoutException(object metaData = null)
    : Exception("GatewayTimeout")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 505 HTTP Version Not Supported
/// </summary>
public class HttpVersionNotSupportedException(object metaData = null)
    : Exception("HttpVersionNotSupported")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 506 Variant Also Negotiates
/// </summary>
public class VariantAlsoNegotiatesException(object metaData = null)
    : Exception("VariantAlsoNegotiates")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 507 Insufficient Storage
/// </summary>
public class InsufficientStorageException(object metaData = null)
    : Exception("InsufficientStorage")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 508 Loop Detected
/// </summary>
public class LoopDetectedException(object metaData = null)
    : Exception("LoopDetected")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 510 Not Extended
/// </summary>
public class NotExtendedException(object metaData = null)
    : Exception("NotExtended")
{
    public object MetaData { get; } = metaData;
}

/// <summary>
/// HTTP 511 Network Authentication Required
/// </summary>
public class NetworkAuthenticationRequiredException(object metaData = null)
    : Exception("NetworkAuthenticationRequired")
{
    public object MetaData { get; } = metaData;
}
