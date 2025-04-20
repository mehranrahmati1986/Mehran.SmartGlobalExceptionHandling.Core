using Mehran.SmartGlobalExceptionHandling.Core.Localizations;
using Mehran.SmartGlobalExceptionHandling.Core.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;

namespace Mehran.SmartGlobalExceptionHandling.Core.Middleware.Extensions;

public static class ApiBehaviorOptionsConfiguration
{
    public static IServiceCollection AddCustomApiBehavior(this IServiceCollection services, IErrorMessageLocalizer errorMessageLocalizer)
    {
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState
                    .Where(e => e.Value.Errors.Count > 0)
                    .ToDictionary(
                        kvp => kvp.Key,
                        kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                    );

                var traceId = context.HttpContext.TraceIdentifier;

                var response = new ErrorResponse<object>
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity,
                    Message = errorMessageLocalizer.Get("Validation"),
                    FluentValidationErrors = errors,
                    TraceId = traceId,
                    Timestamp = DateTime.UtcNow
                };

                return new ObjectResult(response)
                {
                    StatusCode = StatusCodes.Status422UnprocessableEntity
                };
            };
        });

        return services;
    }
}