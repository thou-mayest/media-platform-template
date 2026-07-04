using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;
using Users.Common;
namespace Users.Application.Users.Commands.CreateUser;

internal sealed class CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<CreateUserCommand, Guid>
{
    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        var result = User.Create(request.Name, request.Email, request.Password, request.Role, passwordHasher);

        if (result.IsFailure)
            throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Message)));

        var user = result.Value;
        await userRepository.AddAsync(user, cancellationToken);
        await userRepository.SaveChangesAsync(cancellationToken);
        return user.Id;
    }
}