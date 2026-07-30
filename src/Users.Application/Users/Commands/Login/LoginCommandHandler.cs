using SharedKernal.Messaging;
using Users.Application.Abstractions;
using SharedKernal.Results;
using Users.Domain.Abstractions;

namespace Users.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    ITokenService tokenService) : ICommandHandler<LoginCommand, Result<LoginResult>>
{
    public async Task<Result<LoginResult>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        var user = await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken);

        if (user is null)
        {
            passwordHasher.VerifyDummy(request.Password);
            return AuthenticationErrors.InvalidCredentials;
        }

        if (!passwordHasher.Verify(request.Password, user.Password.HashedValue))
            return AuthenticationErrors.InvalidCredentials;

        var token = tokenService.Create(user.Id, user.Name, user.Email.Value, user.Role, user.Version);
        return new LoginResult(token, user.ToDto());
    }

}
