using SharedKernal.Messaging;
using Users.Domain;

namespace Users.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    Role Role) : ICommand<Guid>;
