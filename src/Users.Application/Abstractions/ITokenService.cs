using Users.Domain;

namespace Users.Application.Abstractions;

internal interface ITokenService
{
    string GenerateToken(User user);
}
