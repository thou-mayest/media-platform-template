using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Commands.DeleteUser;
using Users.Application.Users.Commands.UpdateUser;
using Users.Application.Users.Queries.GetAllUsers;
using Users.Application.Users.Queries.GetUserById;

namespace Users.Presentation.Users;

public static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
            Results.Ok(await sender.Send(new GetAllUsersQuery(), ct)));

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var user = await sender.Send(new GetUserByIdQuery(id), ct);
            return user is null ? Results.NotFound() : Results.Ok(user);
        });

        group.MapPost("/", async (CreateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var id = await sender.Send(
                new CreateUserCommand(request.Name, request.Email, request.Password, request.Role),
                ct);

            return Results.CreatedAtRoute(null, new { id }, new { id });
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(
                new UpdateUserCommand(id, request.Name, request.Email, request.Password, request.Role),
                ct);

            return Results.NoContent();
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            await sender.Send(new DeleteUserCommand(id), ct);
            return Results.NoContent();
        });

        return app;
    }


    // extract into separate files later
    public sealed record CreateUserRequest(string Name, string Email, string Password, string Role);

    public sealed record UpdateUserRequest(string Name, string Email, string Password, string Role);
}
