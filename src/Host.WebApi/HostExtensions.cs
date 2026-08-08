using Microsoft.EntityFrameworkCore;
using Storage.Infrastracture.Persistence;
using Users.Infrastracture.Persistence;
using Users.Infrastracture;
using Users.Presentation;

namespace Host.WebApi;

public static class HostExtensions
{

    public static async Task ApplyMigrations(this WebApplication app)
    {
        await MigrateModuleDbAsync<UsersDbContext>(app);
    }

    public static TBuilder RegisterModules<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // register users modules
        builder.Services.AddUsersInfrastructure(builder.Configuration);
        builder.Services.AddUsersPresentation();

        return builder;
    }


    private static async Task MigrateModuleDbAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Database.MigrateAsync();
    }

    public static async Task MigrateStorageDbAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<StorageDbContext>();

        await db.Database.MigrateAsync();
    }
}
