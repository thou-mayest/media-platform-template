using SharedKernal.Messaging;
using Users.Domain;

namespace Users.Application.Users.Commands.UpdateUser;

public sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    Role Role) : ICommand;
