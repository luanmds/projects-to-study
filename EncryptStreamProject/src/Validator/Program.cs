using Infrastructure.Configuration;
using Validator;

var builder = Host.CreateApplicationBuilder(args);

var configuration = builder.Configuration;

builder.Services.AddMessageBus(configuration);
builder.Services.ConfigureDomainServices();
builder.Services.AddDatabase(configuration);

builder.Services.AddHostedService<ValidateSecretWorker>();
builder.Services.AddHostedService<CheckSecretProcessingWorker>();

var host = builder.Build();
await host.RunAsync();
