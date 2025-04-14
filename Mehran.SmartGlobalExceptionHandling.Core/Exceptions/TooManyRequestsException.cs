namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای تعداد زیاد درخواست
/// </summary>
/// <param name="message"></param>
public class TooManyRequestsException() : Exception("TooManyRequests") { }
