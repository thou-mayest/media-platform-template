using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;

namespace Users.Application.Users.Commands.DeleteUser;

internal sealed class DeleteUserCommandHandler(IUserRepository userRepository)
    : ICommandHandler<DeleteUserCommand, Result>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        // Return a structured NotFound error instead of throwing —
        // the caller decides how to translate it into an HTTP response.
        if (user is null)
            return Result.Failure(Error.NotFound("User.NotFound", $"User '{request.Id}' was not found."));

        user.Delete();
        userRepository.Remove(user);
        await userRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}
