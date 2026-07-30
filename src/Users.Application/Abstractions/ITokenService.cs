using Users.Common;

namespace Users.Application.Abstractions;

internal interface ITokenService
{
    string Create(Guid userId, string name, string email, Role role, Guid version);
}
