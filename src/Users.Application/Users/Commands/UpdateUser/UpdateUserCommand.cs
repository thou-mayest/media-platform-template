using SharedKernal.Messaging;

namespace Users.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    string Role) : ICommand;
