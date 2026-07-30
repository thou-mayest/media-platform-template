using System.ComponentModel.DataAnnotations;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.Commands.Login;
using Users.Presentation.Validation;

namespace Users.Presentation.Users;

internal static class AuthEndpoints
{
    public static IEndpointRouteBuilder MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/auth/login", async (
                LoginRequest request,
                ISender sender,
                CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(
                    new LoginCommand(request.Email!, request.Password!),
                    cancellationToken);

                return Results.Ok(new LoginResponse(
                    result.AccessToken,
                    result.User.ToResponse()));
            })
            .AllowAnonymous()
            .RequireRateLimiting("login")
            .WithTags("Authentication")
            .AddEndpointFilter<ValidationFilter<LoginRequest>>();

        return app;
    }

    public sealed record LoginRequest(
        [property: Required, StringLength(200), EmailAddress] string? Email,
        [property: Required, StringLength(128, MinimumLength = 8)] string? Password);

    public sealed record LoginResponse(string AccessToken, UserResponse User);
}
