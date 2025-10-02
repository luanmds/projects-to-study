using System.Diagnostics.CodeAnalysis;
using Application.Events.Abstractions;
using Application.MessageHandlers;
using Application.Notifications.Abstractions;
using Confluent.Kafka;
using Infrastructure.MessageBus;
using Infrastructure.MessageBus.Model;
using Infrastructure.Repositories.Abstractions;
using MediatR;

namespace Validator;

[ExcludeFromCodeCoverage]
public class ValidatorWorker(
    IConsumer<string, Message> consumer,
    MessageBusSettings messageBusSettings, 
    IMessageHandler messageHandler,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<ValidatorWorker> logger) : BackgroundService
{
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Validator Worker started at: {time}", DateTimeOffset.Now);
        
        var channel = messageBusSettings
            .MessageBusChannels
            .Find(x => x is { IsPublisher: false, ConsumerGroupId: KafkaConfigExtensions.ConsumerGroupId })!;
        
        consumer.Subscribe(channel.TopicName);
        
        try
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = serviceScopeFactory.CreateScope();
                try
                {
                    var result = consumer.Consume(stoppingToken);
                    
                    var message = result.Message.Value;
                    var messageType = messageHandler.GetMessageTypeByName(message.Label);

                    if (messageType is null)
                    {
                        logger.LogWarning("Message Label {Label} not exists", message.Label);
                        return;
                    }

                    var messageData = messageHandler.GetDataFromMessageType(message.Data, messageType)!;

                    var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                    try
                    {
                        switch (messageData)
                        {
                            case Event:
                                await mediator.Send(messageData, stoppingToken);
                                break;
                            case Notification:
                                await mediator.Publish(messageData, stoppingToken);
                                break;
                        }
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error in manipulate message");
                        
                        var repository = scope.ServiceProvider.GetRequiredService<IMessageRepository>();
                        await repository.AddAsync(message);
                        await repository.Commit();
                    }
                    
                    logger.LogInformation("Message with Key {Key} and Type {Type} has been consumed",
                        result.Message.Key,
                        messageData.GetType());
                }
                catch (ConsumeException e)
                {
                    logger.LogError(e, "Error occured in consumer");
                }
            }
        }
        catch (OperationCanceledException)
        {
            consumer.Close();
        }
    }
}
