using Host.WebApi;
using Scalar.AspNetCore;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.RegisterModules();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false)));


const string PublicFrontendCors = "public-frontend";
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];
if (!builder.Environment.IsDevelopment() && allowedOrigins.Length == 0)
    throw new InvalidOperationException("Cors:AllowedOrigins must contain at least one production frontend origin.");

builder.Services.AddCors(options =>
{
    options.AddPolicy(PublicFrontendCors, policy =>
    {
        policy.WithOrigins(allowedOrigins.Length > 0 ? allowedOrigins : ["http://localhost:4321"])
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});


var app = builder.Build();

app.MapDefaultEndpoints();
app.UseForwardedHeaders();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(options =>
    {
        options.OpenApiRoutePattern = "/openapi/v1.json";
    });
}

await app.ApplyMigrations();

app.UseCors(PublicFrontendCors);
app.UseHttpsRedirection();
app.UseRateLimiter();
app.UseAuthentication();
app.UseAuthorization();

// map controllers from all registered application parts (modules)
app.MapControllers();

app.Run();
