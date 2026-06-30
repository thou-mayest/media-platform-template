using SharedKernal.Messaging;
using Users.Common;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    Role Role) : ICommand<Guid>;
