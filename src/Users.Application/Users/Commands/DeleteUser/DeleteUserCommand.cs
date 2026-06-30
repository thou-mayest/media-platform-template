using SharedKernal.Messaging;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed record DeleteUserCommand(Guid Id) : ICommand;
