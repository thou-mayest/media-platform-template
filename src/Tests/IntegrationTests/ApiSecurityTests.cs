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
            password = "IntegrationAdmin123"
        });
        login.EnsureSuccessStatusCode();
        var payload = await login.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", payload.AccessToken);
        var users = await _client.GetAsync("/api/users?page=1&pageSize=10");

        Assert.Equal(HttpStatusCode.OK, users.StatusCode);
    }

    [Fact]
    public async Task ValidFormatInvalidCredentials_ReturnUnauthorized()
    {
        if (!DatabaseIsConfigured()) return;

        var response = await _client.PostAsJsonAsync("/api/auth/login", new
        {
            email = "missing@example.test",
            password = "MissingPassword123"
        });

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task UserCannotPromoteOwnRoleToAdmin()
    {
        if (!DatabaseIsConfigured()) return;

        var adminToken = await LoginAsync("admin@example.test", "IntegrationAdmin123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var email = $"user-{Guid.NewGuid():N}@example.test";
        var create = await _client.PostAsJsonAsync("/api/users", new
        {
            name = "Normal User",
            email,
            password = "NormalUser123",
            role = 2
        });
        create.EnsureSuccessStatusCode();
        var created = await create.Content.ReadFromJsonAsync<CreatedUser>();
        Assert.NotNull(created);

        var userToken = await LoginAsync(email, "NormalUser123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var promote = await _client.PutAsJsonAsync($"/api/users/{created.Id}", new
        {
            name = "Normal User",
            email,
            password = (string?)null,
            role = 1
        });

        Assert.Equal(HttpStatusCode.Forbidden, promote.StatusCode);
    }

    private bool DatabaseIsConfigured()
    {
        var configured = !string.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable("TEST_DATABASE_CONNECTION"));
        if (!configured)
            Assert.NotEqual("true", Environment.GetEnvironmentVariable("REQUIRE_TEST_DATABASE"));
        return configured;
    }

    private async Task<string> LoginAsync(string email, string password)
    {
        var response = await _client.PostAsJsonAsync("/api/auth/login", new { email, password });
        response.EnsureSuccessStatusCode();
        var payload = await response.Content.ReadFromJsonAsync<LoginPayload>();
        Assert.False(string.IsNullOrWhiteSpace(payload?.AccessToken));
        return payload.AccessToken;
    }

    private sealed record LoginPayload(string AccessToken);
    private sealed record CreatedUser(Guid Id);
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
            .UseSetting("BootstrapAdmin:Password", useDatabase ? "IntegrationAdmin123" : string.Empty);
    }
}
