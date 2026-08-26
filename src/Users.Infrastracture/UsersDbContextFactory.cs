using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Users.Infrastracture.Persistence;

namespace Users.Infrastructure;

public class UsersDbContextFactory : IDesignTimeDbContextFactory<UsersDbContext>
{
    public UsersDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<UsersDbContext>();

        optionsBuilder.UseNpgsql("Host=localhost;Port=55288;Database=media_platform;Username=postgres;Password=password");

        return new UsersDbContext(optionsBuilder.Options);
    }
}