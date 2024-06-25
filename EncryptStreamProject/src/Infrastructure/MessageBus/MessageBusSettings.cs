using System.Diagnostics.CodeAnalysis;
using Domain.Events;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public class MessageBusSettings
{
    public string BrokerServer { get; init; }

    public int SessionTimeout { get; set; } = 6000;

    public List<MessageBusChannel> MessageBusChannels { get; set; } = [];
}

[ExcludeFromCodeCoverage]
public class MessageBusChannel
{
    public required string TopicName { get; set; }
    public ChannelType ChannelType { get; set; }
    public bool IsPublisher { get; set; }
    
    public string? ConsumerGroupId { get; set; }
}