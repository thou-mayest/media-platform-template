using Users.Domain.ValueObjects;

namespace Users.Domain.UnitTests;

public class EmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.Empty", result.Error.Code);
    }

    [Theory]
    [InlineData("not-an-email")]
    [InlineData("missing-domain@")]
    [InlineData("@missing-local.com")]
    [InlineData("no-at-sign.com")]
    public void Create_WithInvalidFormat_ReturnsFailure(string email)
    {
        var result = Email.Create(email);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongEmail_ReturnsFailure()
    {
        var localPart = new string('a', 250);
        var email = $"{localPart}@example.com";

        var result = Email.Create(email);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.TooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithValidEmail_ReturnsSuccess()
    {
        var result = Email.Create("user@example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void Create_NormalizesEmailToLowercase()
    {
        var result = Email.Create("User@Example.COM");

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        var result = Email.Create("  user@example.com  ");

        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void TwoEmails_WithSameValue_AreEqual()
    {
        var email1 = Email.Create("user@example.com").Value;
        var email2 = Email.Create("user@example.com").Value;

        Assert.Equal(email1, email2);
    }
}