using SharedKernal.Results;
using SharedKernal.ValueObjects;

namespace Profiles.Domain.ValueObjects;

public sealed class SocialLink : ValueObject
{
    public const int MaxUrlLength = 2048;

    public SocialPlatform Platform { get; }
    public string Url { get; }

    private SocialLink(SocialPlatform platform, string url)
    {
        Platform = platform;
        Url = url;
    }

    internal static Result<SocialLink> Create(SocialPlatform platform, string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return Error.Validation("SocialLink.UrlEmpty", "Social link URL cannot be empty.");

        var trimmed = url.Trim();

        if (trimmed.Length > MaxUrlLength)
            return Error.Validation("SocialLink.UrlTooLong", $"URL must not exceed {MaxUrlLength} characters.");

        if (!Uri.TryCreate(trimmed, UriKind.Absolute, out var uri))
            return Error.Validation("SocialLink.UrlInvalid", "URL must be absolute and well-formed.");

       
        if (uri.Scheme != Uri.UriSchemeHttps)
            return Error.Validation("SocialLink.UrlNotHttps", "URL must use https.");

        return new SocialLink(platform, uri.ToString());
    }

    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Platform;
        yield return Url;
    }
}