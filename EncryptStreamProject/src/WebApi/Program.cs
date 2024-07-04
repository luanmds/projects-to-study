using System.Diagnostics.CodeAnalysis;
using Application.Cqrs.Commands;
using Application.Publishers;
using Infrastructure.Configuration;
using Infrastructure.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Serilog;
using WebApi;

[assembly: ExcludeFromCodeCoverage]

// Config Logs
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Infrastructure and Application
builder.Services.AddOptions();
var appSettings = new ApplicationSettings();
var appSettingsSection = builder.Configuration.GetSection("ApplicationSettings");
appSettingsSection.Bind(appSettings);

builder.Services.AddDatabase(builder.Configuration);
builder.Services.ConfigureDomainServices();
builder.Services.AddMessageBus(builder.Configuration);    

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
    await db.Database.MigrateAsync();
}

app.UseHttpsRedirection();

app.MapGet("", async ([FromServices] ICommandPublisher publisher) => {
    await publisher.Publish(
        new CreateSecret(
        Guid.NewGuid().ToString(),
        Guid.NewGuid().ToString(),
        "secret example"));
});

Log.Information("API Running");
await app.RunAsync();