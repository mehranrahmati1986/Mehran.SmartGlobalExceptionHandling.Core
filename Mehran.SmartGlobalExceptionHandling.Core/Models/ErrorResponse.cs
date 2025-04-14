namespace Mehran.SmartGlobalExceptionHandling.Core.Models;

public class ErrorResponse<T> where T : class
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string Details { get; set; }
    public List<ValidationError> Errors { get; set; }
    public string TraceId { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    public T MetaData { get; set; }
}