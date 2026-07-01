using Users.Common;

namespace Users.Domain.UnitTests;

public class UserTests
{
    private readonly FakePasswordHasher _hasher = new();

    [Fact]
    public void Create_WithValidInput_ReturnsSuccess()
    {
        var result = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher);

        Assert.True(result.IsSuccess);
        Assert.Equal("John Doe", result.Value.Name);
        Assert.Equal("john@example.com", result.Value.Email.Value);
        Assert.Equal(Role.User, result.Value.Role);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Create_WithEmptyName_ReturnsFailure(string? name)
    {
        var result = User.Create(name, "john@example.com", "Password123", Role.User, _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("User.NameEmpty", result.Error.Code);
    }

    [Fact]
    public void Create_WithInvalidEmail_ReturnsFailure()
    {
        var result = User.Create("John Doe", "not-an-email", "Password123", Role.User, _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal("Email.InvalidFormat", result.Error.Code);
    }

    [Fact]
    public void Create_WithWeakPassword_ReturnsFailure()
    {
        var result = User.Create("John Doe", "john@example.com", "weak", Role.User, _hasher);

        Assert.True(result.IsFailure);
    }

    [Fact]
    public void Create_AssignsUniqueId()
    {
        var user1 = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher).Value;
        var user2 = User.Create("Jane Doe", "jane@example.com", "Password123", Role.User, _hasher).Value;

        Assert.NotEqual(user1.Id, user2.Id);
    }

    [Fact]
    public void UpdateProfile_WithValidInput_UpdatesNameAndEmail()
    {
        var user = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher).Value;

        var result = user.UpdateProfile("Jane Doe", "jane@example.com");

        Assert.True(result.IsSuccess);
        Assert.Equal("Jane Doe", user.Name);
        Assert.Equal("jane@example.com", user.Email.Value);
    }

    [Fact]
    public void UpdateProfile_WithInvalidEmail_ReturnsFailure_AndDoesNotMutateState()
    {
        var user = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher).Value;

        var result = user.UpdateProfile("Jane Doe", "not-an-email");

        Assert.True(result.IsFailure);
        Assert.Equal("John Doe", user.Name);
        Assert.Equal("john@example.com", user.Email.Value);
    }

    [Fact]
    public void ChangePassword_WithWeakPassword_ReturnsFailure()
    {
        var user = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher).Value;
        var originalHash = user.Password.HashedValue;

        var result = user.ChangePassword("weak", _hasher);

        Assert.True(result.IsFailure);
        Assert.Equal(originalHash, user.Password.HashedValue);
    }

    [Fact]
    public void ChangeRole_UpdatesRole()
    {
        var user = User.Create("John Doe", "john@example.com", "Password123", Role.User, _hasher).Value;

        user.ChangeRole(Role.Admin);

        Assert.Equal(Role.Admin, user.Role);
    }
}