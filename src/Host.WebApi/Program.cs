using Auth.Infrastructure;
using Auth.Presentation;
using Catalog.Infrastructure;
using Catalog.Presentation;
using Host.WebApi.Infrastructure;
using Users.Infrastracture;
using Users.Presentation;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();
builder.Services.AddApiHostInfrastructure(builder.Configuration);
builder.Services.AddApiDocumentation();

// register modules
builder.Services.AddAuthInfrastructure(builder.Configuration);
builder.Services.AddCatalogInfrastructure(builder.Configuration);
builder.Services.AddUsersInfrastructure(builder.Configuration);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

await app.ApplyDatabaseMigrationsAsync();

app.UseForwardedHeaders();
app.UseExceptionHandler();
app.UseRateLimiter();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

var disableHttpsRedirection = builder.Configuration.GetValue<bool>("DisableHttpsRedirection")
    || string.Equals(builder.Configuration["DISABLE_HTTPS_REDIRECTION"], "true", StringComparison.OrdinalIgnoreCase);

if (!disableHttpsRedirection)
{
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapGet("/api/v1", () => Results.Ok(new
{
    name = "Erofancy API",
    version = "v1",
    status = "ok"
}))
    .WithTags("System")
    .WithName("GetApiDescriptor")
    .WithSummary("Returns a basic descriptor for the API host.");

// map endpoints
app.MapAuthEndpoints();
app.MapCatalogEndpoints();
app.MapUsersEndpoints();

app.Run();
