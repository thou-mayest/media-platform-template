using Auth.Infrastructure.Persistence;
using Catalog.Infrastructure.Persistence;
using Catalog.Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;
using Users.Infrastracture.Persistence;

namespace Host.WebApi.Infrastructure;

public static class DatabaseStartupExtensions
{
    public static async Task ApplyDatabaseMigrationsAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        await MigrateAsync(scope.ServiceProvider.GetRequiredService<UsersDbContext>());
        await MigrateAsync(scope.ServiceProvider.GetRequiredService<AuthDbContext>());
        var catalogDbContext = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
        await MigrateAsync(catalogDbContext);
        await CatalogSeed.SeedAsync(catalogDbContext);
    }

    private static async Task MigrateAsync<TDbContext>(TDbContext dbContext)
        where TDbContext : DbContext
    {
        await dbContext.Database.MigrateAsync();
    }
}
