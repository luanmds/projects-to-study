namespace Domain.Events;

public abstract class DomainEvent : Event
{
    public DomainEvent(string traceKey, string applicationId, ChannelType channelName)
    {
        Id = Guid.NewGuid().ToString();
        TraceKey = traceKey;
        ApplicationId = applicationId;
        ChannelName = channelName;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}