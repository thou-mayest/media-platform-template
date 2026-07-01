using Users.Domain.ValueObjects;

namespace Users.Domain.UnitTests;

// Email is a Value Object with no external dependencies (no interfaces, no services).
// It only takes a plain string, so no mocking is needed here.
// This is pure unit testing — we just pass inputs and verify the output.
public class EmailTests
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? email)
    {
        // Act: Try to create an email with null, empty, or whitespace input
        var result = Email.Create(email);

        // Assert: Should fail with "Email.Empty" error
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
        // Act: Try to create an email with an invalid format
        var result = Email.Create(email);

        // Assert: Should fail with "Email.InvalidFormat" error
        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public void Create_WithTooLongEmail_ReturnsFailure()
    {
        // Arrange: Build an email that exceeds the max allowed length (256 characters)
        var localPart = new string('a', 250);
        var email = $"{localPart}@example.com";

        // Act: Try to create the email
        var result = Email.Create(email);

        // Assert: Should fail with "Email.TooLong" error
        Assert.True(result.IsFailure);
        Assert.Equal("Email.TooLong", result.Error.Code);
    }

    [Fact]
    public void Create_WithValidEmail_ReturnsSuccess()
    {
        // Act: Create an email with a valid input
        var result = Email.Create("user@example.com");

        // Assert: Should succeed and store the correct value
        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void Create_NormalizesEmailToLowercase()
    {
        // Act: Create an email with mixed case input
        var result = Email.Create("User@Example.COM");

        // Assert: Should succeed and normalize the email to lowercase.
        // This ensures "User@Example.COM" and "user@example.com" are treated as the same email.
        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void Create_TrimsWhitespace()
    {
        // Act: Create an email with leading and trailing whitespace
        var result = Email.Create("  user@example.com  ");

        // Assert: Should succeed and strip the whitespace from both ends.
        // This prevents accidental duplicates caused by extra spaces.
        Assert.True(result.IsSuccess);
        Assert.Equal("user@example.com", result.Value.Value);
    }

    [Fact]
    public void TwoEmails_WithSameValue_AreEqual()
    {
        // Arrange: Create two separate Email instances with the same value
        var email1 = Email.Create("user@example.com").Value;
        var email2 = Email.Create("user@example.com").Value;

        // Assert: They should be equal because Email is a Value Object.
        // Value Objects are compared by their value, not by reference.
        Assert.Equal(email1, email2);
    }
}