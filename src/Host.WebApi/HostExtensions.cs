using MassTransit;
using Microsoft.EntityFrameworkCore;
using Posts.Infrastructure;
using Posts.Infrastructure.Persistence;
using Posts.Presentation;
using Profiles.Application;
using Profiles.Infrastructure;
using Profiles.Infrastructure.Persistence;
using Profiles.Presentation;
using Storage.Infrastracture.Persistence;
using Users.Infrastracture.Persistence;
using Users.Infrastracture;
using Storage.Infrastracture;
using Storage.Presentation;
using Users.Presentation;
using Microsoft.Extensions.Caching.Hybrid;
using SharedKernal.Configurations;

namespace Host.WebApi;

public static class HostExtensions
{

    public static async Task ApplyMigrations(this WebApplication app)
    {
        await MigrateModuleDbAsync<UsersDbContext>(app);
        await MigrateModuleDbAsync<StorageDbContext>(app);
        await MigrateModuleDbAsync<PostsDbContext>(app);
        await MigrateModuleDbAsync<ProfilesDbContext>(app);
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

        // profiles module
        builder.Services.AddProfilesInfrastructure(builder.Configuration);
        builder.Services.AddProfilesPresentation();

        builder.AddCors();

        return builder;
    }

    public static TBuilder AddCors<TBuilder>(this TBuilder builder) where TBuilder : IHostApplicationBuilder
    {
        builder.Services
            .AddOptions<CorsOptions>()
            .Bind(builder.Configuration.GetSection(CorsOptions.SectionName))
            .Validate(
                options => options.AllowedOrigins is { Length: > 0 }
                    && options.AllowedOrigins.All(origin =>
                        Uri.TryCreate(origin, UriKind.Absolute, out var uri)
                        && (uri.Scheme == Uri.UriSchemeHttps
                            || uri.Scheme == Uri.UriSchemeHttp)),
                $"{CorsOptions.SectionName}:AllowedOrigins must contain at least one valid absolute HTTP/HTTPS URL.")
            .ValidateOnStart();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsOptions.CorsFrontendPolicyName, policy =>
            {
                var corsOptions = builder.Configuration
                    .GetSection(CorsOptions.SectionName)
                    .Get<CorsOptions>()!;

                policy
                    .WithOrigins(corsOptions.AllowedOrigins)
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return builder;
    }

    private static IServiceCollection AddMessageBus(this IServiceCollection services)
    {
        services.AddMassTransit(bus =>
        {
            bus.SetKebabCaseEndpointNameFormatter();

            // Module consumers are registered here as modules gain them.
            bus.AddProfilesConsumers();

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
