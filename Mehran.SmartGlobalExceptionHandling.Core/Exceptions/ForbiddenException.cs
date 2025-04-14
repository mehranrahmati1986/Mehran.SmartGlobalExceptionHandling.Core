namespace Mehran.SmartGlobalExceptionHandling.Core.Exceptions;

/// <summary>
/// خطای دسترسی غیرمجاز
/// </summary>
/// <param name="message"></param>
public class ForbiddenException() : Exception("Forbidden") { }
