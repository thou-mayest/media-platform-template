using SharedKernal.Messaging;
using Users.Common;

namespace Users.Application.Users.Commands.UpdateUser;

internal sealed record UpdateUserCommand(
    Guid Id,
    string Name,
    string Email,
    string Password,
    Role Role) : ICommand;
