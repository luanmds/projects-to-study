using System.Diagnostics.CodeAnalysis;
using Application.Publishers;
using Confluent.Kafka;
using Infrastructure.MessageBus.Model;
using Microsoft.Extensions.Logging;
using INotificationPublisher = Application.Publishers.INotificationPublisher;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public class MessagePublisher(
    IProducer<string, Message> producer,
    MessageBusSettings settings,
    ILogger<MessagePublisher> logger) : IEventPublisher, INotificationPublisher
{
    private readonly ILogger<MessagePublisher> _logger = logger;
    private readonly List<MessageBusChannel> _channels = [.. settings.MessageBusChannels.Where(x => x.IsPublisher)];

    async Task IEventPublisher.Publish<T>(T @event, CancellationToken cancellationToken)
    {
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