using Catalog.Application;
using Catalog.Application.Abstractions;
using Catalog.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;

namespace Catalog.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddCatalogInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddCatalogDbContext(configuration);
        services.AddCatalogApplication();
        services.AddScoped<ICatalogRepository, CatalogRepository>();

        return services;
    }

    private static IServiceCollection AddCatalogDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = ResolveConnectionString(configuration);
        services.AddDbContext<CatalogDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                npgsql.EnableRetryOnFailure()));

        return services;
    }

    internal static string ResolveConnectionString(IConfiguration configuration)
    {
        var configuredConnectionString = configuration.GetConnectionString("MainDbConnectionString");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = configuration["POSTGRES_HOST"] ?? "localhost",
            Port = int.TryParse(configuration["POSTGRES_PORT"], out var port) ? port : 5432,
            Database = configuration["POSTGRES_DB"] ?? "erofancy",
            Username = configuration["POSTGRES_USER"] ?? "postgres",
            Password = configuration["POSTGRES_PASSWORD"] ?? "postgres"
        };

        return builder.ConnectionString;
    }
}
