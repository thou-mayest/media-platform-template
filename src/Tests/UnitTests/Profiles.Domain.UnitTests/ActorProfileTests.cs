using Profiles.Domain.DomainEvents;
using Profiles.Domain.ValueObjects;

namespace Profiles.Domain.UnitTests;

public class ActorProfileTests
{
    private const string ValidBio =
        "Saturated, sun-drunk images of coastal life — analog portraits, night "
        + "markets and long-exposure seascapes shot across the Mediterranean.";

    private static ActorProfile NewDraft(string displayName = "Mara Solano", string slug = "mara-solano")
        => ActorProfile.Create(Guid.NewGuid(), displayName, slug).Value;

    /// <summary>A profile that satisfies every publish precondition.</summary>
    private static ActorProfile NewCompleteDraft()
    {
        var profile = NewDraft();
        profile.UpdateDetails("Mara Solano", "Photographer", ValidBio);
        return profile;
    }

    // ── CREATE ───────────────────────────────────────────────────

    [Fact]
    public void Create_ProducesAnUnpublishedDraft()
    {
        var profile = NewDraft();

        Assert.False(profile.IsPublished);
        Assert.False(profile.IsIndexable);
        Assert.Empty(profile.Profession);
        Assert.Empty(profile.Bio);
        Assert.Empty(profile.SocialLinks);
    }

    [Fact]
    public void Create_RaisesCreatedDomainEvent()
    {
        var profile = NewDraft();

        var raised = Assert.Single(profile.DomainEvents);
        Assert.IsType<ActorProfileCreatedDomainEvent>(raised);
    }

