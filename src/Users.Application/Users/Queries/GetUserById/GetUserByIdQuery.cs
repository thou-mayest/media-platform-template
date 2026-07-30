using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Users.Application.Users.Queries.GetUserById;

internal sealed record GetUserByIdQuery(Guid Id) : IQuery<Result<UserDto>>;
