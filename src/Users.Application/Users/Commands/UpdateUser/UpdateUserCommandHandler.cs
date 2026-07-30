using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Application.Users.Exceptions;
using Users.Domain;

namespace Users.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(
    IUserRepository userRepository,
    IPasswordHashingService passwordHashingService)
    : ICommandHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new UserNotFoundException(request.Id);

        var normalizedEmail = User.NormalizeEmail(request.Email);
        if (await userRepository.EmailExistsAsync(
                normalizedEmail,
                request.Id,
                cancellationToken))
        {
            throw new EmailAlreadyExistsException();
        }

        user.Update(
            request.Name,
            normalizedEmail,
            passwordHashingService.Hash(request.Password),
            request.Role);

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}
