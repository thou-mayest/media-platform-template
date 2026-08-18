using Profiles.Domain.ValueObjects;

namespace Profiles.Domain.UnitTests;

public class SocialLinkTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyUrl_ReturnsFailure(string? url)
    {
        var result = SocialLink.Create(SocialPlatform.Instagram, url);

        Assert.True(result.IsFailure);
        Assert.Equal("SocialLink.UrlEmpty", result.Error.Code);
    }

    [Theory]
    [InlineData("not a url")]
    [InlineData("instagram.com/mara")]
    [InlineData("/relative/path")]
    public void Create_WithNonAbsoluteUrl_ReturnsFailure(string url)
    {
        var result = SocialLink.Create(SocialPlatform.Instagram, url);

        Assert.True(result.IsFailure);
        Assert.Equal("SocialLink.UrlInvalid", result.Error.Code);
    }

    /// <summary>
    /// Rendered as an outbound link on a public page, so http would downgrade
    /// the visitor's connection on click.
    /// </summary>
    [Theory]
    [InlineData("http://instagram.com/mara")]
    [InlineData("ftp://example.com/file")]
    [InlineData("javascript:alert(1)")]
    public void Create_WithNonHttpsScheme_ReturnsFailure(string url)
    {
        var result = SocialLink.Create(SocialPlatform.Instagram, url);

        Assert.True(result.IsFailure);
        Assert.Equal("SocialLink.UrlNotHttps", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongUrl_ReturnsFailure()
    {
        var url = "https://example.com/" + new string('a', SocialLink.MaxUrlLength);

        var result = SocialLink.Create(SocialPlatform.Instagram, url);

        Assert.True(result.IsFailure);
        Assert.Equal("SocialLink.UrlTooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithValidHttpsUrl_ReturnsSuccess()
    {
        var result = SocialLink.Create(SocialPlatform.Instagram, "  https://instagram.com/mara  ");

        Assert.True(result.IsSuccess);
        Assert.Equal(SocialPlatform.Instagram, result.Value.Platform);
        Assert.Equal("https://instagram.com/mara", result.Value.Url);
    }
}