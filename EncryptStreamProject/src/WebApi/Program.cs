using Application.Cqrs;
using Infrastructure.Configuration;
using Infrastructure.MessageBus.Abstractions;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using WebApi;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Infrastructure and Application
var appSettings = new ApplicationSettings();
var appSettingsSection = builder.Configuration.GetSection("ApplicationSettings");
appSettingsSection.Bind(appSettings);

builder.AddDatabaseContext();
builder.Services.ConfigureDomainServices();
builder.AddMessageBus();    

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

if(appSettings.UseMigration)
{
    // Migrate tables
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<SecretDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.MapGet("", async (IMessagePublisher publisher) => {
    await publisher.Publish(new CreateSecret(
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        "secret example"));
});


app.Run();