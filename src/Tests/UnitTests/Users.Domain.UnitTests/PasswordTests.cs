using Moq;
using Users.Domain.Abstractions;
using Users.Domain.ValueObjects;

namespace Users.Domain.UnitTests;

public class PasswordTests
{
    // Create a mock object that simulates IPasswordHasher behavior
    private readonly Mock<IPasswordHasher> _hasherMock;

    // This is the actual interface reference we pass into our domain methods
    private readonly IPasswordHasher _hasher;

    public PasswordTests()
    {
        // Initialize the mock
        _hasherMock = new Mock<IPasswordHasher>();

        // Setup: When Hash() is called with ANY string,
        // return "hashed:" followed by the input password.
        // This simulates hashing without needing a real implementation.
        _hasherMock
            .Setup(h => h.Hash(It.IsAny<string>()))
            .Returns((string plainText) => $"hashed:{plainText}");

        // Setup: When Verify() is called with ANY two strings,
        // check if the hashed password matches "hashed:" + plain text.
        // This simulates password verification logic.
        _hasherMock
            .Setup(h => h.Verify(It.IsAny<string>(), It.IsAny<string>()))
            .Returns((string plainText, string hashedPassword) =>
                hashedPassword == $"hashed:{plainText}");

        // Extract the mocked IPasswordHasher instance to use in tests
        _hasher = _hasherMock.Object;
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? password)
    {
        // Act: Try to create a password with empty/whitespace input
        var result = Password.Create(password, _hasher);

        // Assert: Should fail with "Password.Empty" error
        Assert.True(result.IsFailure);
        Assert.Equal("Password.Empty", result.Error.Code);
    }

    [Fact]
    public void Create_TooShort_ReturnsFailure()
    {
        // Act: Try to create a password that is too short
        var result = Password.Create("Ab1", _hasher);

        // Assert: Should fail with "Password.TooShort" error
        Assert.True(result.IsFailure);
        Assert.Equal("Password.TooShort", result.Error.Code);
    }

    [Fact]
    public void Create_MissingDigit_ReturnsFailure()
    {
        // Act: Try to create a password without any digit
        var result = Password.Create("Abcdefgh", _hasher);

        // Assert: Should fail with "Password.MissingDigit" error
        Assert.True(result.IsFailure);
        Assert.Equal("Password.MissingDigit", result.Error.Code);
    }

    [Fact]
    public void Create_MissingUppercase_ReturnsFailure()
    {
        // Act: Try to create a password without any uppercase letter
        var result = Password.Create("abcdefg1", _hasher);

        // Assert: Should fail with "Password.MissingUppercase" error
        Assert.True(result.IsFailure);
        Assert.Equal("Password.MissingUppercase", result.Error.Code);
    }

    [Fact]
    public void Create_WithValidPassword_ReturnsSuccess()
    {
        // Act: Create a password with valid input (has uppercase, digit, enough length)
        var result = Password.Create("Password123", _hasher);

        // Assert: Should succeed
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_StoresHashedValue_NotPlainText()
    {
        // Act: Create a valid password
        var result = Password.Create("Password123", _hasher);

        // Assert: The stored value should NOT be the plain text password.
        // It should be the hashed version returned by the mock ("hashed:Password123").
        Assert.True(result.IsSuccess);
        Assert.NotEqual("Password123", result.Value.HashedValue);
    }
}