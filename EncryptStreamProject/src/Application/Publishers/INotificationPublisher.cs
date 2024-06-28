using Application.Events.Abstractions;
using Application.Notifications.Abstractions;

namespace Application.Publishers;

public interface INotificationPublisher
{
    Task Publish<T>(T notification, CancellationToken cancellationToken = default) where T : Notification;
}