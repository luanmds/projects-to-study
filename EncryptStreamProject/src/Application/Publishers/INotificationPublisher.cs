using Application.Events.Abstractions;

namespace Application.Publishers;

public interface INotificationPublisher
{
    Task Publish<T>(T notification, CancellationToken cancellationToken = default) where T : Notification;
}