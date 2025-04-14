﻿namespace Mehran.SmartGlobalExceptionHandling.Core.Models;

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; }
    public List<ValidationError> Errors { get; set; }
    public string TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}