using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed record DeleteUserCommand(Guid Id) : ICommand<Result<bool>>;
