namespace Domain.Events;

public abstract class Event
{
    public string Id { get; init; }
    public string TraceKey { get; init; }
    public string ApplicationId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ChannelType ChannelName { get; init; }
}