using SharedKernal.Messaging;

namespace Users.Application.Users.Commands.CreateUser;

public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password,
    string Role) : ICommand<Guid>;
