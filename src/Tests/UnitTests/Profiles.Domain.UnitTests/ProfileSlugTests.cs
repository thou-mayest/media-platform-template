using Profiles.Domain.ValueObjects;

namespace Profiles.Domain.UnitTests;

public class ProfileSlugTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? value)
    {
        var result = ProfileSlug.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.Empty", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooShortValue_ReturnsFailure()
    {
        var result = ProfileSlug.Create("ab");

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.TooShort", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongValue_ReturnsFailure()
    {
        var result = ProfileSlug.Create(new string('a', ProfileSlug.MaxLength + 1));

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.TooLong", result.Error.Code);
    }

    [Theory]
    [InlineData("has space")]
    [InlineData("double--hyphen")]
    [InlineData("-leading")]
    [InlineData("trailing-")]
    [InlineData("under_score")]
    [InlineData("dot.separated")]
    public void Create_WithInvalidShape_ReturnsFailure(string value)
    {
        var result = ProfileSlug.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.InvalidFormat", result.Error.Code);
    }

    /// <summary>
    /// "me" matters specifically: the controller routes GET /api/profiles/me,
    /// and a profile claiming that slug would be permanently unreachable.
    /// </summary>
    [Theory]
    [InlineData("admin")]
    [InlineData("api")]
    [InlineData("settings")]
    [InlineData("sitemap")]
    public void Create_WithReservedValue_ReturnsFailure(string value)
    {
        var result = ProfileSlug.Create(value);

        Assert.True(result.IsFailure);
        Assert.Equal("ProfileSlug.Reserved", result.Error.Code);
    }


    [Fact]
    public void Create_WithRouteReservedValue_ReturnsFailure()
    {
        var result = ProfileSlug.Create("me");

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_WithValidValue_ReturnsSuccess()
    {
        var result = ProfileSlug.Create("mara-solano");

        Assert.True(result.IsSuccess);
        Assert.Equal("mara-solano", result.Value.Value);
    }

    /// <summary>Trims and lowercases before validating, so a mixed-case URL
    /// still resolves rather than 404ing.</summary>
    [Fact]
    public void Create_NormalisesCaseAndWhitespace()
    {
        var result = ProfileSlug.Create("  Mara-Solano  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("mara-solano", result.Value.Value);
    }

    [Theory]
    [InlineData("Mara Solano", "mara-solano")]
    [InlineData("  Hello   World  ", "hello-world")]
    [InlineData("Café & Bar", "cafe-bar")]
    [InlineData("Tomas Réti", "tomas-reti")]
    public void Slugify_ProducesValidSlug(string displayName, string expected)
    {
        Assert.Equal(expected, ProfileSlug.Slugify(displayName));
    }

    /// <summary>
    /// A name written entirely outside [a-z0-9] reduces to nothing. This is why
    /// ProfileSlugFactory has a fallback — without it, provisioning would fail
    /// for those users and take the signup down with it.
    /// </summary>
    [Theory]
    [InlineData("!!!")]
    [InlineData("你好")]
    [InlineData("---")]
    public void Slugify_WithNoLatinCharacters_ReturnsEmpty(string displayName)
    {
        Assert.Equal(string.Empty, ProfileSlug.Slugify(displayName));
    }
}