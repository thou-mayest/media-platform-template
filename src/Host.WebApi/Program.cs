using Host.WebApi;
using Scalar.AspNetCore;
using SharedKernal.Configurations;
using System.Text.Json.Serialization;


var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.RegisterModules();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();

builder.Services.ConfigureHttpJsonOptions(o => o.SerializerOptions.Converters.Add(new JsonStringEnumConverter(null, false)));


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

app.UseCors(CorsOptions.CorsFrontendPolicyName);
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// map controllers from all registered application parts (modules)
app.MapControllers();

app.Run();
