using SharedKernal.Messaging;

namespace Users.Application.Users.Commands.Login;

internal sealed record LoginCommand(string Email, string Password) : ICommand<LoginResult>;

internal sealed record LoginResult(string AccessToken, UserDto User);
