using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Application.Users.Exceptions;

namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHashingService passwordHashingService)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = User.NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken: cancellationToken))
        {
            throw new EmailAlreadyExistsException();
        }

        var user = new User(
            request.Name,
            normalizedEmail,
            passwordHashingService.Hash(request.Password),
            request.Role);

        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);

        return user.Id;
    }
}
