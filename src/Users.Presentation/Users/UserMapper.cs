using Riok.Mapperly.Abstractions;
using Users.Application.Users;
using Users.Application.Users.Commands.CreateUser;
using Users.Application.Users.Commands.UpdateUser;

namespace Users.Presentation.Users;

[Mapper]
internal static partial class UserMapper
{
    internal static CreateUserCommand ToCommand(this CreateUserRequest request) =>
        new(request.Name!, request.Email!, request.Password!, request.Role!.Value);

    internal static UpdateUserCommand ToCommand(this UpdateUserRequest request, Guid id) =>
        new(id, request.Name!, request.Email!, request.Password, request.Role);

    // Mapperly-generated — fields match exactly
    internal static partial UserResponse ToResponse(this UserDto dto);
}
