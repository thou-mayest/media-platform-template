using Microsoft.AspNetCore.Diagnostics;
using Users.Application.Users.Exceptions;

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
            UserNotFoundException => (StatusCodes.Status404NotFound, "User not found", exception.Message),
            EmailAlreadyExistsException => (StatusCodes.Status409Conflict, "Email already exists", exception.Message),
            InvalidCredentialsException => (StatusCodes.Status401Unauthorized, "Invalid credentials", exception.Message),
            UserConcurrencyException => (StatusCodes.Status409Conflict, "User changed", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Server error", "An unexpected error occurred.")
        };

        if (status == StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled request exception");
        }

        await Results.Problem(statusCode: status, title: title, detail: detail)
            .ExecuteAsync(httpContext);
        return true;
    }
}
