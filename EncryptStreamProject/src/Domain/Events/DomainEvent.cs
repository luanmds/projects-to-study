namespace Domain.Events;

public class DomainEvent : Event
{
    public DomainEvent(string traceKey, string applicationId, ChannelType channelName) 
        : base(traceKey, applicationId, channelName)
    {
        
    }
}