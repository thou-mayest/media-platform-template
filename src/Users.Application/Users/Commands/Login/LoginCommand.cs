using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Users.Application.Users.Commands.Login;

internal sealed record LoginCommand(string Email, string Password) : ICommand<Result<LoginResponseDto>>;
