namespace Users.Application.Users.Commands.Login;

internal sealed record LoginResponseDto(string Token, Guid UserId, string Name, string Email);
