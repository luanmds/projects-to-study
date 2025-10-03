using Application.Events;
using Application.MessageHandlers;
using Encryptor;
using Infrastructure.Configuration;
using Infrastructure.MessageBus;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.AddServiceDefaults();

builder.AddMessageBus(configuration);
builder.Services.ConfigureDomainServices();
builder.AddDatabase(configuration);

builder.AddConsumerKafka(
    builder.Services.BuildServiceProvider().GetRequiredService<MessageBusSettings>(),
    "kafka");

builder.Services.AddHostedService<EncryptorWorker>();

var host = builder.Build();

// Subscribe Events in MessageHandler
using var scope = host.Services.CreateScope();
var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
messageHandler.AddSubscription<SecretCreated>();
messageHandler.AddSubscription<SecretEncrypted>();

await host.RunAsync();
