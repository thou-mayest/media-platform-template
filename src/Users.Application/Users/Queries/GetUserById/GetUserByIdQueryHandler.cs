using SharedKernal.Messaging;
using Users.Application.Abstractions;
using Users.Application.Users.Exceptions;

namespace Users.Application.Users.Queries.GetUserById;

internal sealed class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, UserDto>
{
    public async Task<UserDto> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var user = await userRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new UserNotFoundException(request.Id);

        return user.ToDto();
    }
}
