namespace Mehran.SmartGlobalExceptionHandling.Core.Enums;

public enum ErrorKey : byte
{
    NotFound = 1,
    Validation = 2,
    Unauthorized = 3,
    Conflict = 4,
    Business = 5,
    Unhandled = 6,
    Forbidden = 7,
    TooManyRequests = 8,
    Timeout = 9,
    InvalidArgument = 10,
    InvalidOperation = 11,
    DbUpdate = 12,
    UnexpectedError = 13,
}
