using MassTransit;
using Microsoft.EntityFrameworkCore;
using Host.WebApi.ArtworkViews;
using Posts.Infrastructure;
using Posts.Infrastructure.Persistence;
using Posts.Presentation;
using Storage.Infrastracture.Persistence;
using Users.Infrastracture.Persistence;
using Users.Infrastracture;
using Storage.Infrastracture;
using Storage.Presentation;
using Users.Presentation;
using Microsoft.Extensions.Caching.Hybrid;

namespace Host.WebApi;

public static class HostExtensions
{

    public static async Task ApplyMigrations(this WebApplication app)
    {
        await MigrateModuleDbAsync<UsersDbContext>(app);
        await MigrateModuleDbAsync<StorageDbContext>(app);
        await MigrateModuleDbAsync<PostsDbContext>(app);
        await MigrateModuleDbAsync<ArtworkViewsDbContext>(app);
    }

    public static TBuilder RegisterModules<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        // host service registrations
        builder.Services.AddMessageBus();
        builder.Services.AddCache();

        // users module
        builder.Services.AddUsersInfrastructure(
            builder.Configuration,
            enableSeeding: builder.Environment.IsDevelopment());
        builder.Services.AddUsersPresentation();

        // storage module
        builder.Services.AddStorageInfrastructure(builder.Configuration);
        builder.Services.AddStoragePresentation();

        // Posts module
        builder.Services.AddPostsInfrastructure(builder.Configuration);
        builder.Services.AddPostsPresentation();

        return builder;
    }

    /// <summary>
    /// Registered once for the whole host. AddMassTransit replaces its
    /// configuration rather than merging it, so a second module calling it
    /// would silently discard the first module's consumers and endpoints.
    /// Modules contribute consumers here; they must not configure the bus.
    /// </summary>
    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            // Module consumers are registered here as modules gain them.

            bus.UsingInMemory((ctx, cfg) =>
            {
                cfg.ConfigureEndpoints(ctx);
            });
        });

        return services;
    }

    private static IServiceCollection AddCache(this IServiceCollection services)
    {
        services.AddHybridCache(options =>
        {
            options.DefaultEntryOptions = new HybridCacheEntryOptions
            {
                Expiration = TimeSpan.FromMinutes(5),
                LocalCacheExpiration = TimeSpan.FromMinutes(5)
            };
        });

        return services;
    }

    private static async Task MigrateModuleDbAsync<TDbContext>(this WebApplication app) where TDbContext : DbContext
    {
        await using var scope = app.Services.CreateAsyncScope();
        var db = scope.ServiceProvider.GetRequiredService<TDbContext>();
        await db.Database.MigrateAsync();
    }
}
