using Microsoft.AspNetCore.Diagnostics;
using Users.Infrastracture.Persistence;

namespace Host.WebApi;

internal sealed class ApiExceptionHandler(ILogger<ApiExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (status, title, detail) = exception switch
        {
            UserEmailConflictException => (StatusCodes.Status409Conflict, "Email already exists", exception.Message),
            UserConcurrencyException => (StatusCodes.Status409Conflict, "User changed", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
        };
        if (status == StatusCodes.Status500InternalServerError)
            logger.LogError(exception, "Unhandled request exception");

        await Results.Problem(statusCode: status, title: title, detail: detail).ExecuteAsync(httpContext);
        return true;
    }
}
