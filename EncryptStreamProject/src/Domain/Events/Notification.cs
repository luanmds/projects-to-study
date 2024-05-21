namespace Domain.Events;

public abstract class Notification : Event
{
    public Notification(string traceKey, string applicationId, ChannelType channelName)
    {
        Id = Guid.NewGuid().ToString();
        TraceKey = traceKey;
        ApplicationId = applicationId;
        ChannelName = channelName;
        CreatedAt = DateTimeOffset.UtcNow;
    }
}