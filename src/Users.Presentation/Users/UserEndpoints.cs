using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.Queries.GetAllUsers;
using Users.Application.Users.Queries.GetUserById;
using Users.Application.Users.Commands.DeleteUser;
using Users.Common;
using System.ComponentModel.DataAnnotations;
using Users.Presentation.Validation;

namespace Users.Presentation.Users;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users")
            .WithTags("Users")
            .RequireAuthorization("Admin");

        group.MapGet("/", async (int? page, int? pageSize, ISender sender, CancellationToken ct) =>
        {
            var requestedPage = page ?? 1;
            var requestedPageSize = pageSize ?? 50;
            if (requestedPage is < 1 or > 1_000_000 || requestedPageSize is < 1 or > 100)
            {
                return Results.ValidationProblem(new Dictionary<string, string[]>
                {
                    ["pagination"] = ["Page must be between 1 and 1,000,000 and pageSize must be between 1 and 100."]
                });
            }

            var users = await sender.Send(
                new GetAllUsersQuery(requestedPage, requestedPageSize),
                ct);
            return Results.Ok(users.Select(user => user.ToResponse()));
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var user = await sender.Send(new GetUserByIdQuery(id), ct);
            return Results.Ok(user.ToResponse());
        });

        group.MapPost("/", async (CreateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var id = await sender.Send(request.ToCommand(), ct);
            return Results.Created($"/api/users/{id}", new { id });
        }).AddEndpointFilter<ValidationFilter<CreateUserRequest>>();

        group.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(request.ToCommand(id), ct);
            return Results.NoContent();
        }).AddEndpointFilter<ValidationFilter<UpdateUserRequest>>();

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteUserCommand(id), ct);
            return Results.NoContent();
        });

        return app;
    }

    public sealed record CreateUserRequest(
        [property: Required, StringLength(200, MinimumLength = 1)] string? Name,
        [property: Required, StringLength(200), EmailAddress] string? Email,
        [property: Required, StringLength(128, MinimumLength = 8)] string? Password,
        [property: Required, EnumDataType(typeof(Role))] Role? Role);

    public sealed record UpdateUserRequest(
        [property: Required, StringLength(200, MinimumLength = 1)] string? Name,
        [property: Required, StringLength(200), EmailAddress] string? Email,
        [property: Required, StringLength(128, MinimumLength = 8)] string? Password,
        [property: Required, EnumDataType(typeof(Role))] Role? Role);
}
