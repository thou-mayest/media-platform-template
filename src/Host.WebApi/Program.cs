using Host.WebApi;
using Users.Infrastracture;
using Users.Presentation;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Threading.RateLimiting;
using Users.Application.Abstractions;
using Users.Common;
using Microsoft.EntityFrameworkCore;
using Users.Infrastracture.Persistence;
using Microsoft.AspNetCore.HttpOverrides;
using System.Net;

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();

// register modules
builder.Services.AddUsersInfrastructure(builder.Configuration);

var jwtSection = builder.Configuration.GetSection(JwtOptions.SectionName);
var jwtOptions = jwtSection.Get<JwtOptions>()
    ?? throw new InvalidOperationException("JWT configuration is missing.");
if (string.IsNullOrWhiteSpace(jwtOptions.SigningKey) || jwtOptions.SigningKey.Length < 32)
{
    throw new InvalidOperationException("Jwt:SigningKey must contain at least 32 characters.");
}
if (string.IsNullOrWhiteSpace(jwtOptions.Issuer) || string.IsNullOrWhiteSpace(jwtOptions.Audience))
{
    throw new InvalidOperationException("Jwt:Issuer and Jwt:Audience are required.");
}

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
                var subject = context.Principal?.FindFirst(System.IdentityModel.Tokens.Jwt.JwtRegisteredClaimNames.Sub)?.Value;
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
                {
                    context.Fail("The user account has changed.");
                }
            }
        };
    });
builder.Services.AddAuthorization(options =>
    options.AddPolicy("Admin", policy => policy.RequireRole(Role.Admin.ToString())));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("login", context => RateLimitPartition.GetFixedWindowLimiter(
        context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
        _ => new FixedWindowRateLimiterOptions
        {
            PermitLimit = 5,
            Window = TimeSpan.FromMinutes(1),
            QueueLimit = 0
        }));
});
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    foreach (var proxy in builder.Configuration.GetSection("ReverseProxy:KnownProxies").Get<string[]>() ?? [])
    {
        if (!IPAddress.TryParse(proxy, out var address))
        {
            throw new InvalidOperationException($"ReverseProxy:KnownProxies contains invalid address '{proxy}'.");
        }
        options.KnownProxies.Add(address);
    }
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.MapDefaultEndpoints();
app.UseExceptionHandler();
app.UseForwardedHeaders();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (app.Configuration.GetValue<bool>("Database:ApplyMigrations"))
{
    await app.MigrateUsersDbAsync();
}

await app.SeedBootstrapAdminAsync();

app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// map endpoints
app.MapUsersEndpoints();

app.Run();

public partial class Program;
