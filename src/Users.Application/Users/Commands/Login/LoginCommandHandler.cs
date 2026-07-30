using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Application.Users.Exceptions;
using Users.Domain;

namespace Users.Application.Users.Commands.Login;

internal sealed class LoginCommandHandler(
    IUserRepository userRepository,
    IPasswordHashingService passwordHashingService,
    ITokenService tokenService) : ICommandHandler<LoginCommand, LoginResult>
{
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByEmailAsync(
            User.NormalizeEmail(request.Email),
            cancellationToken);

        if (user is null)
        {
            passwordHashingService.VerifyDummy(request.Password);
            throw new InvalidCredentialsException();
        }

        if (!passwordHashingService.Verify(user.PasswordHash, request.Password))
        {
            throw new InvalidCredentialsException();
        }

        var token = tokenService.Create(user.Id, user.Name, user.Email, user.Role, user.Version);
        return new LoginResult(token, user.ToDto());
    }
}
