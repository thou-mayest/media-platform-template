using Host.WebApi;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Users.Infrastracture;
using Users.Presentation;

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();

// register modules
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddUsersPresentation();

var jwtSection = builder.Configuration.GetSection("Jwt");
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
            ValidIssuer = jwtSection["Issuer"],
            ValidAudience = jwtSection["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSection["SecretKey"]!)),
            ClockSkew = TimeSpan.FromMinutes(1)
        };
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:4321") // get from config
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

app.MapDefaultEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


await app.MigrateUsersDbAsync();

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// map controllers from all registered application parts (modules)
app.MapControllers();

app.Run();
