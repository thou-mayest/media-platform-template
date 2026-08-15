using Profiles.Application.Abstractions;
using SharedKernal.Messaging;
using SharedKernal.Results;

namespace Profiles.Application.ActorProfiles.Commands.UpdateActorProfile;

internal sealed class UpdateActorProfileCommandHandler(IActorProfileRepository repository)
    : ICommandHandler<UpdateActorProfileCommand, Result<ActorProfileDto>>
{
    public async Task<Result<ActorProfileDto>> Handle(
        UpdateActorProfileCommand request, CancellationToken cancellationToken)
    {
        var profile = await repository.GetByIdAsync(request.ProfileId, cancellationToken);

        if (profile is null)
            return Error.NotFound("ActorProfile.NotFound", "Profile not found.");

        
        var errors = new List<Error>();

        var details = profile.UpdateDetails(request.DisplayName, request.Profession, request.Bio);
        if (details.IsFailure)
            errors.AddRange(details.Errors);

        if (!string.IsNullOrWhiteSpace(request.Slug))
        {
            var slug = request.Slug.Trim().ToLowerInvariant();

            if (slug != profile.Slug.Value)
            {
                if (await repository.SlugExistsAsync(slug, cancellationToken))
                {
                    errors.Add(Error.Conflict("ActorProfile.SlugTaken", $"'{slug}' is already taken."));
                }
                else
                {
                    var changed = profile.ChangeSlug(slug);
                    if (changed.IsFailure)
                        errors.AddRange(changed.Errors);
                }
            }
        }

        var avatar = profile.SetAvatar(request.AvatarStorageKey);
        if (avatar.IsFailure)
            errors.AddRange(avatar.Errors);

        foreach (var link in request.SocialLinks ?? [])
        {
            var applied = profile.SetSocialLink(link.Platform, link.Url);
            if (applied.IsFailure)
                errors.AddRange(applied.Errors);
        }

        if (errors.Count > 0)
            return Result.Failure<ActorProfileDto>(errors);

        repository.Update(profile);
        await repository.SaveChangesAsync(cancellationToken);
        return profile.ToDto();
    }
}