using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Application.Cqrs.Abstractions;
using Application.Events.Abstractions;
using Confluent.Kafka;
using Infrastructure.MessageBus.Abstractions;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public class MessagePublisher : IMessagePublisher
{
    private readonly ILogger<MessagePublisher> _logger;
    private readonly ISender _sender;
    private readonly ProducerConfig _config;
    private readonly List<MessageBusChannel> _channels;

    public MessagePublisher(MessageBusSettings settings, ISender sender, ILogger<MessagePublisher> logger)
    {
        _logger = logger;
        _sender = sender;        
        _channels = settings.MessageBusChannels.Where(x => x.IsPublisher).ToList();

        _config = new ProducerConfig
        {
            BootstrapServers = settings.BrokerServer
        };
    }

    public Task Publish(IEvent @event, CancellationToken cancellationToken = default)
    {
        using var producer = new ProducerBuilder<string, string>(_config).Build();

        var data = JsonSerializer.Serialize(@event);

        _channels.Where(x => x.ChannelType == @event.ChannelType).ToList()
            .ForEach(async c => 
            {
                var deliveryResult = await producer.ProduceAsync(
                    c.TopicName,  new Message<string, string> { Key = @event.Id, Value = data }, cancellationToken);

                if (deliveryResult.Status == PersistenceStatus.NotPersisted)
                    _logger.LogError("Produce not delivered message in Topic {topic}", deliveryResult.Topic);
                else
                    _logger.LogInformation("Produce delivered message {type} in Topic {topic} and Partition {partition}", 
                        nameof(@event), deliveryResult.Topic, deliveryResult.Partition);
            });
        return Task.CompletedTask;
    }

    public async Task Publish(ICommand command, CancellationToken cancellationToken = default)
    {
        await _sender.Send(command, cancellationToken);
    }
}