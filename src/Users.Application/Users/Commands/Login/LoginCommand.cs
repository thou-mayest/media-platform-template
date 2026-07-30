using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Users.Application.Users.Commands.Login;

internal sealed record LoginCommand(string Email, string Password) : ICommand<Result<LoginResult>>;

internal sealed record LoginResult(string AccessToken, UserDto User);

internal static class AuthenticationErrors
{
    public static readonly Error InvalidCredentials =
        new("Authentication.InvalidCredentials", "The email or password is incorrect.", ErrorType.Unauthorized);
}
