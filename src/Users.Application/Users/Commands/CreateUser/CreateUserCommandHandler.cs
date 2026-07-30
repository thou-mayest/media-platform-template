using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;
using SharedKernal.Results;
namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var normalizedEmail = request.Email.Trim().ToLowerInvariant();
        if (await userRepository.EmailExistsAsync(normalizedEmail, cancellationToken: cancellationToken))
            return Error.Conflict("User.EmailExists", "A user with that email already exists.");

        Result<User> result = User.Create(request.Name, request.Email, request.Password, request.Role, passwordHasher);

        if (result.IsFailure)
            return result.Error;

        var user = result.Value;
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}
