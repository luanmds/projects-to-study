using System.Diagnostics.CodeAnalysis;
using Application;
using Application.MessageHandlers;
using Application.Publishers;
using Confluent.Kafka;
using Domain.Repositories;
using Infrastructure.MessageBus;
using Infrastructure.MessageBus.Model;
using Infrastructure.MessageBus.Serializers;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Infrastructure.Configuration;

[ExcludeFromCodeCoverage]
public static class ConfigureInfrastructure
{

    public static void AddMessageBus(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        var messageBusSettings = new MessageBusSettings();
        configuration.GetSection("MessageBusSettings").Bind(messageBusSettings);

        builder.Services.AddSingleton(messageBusSettings);
        builder.Services.AddSingleton<ISerializer<Message>, MessageSerializer>();

        builder.ConfigureKafkaProducer(messageBusSettings);

        builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<IApplicationEntryPoint>());

        builder.Services.AddSingleton<IMessageHandler, MessageHandler>();
        builder.Services.AddScoped<ICommandPublisher, CommandPublisher>();
        builder.Services.AddScoped<IEventPublisher, MessagePublisher>();
        builder.Services.AddScoped<INotificationPublisher, MessagePublisher>();
    }

    // Deprecated in favor of IHostApplicationBuilder extension method
    // public static void AddDatabase(this IServiceCollection services, IConfiguration configuration)
    // {
    //     services.AddDbContext<SecretDbContext>(options =>
    //         options.UseNpgsql(configuration.GetConnectionString("postgresdb")));
    //     services.AddDbContext<MessageDbContext>(options =>
    //         options.UseNpgsql(configuration.GetConnectionString("postgresdb")));
    //     services.AddScoped<ISecretRepository, SecretRepository>();
    //     services.AddScoped<IMessageRepository, MessageRepository>();
    // }

    public static void AddDatabase(this IHostApplicationBuilder builder, IConfiguration configuration)
    {
        builder.AddNpgsqlDbContext<SecretDbContext>("postgresdb");
        builder.AddNpgsqlDbContext<MessageDbContext>("postgresdb");
        builder.Services.AddScoped<ISecretRepository, SecretRepository>();
        builder.Services.AddScoped<IMessageRepository, MessageRepository>();
    }

    private static void ConfigureKafkaProducer(this IHostApplicationBuilder builder, MessageBusSettings settings)
    {
        builder.AddKafkaProducer<string, Message>(
            "kafka",
            kafkaSettings =>
            {
                kafkaSettings.Config.BootstrapServers = settings.BrokerServer;
            },
            static (serviceProvider, producerBuilder) =>
            {
                var messageSerializer = serviceProvider.GetRequiredService<ISerializer<Message>>();
                producerBuilder.SetValueSerializer(messageSerializer);
            });
    }
}