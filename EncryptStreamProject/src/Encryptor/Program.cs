using Application.Events;
using Application.MessageHandlers;
using Encryptor;
using Infrastructure.Configuration;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.AddServiceDefaults();

builder.Services.AddMessageBus(configuration);
builder.Services.ConfigureDomainServices();
builder.Services.AddDatabase(configuration);

builder.Services.AddHostedService<EncryptorWorker>();

var host = builder.Build();

// Subscribe Events in MessageHandler
using var scope = host.Services.CreateScope();
var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
messageHandler.AddSubscription<SecretCreated>();
messageHandler.AddSubscription<SecretEncrypted>();

await host.RunAsync();
