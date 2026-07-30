using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;
using System.Threading.RateLimiting;
using Host.WebApi;
using Host.WebApi.ArtworkViews;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Users.Application.Abstractions;
using Users.Common;
using Users.Infrastracture;
using Users.Infrastracture.Persistence;
using Users.Presentation;

var builder = WebApplication.CreateBuilder(args);
builder.AddServiceDefaults();
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddUsersPresentation();

var mainDbConnectionString = builder.Configuration.GetConnectionString("MainDb")
    ?? throw new InvalidOperationException("Connection string 'MainDb' is not configured.");
builder.Services.AddDbContextPool<ArtworkViewsDbContext>(options =>
    options.UseNpgsql(mainDbConnectionString, npgsql =>
        npgsql.MigrationsHistoryTable("__artwork_view_migrations", "analytics")));
builder.Services.AddSingleton<ArtworkSlugCatalog>();
builder.Services.AddScoped<ArtworkViewStore>();
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();

var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
builder.Services.AddCors(options => options.AddPolicy("public-frontend", policy =>
{
    if (allowedOrigins.Length > 0)
    {
        policy.WithOrigins(allowedOrigins).WithMethods("GET", "POST").AllowAnyHeader();
    }
}));

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
    throw new InvalidOperationException("Jwt:SigningKey must contain at least 32 characters.");
if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");

builder.Services.Configure<JwtOptions>(jwtSection);
builder.Services.AddSingleton<ITokenService, JwtTokenService>();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.MapInboundClaims = false;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.SigningKey)),
            ClockSkew = TimeSpan.FromMinutes(1),
            RoleClaimType = "role"
        };
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var subject = context.Principal?.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;
                var version = context.Principal?.FindFirst("user_version")?.Value;
                if (!Guid.TryParse(subject, out var userId) || !Guid.TryParse(version, out var userVersion))
                {
                    context.Fail("Invalid token claims.");
                    return;
                }

                var db = context.HttpContext.RequestServices.GetRequiredService<UsersDbContext>();
                var currentVersion = await db.Users
                    .AsNoTracking()
                    .Where(user => user.Id == userId)
                    .Select(user => (Guid?)user.Version)
                    .SingleOrDefaultAsync(context.HttpContext.RequestAborted);
                if (currentVersion != userVersion)
                    context.Fail("The user account has changed.");
            }
        };
    });

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context => CreateLimiter(GetClientAddress(context), 5));
    options.AddPolicy("artwork-view-recording", context => CreateLimiter(GetClientAddress(context), 30));
    options.AddPolicy("artwork-view-ranking", context => CreateLimiter(GetClientAddress(context), 120));
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var proxy in builder.Configuration.GetSection("ReverseProxy:KnownProxies").Get<string[]>() ?? [])
    {
        if (!IPAddress.TryParse(proxy, out var address))
            throw new InvalidOperationException($"ReverseProxy:KnownProxies contains invalid address '{proxy}'.");
        options.KnownProxies.Add(address);
    }
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
app.MapDefaultEndpoints();
app.UseExceptionHandler();
app.UseForwardedHeaders();
app.UseCors();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await app.MigrateUsersDbAsync();
    await app.MigrateArtworkViewsDbAsync();
}
await app.SeedBootstrapAdminAsync();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();
app.MapUsersAuthentication();
app.MapControllers();
app.MapArtworkViewEndpoints();
app.Run();

static RateLimitPartition<string> CreateLimiter(string key, int permitLimit) =>
    RateLimitPartition.GetFixedWindowLimiter(key, _ => new FixedWindowRateLimiterOptions
    {
        PermitLimit = permitLimit,
        Window = TimeSpan.FromMinutes(1),
        QueueLimit = 0
    });

static string GetClientAddress(HttpContext context)
{
    var address = context.Connection.RemoteIpAddress;
    if (address?.IsIPv4MappedToIPv6 == true) address = address.MapToIPv4();
    return address?.ToString() ?? "unknown";
}

public partial class Program;
