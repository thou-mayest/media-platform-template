using Host.WebApi;
using Users.Infrastracture;
using Users.Presentation;

var builder = WebApplication.CreateBuilder(args);



builder.AddServiceDefaults();

// register modules
builder.Services.AddUsersInfrastructure(builder.Configuration);
builder.Services.AddUsersPresentation();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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

// map endpoints
app.MapUsersEndpoints();

app.Run();
