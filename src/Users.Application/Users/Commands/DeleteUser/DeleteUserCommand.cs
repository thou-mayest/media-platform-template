using SharedKernal.Messaging.Commands;
using SharedKernal.Results;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed record DeleteUserCommand(Guid Id) : ICommand<Result<bool>>;
