namespace Domain.Events.Abstractions;

public interface IEvent
{
    public string Id { get; init; }
    public string TraceKey { get; init; }
    public string ApplicationId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ChannelType ChannelName { get; init; }
}