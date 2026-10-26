using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Npgsql;

namespace Catalog.Infrastructure.Persistence;

public sealed class CatalogDbContextFactory : IDesignTimeDbContextFactory<CatalogDbContext>
{
    public CatalogDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<CatalogDbContext>();
        optionsBuilder.UseNpgsql(ResolveConnectionString());

        return new CatalogDbContext(optionsBuilder.Options);
    }

    private static string ResolveConnectionString()
    {
        var configuredConnectionString =
            Environment.GetEnvironmentVariable("ConnectionStrings__MainDbConnectionString");
        if (!string.IsNullOrWhiteSpace(configuredConnectionString))
        {
            return configuredConnectionString;
        }

        return new NpgsqlConnectionStringBuilder
        {
            Host = Environment.GetEnvironmentVariable("POSTGRES_HOST") ?? "localhost",
            Port = int.TryParse(Environment.GetEnvironmentVariable("POSTGRES_PORT"), out var port) ? port : 5432,
            Database = Environment.GetEnvironmentVariable("POSTGRES_DB") ?? "erofancy",
            Username = Environment.GetEnvironmentVariable("POSTGRES_USER") ?? "postgres",
            Password = Environment.GetEnvironmentVariable("POSTGRES_PASSWORD") ?? "postgres"
        }.ConnectionString;
    }
}
