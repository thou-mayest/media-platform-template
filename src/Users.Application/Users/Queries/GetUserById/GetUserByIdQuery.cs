using SharedKernal.Messaging;

namespace Users.Application.Users.Queries.GetUserById;

internal sealed record GetUserByIdQuery(Guid Id) : IQuery<UserDto?>;
