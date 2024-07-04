using System.Diagnostics.CodeAnalysis;
using Application.MessageHandlers;
using Confluent.Kafka;
using Infrastructure.MessageBus;
using Infrastructure.MessageBus.Model;
using Infrastructure.MessageBus.Serializers;
using MediatR;

namespace Encryptor;

[ExcludeFromCodeCoverage]
public class EncryptorWorker(
    MessageBusSettings messageBusSettings, 
    IMessageHandler messageHandler,
    IServiceScopeFactory serviceScopeFactory,
    ILogger<EncryptorWorker> logger) : BackgroundService
{
    private const string ConsumerGroupId = "SecretEvents";
    
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Start Encryptor Worker for ConsumerGroup {ConsumerGroup} ", ConsumerGroupId);
        
        var channel = messageBusSettings
            .MessageBusChannels
            .Find(x => x is { IsPublisher: false, ConsumerGroupId: ConsumerGroupId });

        if (channel is null)
        {
            logger.LogError("Channel with ConsumerGroupId {GroupId} to consume not found", ConsumerGroupId);
            return;
        }
        
        var config = new ConsumerConfig()
        {
            BootstrapServers = messageBusSettings.BrokerServer,
            GroupId = channel.ConsumerGroupId,
            EnableAutoCommit = true,
            SessionTimeoutMs = messageBusSettings.SessionTimeout,
            EnableAutoOffsetStore = true,
            AutoOffsetReset = AutoOffsetReset.Latest
        };
        
        using var consumer = new ConsumerBuilder<string, Message>(config)
            .SetKeyDeserializer(Deserializers.Utf8)
            .SetValueDeserializer(new MessageDeserializer())
            .Build();
        
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
                        await mediator.Send(messageData, stoppingToken);
                    }
                    catch (Exception e)
                    {
                        logger.LogError(e, "Error in manipulate message");
                        throw;
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
