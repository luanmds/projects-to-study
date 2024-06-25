using System.Diagnostics.CodeAnalysis;
using Application.Publishers;
using Confluent.Kafka;
using Infrastructure.MessageBus.Model;
using Infrastructure.MessageBus.Serializers;
using Microsoft.Extensions.Logging;
using INotificationPublisher = Application.Publishers.INotificationPublisher;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public class MessagePublisher : IEventPublisher, INotificationPublisher
{
    private readonly ILogger<MessagePublisher> _logger;
    private readonly ProducerConfig _config;
    private readonly List<MessageBusChannel> _channels;

    public MessagePublisher(MessageBusSettings settings, ILogger<MessagePublisher> logger)
    {
        _logger = logger;      
        _channels = settings.MessageBusChannels.Where(x => x.IsPublisher).ToList();

        _config = new ProducerConfig
        {
            BootstrapServers = settings.BrokerServer
        };
    }

    async Task IEventPublisher.Publish<T>(T @event, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<string, Message>(_config)
            .SetValueSerializer(new MessageSerializer())
            .Build();

        var data = MessageSubscriptionManager.ParseEventToMessage(@event);
        
        var message = new Message<string, Message> { Key = @event.Id, Value = data };
        
        var channelsToPublish = _channels
            .Where(x => x.ChannelType == @event.ChannelType)
            .ToList();
        
        foreach (var channel in channelsToPublish)
        {
            var deliveryResult = await producer.ProduceAsync(channel.TopicName, message , cancellationToken);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                _logger.LogError("Produce not delivered message in Topic {Topic}", deliveryResult.Topic);
            else
                _logger.LogInformation("Produce delivered message {Type} in Topic {Topic} and Partition {Partition}", 
                    nameof(@event), deliveryResult.Topic, deliveryResult.Partition);
        }
    }

    async Task INotificationPublisher.Publish<T>(T notification, CancellationToken cancellationToken)
    {
        using var producer = new ProducerBuilder<string, Message>(_config)
            .SetValueSerializer(new MessageSerializer())
            .Build();

        var data = MessageSubscriptionManager.ParseEventToMessage(notification);
        
        var message = new Message<string, Message> { Key = notification.Id, Value = data };
        
        var channelsToPublish = _channels
            .Where(x => x.ChannelType == notification.ChannelType)
            .ToList();
        
        foreach (var channel in channelsToPublish)
        {
            var deliveryResult = await producer.ProduceAsync(channel.TopicName, message , cancellationToken);

            if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                _logger.LogError("Produce not delivered message in Topic {Topic}", deliveryResult.Topic);
            else
                _logger.LogInformation("Produce delivered message {Type} in Topic {Topic} and Partition {Partition}", 
                    nameof(notification), deliveryResult.Topic, deliveryResult.Partition);
        }
    }
}