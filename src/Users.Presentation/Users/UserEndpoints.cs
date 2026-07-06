using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Users.Application.Users.Queries.GetAllUsers;
using Users.Application.Users.Queries.GetUserById;
using Users.Application.Users.Commands.DeleteUser;
using Users.Common;

namespace Users.Presentation.Users;

internal static class UserEndpoints
{
    public static IEndpointRouteBuilder MapUserEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/users").WithTags("Users");

        group.MapGet("/", async (ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetAllUsersQuery(), ct);
            return result.IsSuccess 
            ? Results.Ok(result.Value.Select(u => u.ToResponse())) 
            : Results.NotFound(result.Error);
        });

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetUserByIdQuery(id), ct);
            return result.IsSuccess 
            ? Results.Ok(result.Value.ToResponse()) 
            : Results.NotFound(result.Error);
        });

        group.MapPost("/", async (CreateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request.ToCommand(), ct);
            return result.IsSuccess 
            ? Results.Created($"/api/users/{result.Value}", new { id = result.Value }) 
            : Results.BadRequest(result.Error);
        });

        group.MapPut("/{id:guid}", async (Guid id, UpdateUserRequest request, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(request.ToCommand(id), ct);
            return result.IsSuccess 
            ? Results.NoContent() 
            : Results.BadRequest(result.Error);
        });

        group.MapDelete("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new DeleteUserCommand(id), ct);
            return result.IsSuccess 
            ? Results.NoContent() 
            : Results.NotFound(result.Error);
        });

        return app;
    }

    public sealed record CreateUserRequest(string Name, string Email, string Password, Role Role);
    public sealed record UpdateUserRequest(string Name, string Email, string Password, Role Role);
}