    [Fact]
    public void Create_WithEmptyUserId_ReturnsFailure()
    {
        var result = ActorProfile.Create(Guid.Empty, "Mara Solano", "mara-solano");

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.UserIdEmpty", result.Error.Code);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyDisplayName_ReturnsFailure(string? displayName)
    {
        var result = ActorProfile.Create(Guid.NewGuid(), displayName, "mara-solano");

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.DisplayNameEmpty", result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidSlug_ReturnsFailure()
    {
        var result = ActorProfile.Create(Guid.NewGuid(), "Admin", "admin");

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.Reserved", result.Error.Code);
    }

    // ── PUBLISH ──────────────────────────────────────────────────

    [Fact]
    public void Publish_WithoutProfession_ReturnsFailure()
    {
        var profile = NewDraft();
        profile.UpdateDetails("Mara Solano", null, ValidBio);

        var result = profile.Publish();

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.ProfessionRequired", result.Error.Code);
        Assert.False(profile.IsPublished);
    }

    [Fact]
    public void Publish_WithoutBio_ReturnsFailure()
    {
        var profile = NewDraft();
        profile.UpdateDetails("Mara Solano", "Photographer", null);

        var result = profile.Publish();

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.BioRequired", result.Error.Code);
        Assert.False(profile.IsPublished);
    }

    [Fact]
    public void Publish_WhenComplete_Succeeds()
    {
        var profile = NewCompleteDraft();

        var result = profile.Publish();

        Assert.True(result.IsSuccess);
        Assert.True(profile.IsPublished);
    }

    [Fact]
    public void Publish_RaisesPublishedDomainEvent()
    {
        var profile = NewCompleteDraft();
        profile.ClearDomainEvents();

        profile.Publish();

        var raised = Assert.Single(profile.DomainEvents);
        Assert.IsType<ActorProfilePublishedDomainEvent>(raised);
    }

    [Fact]
    public void Publish_WhenAlreadyPublished_ReturnsConflict()
    {
        var profile = NewCompleteDraft();
        profile.Publish();

        var result = profile.Publish();

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.AlreadyPublished", result.Error.Code);
    }

    [Fact]
    public void Unpublish_WhenNotPublished_ReturnsConflict()
    {
        var profile = NewCompleteDraft();

        var result = profile.Unpublish();

        Assert.True(result.IsFailure);
        Assert.Equal("ActorProfile.NotPublished", result.Error.Code);
    }

    // ── INDEXABILITY ─────────────────────────────────────────────

    /// <summary>
    /// Publishing alone is not enough. This is the whole point of separating
    /// the two flags: a live page with no albums is thin content and must not
    /// reach the sitemap.
    /// </summary>
    [Fact]
    public void Publish_WithNoAlbums_DoesNotBecomeIndexable()
    {
        var profile = NewCompleteDraft();

        profile.Publish();

        Assert.True(profile.IsPublished);
        Assert.False(profile.IsIndexable);
    }

    [Fact]
    public void AdjustAlbumCount_OnPublishedProfile_MakesItIndexable()
    {
        var profile = NewCompleteDraft();
        profile.Publish();

        profile.AdjustAlbumCount(1, 12);

        Assert.True(profile.IsIndexable);
    }

    [Fact]
    public void AdjustAlbumCount_OnDraft_DoesNotMakeItIndexable()
    {
        var profile = NewCompleteDraft();

        profile.AdjustAlbumCount(1, 12);

        Assert.False(profile.IsIndexable);
    }

    [Fact]
    public void Unpublish_RemovesIndexability()
    {
        var profile = NewCompleteDraft();
        profile.Publish();
        profile.AdjustAlbumCount(1, 12);

        profile.Unpublish();

        Assert.False(profile.IsIndexable);
    }

    /// <summary>
    /// Losing the last album drops the profile out of the sitemap. Without
    /// RecomputeIndexability in AdjustAlbumCount it would stay indexable
    /// pointing at an empty page.
    /// </summary>
    [Fact]
    public void AdjustAlbumCount_DroppingToZero_RemovesIndexability()
    {
        var profile = NewCompleteDraft();
        profile.Publish();
        profile.AdjustAlbumCount(1, 12);

        profile.AdjustAlbumCount(-1, -12);

        Assert.False(profile.IsIndexable);
    }

    [Fact]
    public void UpdateDetails_ShorteningBioBelowThreshold_RemovesIndexability()
    {
        var profile = NewCompleteDraft();
        profile.Publish();
        profile.AdjustAlbumCount(1, 12);

        profile.UpdateDetails("Mara Solano", "Photographer", "Too short.");

        Assert.False(profile.IsIndexable);
    }

    // ── COUNTERS ─────────────────────────────────────────────────

    [Fact]
    public void AdjustAlbumCount_ClampsAtZero()
    {
        var profile = NewDraft();

        profile.AdjustAlbumCount(-5, -50);

        Assert.Equal(0, profile.AlbumCount);
        Assert.Equal(0, profile.MediaCount);
    }

    [Fact]
    public void AdjustFollowerCount_ClampsAtZero()
    {
        var profile = NewDraft();

        profile.AdjustFollowerCount(-3);

        Assert.Equal(0, profile.FollowerCount);
    }

    /// <summary>
    /// A follow is not a content change. UpdateDate drives sitemap lastmod, and
    /// churning it on every follow would train crawlers to ignore the signal.
    /// </summary>
    [Fact]
    public void AdjustFollowerCount_DoesNotTouchUpdateDate()
    {
        var profile = NewCompleteDraft();
        var before = profile.UpdateDate;

        profile.AdjustFollowerCount(1);

        Assert.Equal(before, profile.UpdateDate);
        Assert.Equal(1, profile.FollowerCount);
    }

    [Fact]
    public void AdjustAlbumCount_TouchesUpdateDate()
    {
        var profile = NewDraft();

        profile.AdjustAlbumCount(1, 5);

        Assert.NotNull(profile.UpdateDate);
    }

    // ── SOCIAL LINKS ─────────────────────────────────────────────

    [Fact]
    public void SetSocialLink_AddsLink()
    {
        var profile = NewDraft();

        var result = profile.SetSocialLink(SocialPlatform.Instagram, "https://instagram.com/mara");

        Assert.True(result.IsSuccess);
        var link = Assert.Single(profile.SocialLinks);
        Assert.Equal(SocialPlatform.Instagram, link.Platform);
    }

    /// <summary>The chip renders the platform name, so two entries for one
    /// platform are indistinguishable on screen.</summary>
    [Fact]
    public void SetSocialLink_SamePlatformTwice_Replaces()
    {
        var profile = NewDraft();
        profile.SetSocialLink(SocialPlatform.Instagram, "https://instagram.com/old");

        profile.SetSocialLink(SocialPlatform.Instagram, "https://instagram.com/new");

        var link = Assert.Single(profile.SocialLinks);
        Assert.Equal("https://instagram.com/new", link.Url);
    }

    /// <summary>Clearing a form field is a removal, not invalid input.</summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SetSocialLink_WithBlankUrl_RemovesLink(string? url)
    {
        var profile = NewDraft();
        profile.SetSocialLink(SocialPlatform.Instagram, "https://instagram.com/mara");

        var result = profile.SetSocialLink(SocialPlatform.Instagram, url);

        Assert.True(result.IsSuccess);
        Assert.Empty(profile.SocialLinks);
    }

    [Fact]
    public void SetSocialLink_BeyondMax_ReturnsFailure()
    {
        var profile = NewDraft();
        var platforms = Enum.GetValues<SocialPlatform>();

        foreach (var platform in platforms.Take(ActorProfile.MaxSocialLinks))
            profile.SetSocialLink(platform, $"https://example.com/{platform}");

        Assert.Equal(ActorProfile.MaxSocialLinks, profile.SocialLinks.Count);
    }

    [Fact]
    public void RemoveSocialLink_WhenAbsent_Succeeds()
    {
        var profile = NewDraft();

        var result = profile.RemoveSocialLink(SocialPlatform.TikTok);

        Assert.True(result.IsSuccess);
    }

    // ── SLUG ─────────────────────────────────────────────────────

    /// <summary>
    /// The no-op guard shipped broken once as reference equality — ValueObject
    /// does not overload ==, so it was always false and every save re-dated the
    /// profile, invalidating CDN caches and moving sitemap lastmod for a change
    /// that did not happen.
    /// </summary>
    [Fact]
    public void ChangeSlug_ToSameValue_DoesNotTouchUpdateDate()
    {
        var profile = NewDraft(slug: "mara-solano");
        var before = profile.UpdateDate;

        var result = profile.ChangeSlug("mara-solano");

        Assert.True(result.IsSuccess);
        Assert.Equal(before, profile.UpdateDate);
    }

    [Fact]
    public void ChangeSlug_ToNewValue_UpdatesSlug()
    {
        var profile = NewDraft();

        var result = profile.ChangeSlug("mara-s");

        Assert.True(result.IsSuccess);
        Assert.Equal("mara-s", profile.Slug.Value);
    }

    // ── EVENTS ───────────────────────────────────────────────────

    [Fact]
    public void UpdateDetails_OnDraft_RaisesNoUpdatedEvent()
    {
        var profile = NewDraft();
        profile.ClearDomainEvents();

        profile.UpdateDetails("Mara S", "Photographer", ValidBio);

        Assert.Empty(profile.DomainEvents);
    }

    [Fact]
    public void UpdateDetails_OnPublished_RaisesUpdatedEvent()
    {
        var profile = NewCompleteDraft();
        profile.Publish();
        profile.ClearDomainEvents();

        profile.UpdateDetails("Mara S", "Photographer", ValidBio);

        var raised = Assert.Single(profile.DomainEvents);
        Assert.IsType<ActorProfileUpdatedDomainEvent>(raised);
    }

    [Fact]
    public void Delete_RaisesDeletedDomainEvent()
    {
        var profile = NewDraft();
        profile.ClearDomainEvents();

        profile.Delete();

        var raised = Assert.Single(profile.DomainEvents);
        Assert.IsType<ActorProfileDeletedDomainEvent>(raised);
    }
}