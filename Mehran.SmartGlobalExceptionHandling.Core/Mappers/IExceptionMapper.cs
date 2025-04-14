using Mehran.SmartGlobalExceptionHandling.Core.Models;

namespace Mehran.SmartGlobalExceptionHandling.Core.Mappers;

public interface IExceptionMapper
{
    ErrorResponse Map(Exception ex);
}