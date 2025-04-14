namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای پایان مهلت درخواست
/// </summary>
/// <param name="message"></param>
public class RequestTimeoutException() : Exception("Timeout") { }
