using System.Net;
using System.Net.Http.Json;
using System.Net.Http.Headers;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;

namespace CleanModular.IntegrationTests;

[Collection(IntegrationTestCollection.Name)]
public sealed class ApiSecurityTests
{
    private readonly HttpClient _client;

    public ApiSecurityTests(ApiFactory factory)
    {
        _client = factory.CreateClient(new WebApplicationFactoryClientOptions
        {
            AllowAutoRedirect = false
        });
    }

    [Fact]
    public async Task UsersEndpoint_RejectsAnonymousRequests()
    {
        var response = await _client.GetAsync("/api/users");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task LoginEndpoint_RejectsInvalidInputBeforeDatabaseAccess()
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "not-an-email",
            password = "short"
        });

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        Assert.Equal("application/problem+json", response.Content.Headers.ContentType?.MediaType);
    }

    [Fact]
    public async Task BootstrapAdmin_CanLoginAndAccessProtectedUsers()
    {
        if (string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION")))
        {
            Assert.NotEqual("true", Environment.GetEnvironmentVariable("REQUIRE_TEST_DATABASE"));
            return;
        }

        var login = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "admin@example.test",
            password = "integration-admin-password"
        });
        login.EnsureSuccessStatusCode();
        var payload = await login.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", payload.AccessToken);
        var users = await _client.GetAsync("/api/users?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, users.StatusCode);
    }

    private sealed record LoginPayload(string AccessToken);
}

public sealed class ApiFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var database = Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION")
            ?? "Host=127.0.0.1;Port=59999;Database=tests;Username=tests;Password=tests;Timeout=1";
        var useDatabase = Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION") is not null;
        builder
            .UseEnvironment("Testing")
            .UseSetting("ConnectionStrings:MainDb", database)
            .UseSetting("Jwt:Issuer", "integration-tests")
            .UseSetting("Jwt:Audience", "integration-tests")
            .UseSetting("Jwt:SigningKey", "integration-test-signing-key-at-least-32-characters")
            .UseSetting("Database:ApplyMigrations", useDatabase.ToString())
            .UseSetting("BootstrapAdmin:Name", "Integration Administrator")
            .UseSetting("BootstrapAdmin:Email", "admin@example.test")
            .UseSetting("BootstrapAdmin:Password", useDatabase ? "integration-admin-password" : string.Empty);
    }
}
