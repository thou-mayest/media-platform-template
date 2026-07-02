using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Domain.Abstractions;
namespace Users.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<UpdateUserCommand>
{
    public async Task Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new KeyNotFoundException($"User '{request.Id}' was not found.");

        var profileResult = user.UpdateProfile(request.Name, request.Email);
        if (profileResult.IsFailure)
            throw new InvalidOperationException(string.Join("; ", profileResult.Errors.Select(e => e.Message)));

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordResult = user.ChangePassword(request.Password, passwordHasher);
            if (passwordResult.IsFailure)
                throw new InvalidOperationException(string.Join("; ", passwordResult.Errors.Select(e => e.Message)));
        }

        user.ChangeRole(request.Role);

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);
    }
}