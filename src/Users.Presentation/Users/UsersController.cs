using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SharedKernal.Extensions;
using Users.Application.Users.Commands.DeleteUser;
using Users.Application.Users.Commands.UpdateUser;
using Users.Application.Users.Queries.GetAllUsers;
using Users.Application.Users.Queries.GetUserById;
using Users.Presentation.Authorization;

namespace Users.Presentation.Users;

[ApiController]
[Route("api/users")]
[Authorize]
public sealed class UsersController(ISender sender) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = UsersPolicies.RequireAdmin)]
    public async Task<IActionResult> GetAll(CancellationToken ct)
    {
        var result = await sender.Send(new GetAllUsersQuery(), ct);

        return result.Match(
            users => Ok(users.Select(u => u.ToResponse()))
        );
    }

    [HttpGet("{id:guid}")]
    [Authorize(Policy = UsersPolicies.CanManageUsers)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new GetUserByIdQuery(id), ct);

        return result.Match(
            user => Ok(user.ToResponse())
        );
    }

    [HttpPost]
    [Authorize(Policy = UsersPolicies.RequireAdmin)]
    public async Task<IActionResult> Create([FromBody] CreateUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(), ct);

        return result.Match(
            id => Created($"/api/users/{id}", new { id })
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(Policy = UsersPolicies.CanManageUsers)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUserRequest request, CancellationToken ct)
    {
        var result = await sender.Send(request.ToCommand(id), ct);

        return result.Match(
            _ => (IActionResult)NoContent()
        );
    }

    [HttpPut("{id:guid}/role")]
    [Authorize(Policy = UsersPolicies.RequireAdmin)]
    public async Task<IActionResult> ChangeRole(
        Guid id,
        [FromBody] ChangeUserRoleRequest request,
        CancellationToken ct)
    {
        var userResult = await sender.Send(new GetUserByIdQuery(id), ct);
        if (userResult.IsFailure)
        {
            return userResult.Match(user => Ok(user.ToResponse()));
        }

        var current = userResult.Value;
        var result = await sender.Send(
            new UpdateUserCommand(id, current.Name, current.Email, null, request.Role!.Value),
            ct);
        return result.Match(_ => (IActionResult)NoContent());
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Policy = UsersPolicies.RequireAdmin)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken ct)
    {
        var result = await sender.Send(new DeleteUserCommand(id), ct);

        return result.Match(
            _ => (IActionResult)NoContent()
        );
    }
}
