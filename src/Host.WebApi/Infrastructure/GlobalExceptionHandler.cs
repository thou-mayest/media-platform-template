using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Host.WebApi.Infrastructure;

public sealed class GlobalExceptionHandler(
    IProblemDetailsService problemDetailsService,
    ILogger<GlobalExceptionHandler> logger)
    : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var (statusCode, title, detail) = exception switch
        {
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "Unauthorized", exception.Message),
            KeyNotFoundException => (StatusCodes.Status404NotFound, "Resource not found", exception.Message),
            ArgumentException => (StatusCodes.Status400BadRequest, "Invalid request", exception.Message),
            InvalidOperationException => (StatusCodes.Status409Conflict, "Operation could not be completed", exception.Message),
            BadHttpRequestException => (StatusCodes.Status400BadRequest, "Malformed request", exception.Message),
            _ => (StatusCodes.Status500InternalServerError, "Unexpected server error", "An unexpected error occurred.")
        };

        logger.LogError(exception, "Request failed with status code {StatusCode}", statusCode);

        httpContext.Response.StatusCode = statusCode;

        return await problemDetailsService.TryWriteAsync(new ProblemDetailsContext
        {
            HttpContext = httpContext,
            ProblemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = detail
            },
            Exception = exception
        });
    }
}
