using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;
using Users.Domain.Abstractions;

namespace Users.Application.Users.Commands.UpdateUser;

internal sealed class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher passwordHasher)
    : ICommandHandler<UpdateUserCommand, Result>
{
    public async Task<Result> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

       
        if (user is null)
            return Result.Failure(Error.NotFound("User.NotFound", $"User '{request.Id}' was not found."));

        var profileResult = user.UpdateProfile(request.Name, request.Email);
        if (profileResult.IsFailure)
            return profileResult;

        if (!string.IsNullOrWhiteSpace(request.Password))
        {
            var passwordResult = user.ChangePassword(request.Password, passwordHasher);
            if (passwordResult.IsFailure)
                return passwordResult;
        }

        user.ChangeRole(request.Role);

        userRepository.Update(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
