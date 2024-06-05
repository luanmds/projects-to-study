namespace Application.Events;

public class Notification(
    string traceKey, 
    string applicationId) 
    : Event(traceKey, applicationId, ChannelType.NotificationTopic)
{
}