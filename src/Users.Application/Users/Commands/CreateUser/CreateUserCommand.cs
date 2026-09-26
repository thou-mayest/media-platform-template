using Users.Common;
using SharedKernal.Results;
using SharedKernal.Messaging.Commands;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    Role Role) : ICommand<Result<Guid>>;
