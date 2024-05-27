using Domain.Events.Abstractions;

namespace Domain.Events;

public abstract class Event(
    string traceKey,
    string applicationId,
    ChannelType channelName)
    : IEvent
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TraceKey { get; init; } = traceKey;
    public string ApplicationId { get; init; } = applicationId;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public ChannelType ChannelName { get; init; } = channelName;
}