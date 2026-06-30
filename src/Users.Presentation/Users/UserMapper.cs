using Riok.Mapperly.Abstractions;
using Users.Application.Users;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Commands.UpdateUser;
using static Users.Presentation.Users.UserEndpoints;

namespace Users.Presentation.Users;

[Mapper]
internal static partial class UserMapper
{
    // Mapperly-generated — fields match exactly
    internal static partial CreateUserCommand ToCommand(this CreateUserRequest request);

    // Manual — Id comes from the route, not the request body
    internal static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid id)
        => new(id, request.Name, request.Email, request.Password, request.Role);

    // Mapperly-generated — fields match exactly
    internal static partial UserResponse ToResponse(this UserDto dto);
}