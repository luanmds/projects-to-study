namespace Domain.Events;

public class Notification : Event
{
    public Notification(string traceKey, string applicationId, ChannelType channelName) 
        : base(traceKey, applicationId, channelName)
    {
        
    }
}