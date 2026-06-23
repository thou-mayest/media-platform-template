using SharedKernal.Messaging;

namespace Users.Application.Users.Commands.DeleteUser;

public sealed record DeleteUserCommand(Guid Id) : ICommand;
