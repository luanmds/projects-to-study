using System.Diagnostics.CodeAnalysis;
using Application;
using Application.MessageHandlers;
using Application.Publishers;
using Confluent.Kafka;
using Domain.Repositories;
using Infrastructure.MessageBus;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public static class ConfigureInfrastructure
{
    public static void AddMessageBus(this IServiceCollection services, IConfiguration configuration)
    {
        var messageBusSettings = new MessageBusSettings();
        configuration.GetSection("MessageBusSettings").Bind(messageBusSettings);
        
        services.AddSingleton(messageBusSettings);
        
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IApplicationEntryPoint>());
        
        services.AddSingleton<IMessageHandler, MessageHandler>();
        services.AddScoped<ICommandPublisher, CommandPublisher>();
        services.AddScoped<IEventPublisher, MessagePublisher>();
        services.AddScoped<INotificationPublisher, MessagePublisher>();
    }

    public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SecretDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("postgresdb")));
        services.AddDbContext<MessageDbContext>(options =>
            options.UseNpgsql(configuration.GetConnectionString("postgresdb")));
        services.AddScoped<ISecretRepository, SecretRepository>();
        services.AddScoped<IMessageRepository, MessageRepository>();
    }
}