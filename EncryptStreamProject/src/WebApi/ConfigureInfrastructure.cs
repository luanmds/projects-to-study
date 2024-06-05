using System.Diagnostics.CodeAnalysis;
using Application;
using Infrastructure.MessageBus;
using Infrastructure.MessageBus.Abstractions;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;

namespace WebApi;

[ExcludeFromCodeCoverage]
public static class ConfigureInfrastructure
{
    public static void AddMessageBus(this WebApplicationBuilder builder)
    {
        var messageBusSettingsSection = builder.Configuration.GetRequiredSection(nameof(MessageBusSettings));
        var messageBusSettings = new MessageBusSettings();
        messageBusSettingsSection.Bind(messageBusSettings);
        builder.Services.AddSingleton(messageBusSettings);

        builder.Services.AddMediatR(cfg => 
            cfg.RegisterServicesFromAssemblyContaining<IApplicationEntryPoint>());
        
        builder.Services.AddTransient<IMessagePublisher, MessagePublisher>();
    }

    public static void AddDatabaseContext(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;
        builder.Services.AddDbContext<SecretDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

    }
}