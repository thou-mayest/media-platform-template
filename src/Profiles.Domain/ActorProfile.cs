using Profiles.Domain.DomainEvents;
using Profiles.Domain.ValueObjects;
using SharedKernal.Entities;
using SharedKernal.Results;

namespace Profiles.Domain;

public class ActorProfile : AggregateRoot
{
    public const int MaxDisplayNameLength = 100;
    public const int MaxProfessionLength = 120;
    public const int MaxBioLength = 600;
    public const int MaxAvatarKeyLength = 1024;
    public const int MaxSocialLinks = 6;

   
    public const int MinIndexableBioLength = 80;
    public const int MinIndexableAlbums = 1;

    private readonly List<SocialLink> _socialLinks = [];

    
    public Guid UserId { get; private set; }

    public ProfileSlug Slug { get; private set; }
    public string DisplayName { get; private set; }
    public string Profession { get; private set; }
    public string Bio { get; private set; }
    public string? AvatarStorageKey { get; private set; }

    public IReadOnlyList<SocialLink> SocialLinks => _socialLinks.AsReadOnly();

  
    public int AlbumCount { get; private set; }
    public int MediaCount { get; private set; }
    public int FollowerCount { get; private set; }

    public bool IsPublished { get; private set; }


    public bool IsIndexable { get; private set; }

    private ActorProfile(Guid id, Guid userId, ProfileSlug slug, string displayName)
        : base(id)
    {
        UserId = userId;
        Slug = slug;
        DisplayName = displayName;
        Profession = string.Empty;
        Bio = string.Empty;
    }

    private ActorProfile()
    {
        Slug = null!;
        DisplayName = null!;
        Profession = null!;
        Bio = null!;
    }

    
    public static Result<ActorProfile> Create(Guid userId, string? displayName, string? slug)
    {
        if (userId == Guid.Empty)
            return Result.Failure<ActorProfile>(
                Error.Validation("ActorProfile.UserIdEmpty", "UserId is required."));

        var nameResult = ValidateDisplayName(displayName);
        if (nameResult.IsFailure)
            return Result.Failure<ActorProfile>(nameResult.Errors);

        var slugResult = ProfileSlug.Create(slug);
        if (slugResult.IsFailure)
            return Result.Failure<ActorProfile>(slugResult.Errors);

        var profile = new ActorProfile(
            Guid.NewGuid(), userId, slugResult.Value, displayName!.Trim());

        profile.RaiseDomainEvent(new ActorProfileCreatedDomainEvent(
            profile.Id, profile.UserId, profile.Slug.Value));

        return profile;
    }


    public Result UpdateDetails(string? displayName, string? profession, string? bio)
    {
        var nameResult = ValidateDisplayName(displayName);
        if (nameResult.IsFailure) return nameResult;

        profession = profession?.Trim() ?? string.Empty;
        bio = bio?.Trim() ?? string.Empty;

        if (profession.Length > MaxProfessionLength)
            return Error.Validation("ActorProfile.ProfessionTooLong",
                $"Profession must not exceed {MaxProfessionLength} characters.");

        if (bio.Length > MaxBioLength)
            return Error.Validation("ActorProfile.BioTooLong",
                $"Bio must not exceed {MaxBioLength} characters.");

        DisplayName = displayName!.Trim();
        Profession = profession;
        Bio = bio;
        Touch();
        RecomputeIndexability();
        RaiseUpdatedIfPublished();
        return Result.Success();
    }

    public Result ChangeSlug(string? slug)
    {
        var slugResult = ProfileSlug.Create(slug);
        if (slugResult.IsFailure) return Result.Failure(slugResult.Errors);

        if (slugResult.Value.Equals(Slug)) return Result.Success();

        Slug = slugResult.Value;
        Touch();
        RaiseUpdatedIfPublished();
        return Result.Success();
    }

    public Result SetAvatar(string? storageKey)
    {
        if (string.IsNullOrWhiteSpace(storageKey))
        {
            AvatarStorageKey = null;
            Touch();
            return Result.Success();
        }

        var key = storageKey.Trim();
        if (key.Length > MaxAvatarKeyLength)
            return Error.Validation("ActorProfile.AvatarKeyTooLong",
                $"Avatar key must not exceed {MaxAvatarKeyLength} characters.");

        AvatarStorageKey = key;
        Touch();
        return Result.Success();
    }

   
    public Result SetSocialLink(SocialPlatform platform, string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return RemoveSocialLink(platform);

        var linkResult = SocialLink.Create(platform, url);
        if (linkResult.IsFailure) return Result.Failure(linkResult.Errors);

        var existing = _socialLinks.FindIndex(l => l.Platform == platform);
        if (existing >= 0)
        {
            _socialLinks[existing] = linkResult.Value;
        }
        else
        {
            if (_socialLinks.Count >= MaxSocialLinks)
                return Error.Validation("ActorProfile.TooManySocialLinks",
                    $"A profile may have at most {MaxSocialLinks} social links.");

            _socialLinks.Add(linkResult.Value);
        }

        Touch();
        return Result.Success();
    }

    public Result RemoveSocialLink(SocialPlatform platform)
    {
        if (_socialLinks.RemoveAll(l => l.Platform == platform) > 0) Touch();
        return Result.Success();
    }

  
    public Result Publish()
    {
        if (IsPublished)
            return Error.Conflict("ActorProfile.AlreadyPublished", "Profile is already published.");

        if (string.IsNullOrWhiteSpace(Profession))
            return Error.Validation("ActorProfile.ProfessionRequired",
                "A profession is required before publishing.");

        if (string.IsNullOrWhiteSpace(Bio))
            return Error.Validation("ActorProfile.BioRequired",
                "A bio is required before publishing.");

        IsPublished = true;
        Touch();
        RecomputeIndexability();
        RaiseDomainEvent(new ActorProfilePublishedDomainEvent(
            Id, UserId, Slug.Value, DisplayName));

        return Result.Success();
    }

    public Result Unpublish()
    {
        if (!IsPublished)
            return Error.Conflict("ActorProfile.NotPublished", "Profile is not published.");

        IsPublished = false;
        Touch();
        RecomputeIndexability();
        return Result.Success();
    }

    public void Delete() => RaiseDomainEvent(new ActorProfileDeletedDomainEvent(Id));

    public void AdjustAlbumCount(int albumDelta, int mediaDelta)
    {
        AlbumCount = Math.Max(0, AlbumCount + albumDelta);
        MediaCount = Math.Max(0, MediaCount + mediaDelta);
        Touch();
        RecomputeIndexability();
    }

  
    public void AdjustFollowerCount(int delta)
    {
        FollowerCount = Math.Max(0, FollowerCount + delta);
    }

    private static Result ValidateDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return Error.Validation("ActorProfile.DisplayNameEmpty", "Display name cannot be empty.");

        if (displayName.Trim().Length > MaxDisplayNameLength)
            return Error.Validation("ActorProfile.DisplayNameTooLong",
                $"Display name must not exceed {MaxDisplayNameLength} characters.");

        return Result.Success();
    }


    private void Touch() => UpdateDate = DateTime.UtcNow;


    private void RecomputeIndexability() =>
        IsIndexable = IsPublished
            && AlbumCount >= MinIndexableAlbums
            && Bio.Length >= MinIndexableBioLength;

  
    private void RaiseUpdatedIfPublished()
    {
        if (IsPublished)
            RaiseDomainEvent(new ActorProfileUpdatedDomainEvent(Id, Slug.Value, DisplayName));
    }
}