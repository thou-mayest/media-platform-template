using Users.Common;
using Users.Domain;
using Users.Infrastracture.Security;

namespace CleanModular.ArchTests;

public sealed class UserSecurityTests
{
    [Fact]
    public void User_NormalizesEmailAndStoresOnlyHash()
    {
        var user = new User("  Test User  ", "  User@Example.com ", "hashed-value", Role.User);

        Assert.Equal("Test User", user.Name);
        Assert.Equal("USER@EXAMPLE.COM", user.Email);
        Assert.Equal("hashed-value", user.PasswordHash);
    }

    [Fact]
    public void RoleValues_AreExplicitAndAdminIsNotDefault()
    {
        Assert.Equal(1, (int)Role.Admin);
        Assert.Equal(2, (int)Role.User);
        Assert.Equal(3, (int)Role.PremiumUser);
        Assert.False(Enum.IsDefined(default(Role)));
    }

    [Fact]
    public void UpdatingUser_RotatesConcurrencyVersion()
    {
        var user = new User("User", "user@example.com", "hash-1", Role.User);
        var initialVersion = user.Version;

        user.Update("Updated", "updated@example.com", "hash-2", Role.PremiumUser);

        Assert.NotEqual(initialVersion, user.Version);
        Assert.NotNull(user.UpdateDate);
    }

    [Fact]
    public void PasswordHasher_UsesSaltAndVerifiesCredentials()
    {
        var hasher = new AspNetPasswordHashingService();

        var firstHash = hasher.Hash("correct-horse-battery-staple");
        var secondHash = hasher.Hash("correct-horse-battery-staple");

        Assert.NotEqual(firstHash, secondHash);
        Assert.True(hasher.Verify(firstHash, "correct-horse-battery-staple"));
        Assert.False(hasher.Verify(firstHash, "wrong-password"));
        Assert.False(hasher.Verify(User.InvalidatedPasswordHash, "anything"));
    }
}
