using Microsoft.EntityFrameworkCore;
using Storage.Infrastracture.Persistence;
using Users.Infrastracture.Persistence;

namespace Host.WebApi;

public static class HostExtensions
{

    public static async Task MigrateUsersDbAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<UsersDbContext>();

        await db.Database.MigrateAsync();
    }

    public static async Task MigrateStorageDbAsync(this WebApplication app)
    {
        await using var scope = app.Services.CreateAsyncScope();

        var db = scope.ServiceProvider.GetRequiredService<StorageDbContext>();

        await db.Database.MigrateAsync();
    }
}
