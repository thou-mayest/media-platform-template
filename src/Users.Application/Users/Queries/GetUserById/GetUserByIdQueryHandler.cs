using SharedKernal.Messaging;
using SharedKernal.Results;
using Users.Application.Abstractions;

namespace Users.Application.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, Result<UserDto>>
{
    public async Task<Result<UserDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
        {
            return Error.NotFound(ErrorCodes.NotFound, $"user list is empty");
        }

        return user.ToDto();
    }
}