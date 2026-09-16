using Microsoft.Extensions.Options;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;

namespace Users.Infrastracture.Seeding;

internal sealed class UserSeeder(
    IUserRepository userRepository,
    IPasswordHasher passwordHasher,
    IOptions<UserSeedOptions> options)
{
    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        if (!options.Value.Enabled || options.Value.Users.Count == 0)
            return;

        var processedEmails = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var usersAdded = false;

        foreach (var definition in options.Value.Users)
        {
            var normalizedEmail = definition.Email.Trim().ToLowerInvariant();

            if (!processedEmails.Add(normalizedEmail) ||
                await userRepository.GetByEmailAsync(normalizedEmail, cancellationToken) is not null)
            {
                continue;
            }

            var result = User.Create(
                definition.Name,
                normalizedEmail,
                definition.Password,
                definition.Role,
                passwordHasher);

            if (result.IsFailure)
            {
                var errors = string.Join("; ", result.Errors.Select(error => $"{error.Code}: {error.Message}"));
                throw new InvalidOperationException($"Could not seed user '{normalizedEmail}': {errors}");
            }

            await userRepository.AddAsync(result.Value, cancellationToken);
            usersAdded = true;
        }

        if (!usersAdded)
            return;

        var saveResult = await userRepository.SaveChangesAsync(cancellationToken);
        if (saveResult.IsFailure)
        {
            var errors = string.Join("; ", saveResult.Errors.Select(error => $"{error.Code}: {error.Message}"));
            throw new InvalidOperationException($"Could not save seeded users: {errors}");
        }
    }
}
