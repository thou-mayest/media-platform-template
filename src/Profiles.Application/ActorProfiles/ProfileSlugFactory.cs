using Profiles.Application.Abstractions;
using Profiles.Domain.ValueObjects;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles;

internal static class ProfileSlugFactory
{
    private const string Fallback = "profile";
    private const int MaxAttempts = 25;
    private const int SuffixBudget = 4;   // room for "-25"

    public static async Task<Result<string>> CreateUniqueAsync(
        IActorProfileRepository repository,
        string? displayName,
        CancellationToken cancellationToken)
    {
        var root = string.IsNullOrWhiteSpace(displayName)
            ? Fallback
            : ProfileSlug.Slugify(displayName);

        if (root.Length < ProfileSlug.MinLength)
            root = Fallback;

        if (root.Length > ProfileSlug.MaxLength - SuffixBudget)
            root = root[..(ProfileSlug.MaxLength - SuffixBudget)].TrimEnd('-');

        for (var attempt = 1; attempt <= MaxAttempts; attempt++)
        {
            var candidate = attempt == 1 ? root : $"{root}-{attempt}";

            if (!ProfileSlug.IsValid(candidate))
                continue;

            if (!await repository.SlugExistsAsync(candidate, cancellationToken))
                return candidate;
        }

        return Error.Conflict(
            "ActorProfile.SlugUnavailable",
            "Could not derive an available slug. Choose one manually.");
    }
}