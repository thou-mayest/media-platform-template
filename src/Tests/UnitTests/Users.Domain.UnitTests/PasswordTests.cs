using Users.Domain.ValueObjects;

namespace Users.Domain.UnitTests;

public class PasswordTests
{
    private readonly FakePasswordHasher _hasher = new();

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyOrWhitespace_ReturnsFailure(string? password)
    {
        var result = Password.Create(password, _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("Password.Empty", result.Error.Code);
    }

    [Fact]
    public void Create_TooShort_ReturnsFailure()
    {
        var result = Password.Create("Ab1", _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("Password.TooShort", result.Error.Code);
    }

    [Fact]
    public void Create_MissingDigit_ReturnsFailure()
    {
        var result = Password.Create("Abcdefgh", _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("Password.MissingDigit", result.Error.Code);
    }

    [Fact]
    public void Create_MissingUppercase_ReturnsFailure()
    {
        var result = Password.Create("abcdefg1", _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("Password.MissingUppercase", result.Error.Code);
    }

    [Fact]
    public void Create_WithValidPassword_ReturnsSuccess()
    {
        var result = Password.Create("Password123", _hasher);

        Assert.True(result.IsSuccess);
    }

    [Fact]
    public void Create_StoresHashedValue_NotPlainText()
    {
        var result = Password.Create("Password123", _hasher);

        Assert.True(result.IsSuccess);
        Assert.NotEqual("Password123", result.Value.HashedValue);
    }
}