using Application.Events.Abstractions;

namespace Application.Events;

public abstract class Event(
    string traceKey,
    string applicationId,
    ChannelType channelType)
    : IEvent
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TraceKey { get; init; } = traceKey;
    public string ApplicationId { get; init; } = applicationId;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public ChannelType ChannelType { get; init; } = channelType;
}