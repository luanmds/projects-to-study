using Application.MessageHandlers;
using Application.Notifications;
using Infrastructure.Configuration;
using Validator;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.AddServiceDefaults();

builder.Services.AddMessageBus(configuration);
builder.Services.ConfigureDomainServices();
builder.AddDatabase(configuration);

builder.Services.AddHostedService<ValidatorConsumer>();

var host = builder.Build();

using var scope = host.Services.CreateScope();
var messageHandler = scope.ServiceProvider.GetRequiredService<IMessageHandler>();
messageHandler.AddSubscription<SecretEncrypted>();

await host.RunAsync();
