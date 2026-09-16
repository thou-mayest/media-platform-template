using Microsoft.Extensions.Options;
using SharedKernal.Results;
using SharedKernel.Entities.Enums;
using Users.Application.Abstractions;
using Users.Domain;
using Users.Domain.Abstractions;
using Users.Infrastracture.Seeding;

namespace Users.IntegrationTests;

public sealed class UserSeederTests
{
    [Fact]
    public async Task SeedAsync_AddsConfiguredUsers()
    {
        var repository = new FakeUserRepository();
        var options = CreateOptions(
            new UserSeedDefinition
            {
                Name = "Admin",
                Email = " ADMIN@example.test ",
                Password = "Password123",
                Role = Role.Admin
            });

        await new UserSeeder(repository, FakePasswordHasher.Instance, options).SeedAsync();

        var user = Assert.Single(repository.Users);
        Assert.Equal("admin@example.test", user.Email.Value);
        Assert.Equal(Role.Admin, user.Role);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public async Task SeedAsync_IsIdempotentByNormalizedEmail()
    {
        var repository = new FakeUserRepository();
        repository.Users.Add(CreateUser("Existing", "admin@example.test"));
        var options = CreateOptions(
            new UserSeedDefinition
            {
                Name = "Duplicate",
                Email = " ADMIN@EXAMPLE.TEST ",
                Password = "Password123",
                Role = Role.Admin
            });

        await new UserSeeder(repository, FakePasswordHasher.Instance, options).SeedAsync();

        Assert.Single(repository.Users);
        Assert.Equal(0, repository.SaveCalls);
    }

    [Fact]
    public async Task SeedAsync_IgnoresDuplicateEntriesInConfiguration()
    {
        var repository = new FakeUserRepository();
        var options = CreateOptions(
            new UserSeedDefinition
            {
                Name = "First",
                Email = "user@example.test",
                Password = "Password123",
                Role = Role.User
            },
            new UserSeedDefinition
            {
                Name = "Second",
                Email = "USER@example.test",
                Password = "Password123",
                Role = Role.PremiumUser
            });

        await new UserSeeder(repository, FakePasswordHasher.Instance, options).SeedAsync();

        Assert.Single(repository.Users);
        Assert.Equal(1, repository.SaveCalls);
    }

    [Fact]
    public async Task SeedAsync_DoesNothingWhenDisabled()
    {
        var repository = new FakeUserRepository();
        var options = Options.Create(new UserSeedOptions
        {
            Enabled = false,
            Users =
            [
                new UserSeedDefinition
                {
                    Name = "Admin",
                    Email = "admin@example.test",
                    Password = "Password123",
                    Role = Role.Admin
                }
            ]
        });

        await new UserSeeder(repository, FakePasswordHasher.Instance, options).SeedAsync();

        Assert.Empty(repository.Users);
        Assert.Equal(0, repository.SaveCalls);
    }

    [Fact]
    public async Task SeedAsync_RejectsInvalidConfiguration()
    {
        var repository = new FakeUserRepository();
        var options = CreateOptions(
            new UserSeedDefinition
            {
                Name = "Admin",
                Email = "not-an-email",
                Password = "Password123",
                Role = Role.Admin
            });

        var exception = await Assert.ThrowsAsync<InvalidOperationException>(
            () => new UserSeeder(repository, FakePasswordHasher.Instance, options).SeedAsync());

        Assert.Contains("Could not seed user", exception.Message);
        Assert.Empty(repository.Users);
        Assert.Equal(0, repository.SaveCalls);
    }

    private static IOptions<UserSeedOptions> CreateOptions(params UserSeedDefinition[] users) =>
        Options.Create(new UserSeedOptions { Enabled = true, Users = [.. users] });

    private static User CreateUser(string name, string email) =>
        User.Create(name, email, "Password123", Role.User, FakePasswordHasher.Instance).Value;

    private sealed class FakeUserRepository : IUserRepository
    {
        public List<User> Users { get; } = [];
        public int SaveCalls { get; private set; }

        public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.SingleOrDefault(user => user.Id == id));

        public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default) =>
            Task.FromResult(Users.SingleOrDefault(user =>
                string.Equals(user.Email.Value, email.Trim(), StringComparison.OrdinalIgnoreCase)));

        public Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<User>>(Users);

        public Task AddAsync(User user, CancellationToken cancellationToken = default)
        {
            Users.Add(user);
            return Task.CompletedTask;
        }

        public void Update(User user)
        {
        }

        public void Remove(User user) => Users.Remove(user);

        public Task<Result<int>> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveCalls++;
            return Task.FromResult<Result<int>>(Users.Count);
        }
    }

    private sealed class FakePasswordHasher : IPasswordHasher
    {
        public static readonly FakePasswordHasher Instance = new();

        public string Hash(string plainTextPassword) => $"hashed:{plainTextPassword}";
        public bool Verify(string plainTextPassword, string hashedPassword) =>
            hashedPassword == Hash(plainTextPassword);

        public void PerformFakeVerification()
        {
        }
    }
}
