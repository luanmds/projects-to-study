using MediatR;
using Microsoft.Extensions.Logging;
using INotificationPublisher = Application.Publishers.INotificationPublisher;

namespace Application.Notifications;

public class SecretEncryptedNotificationHandler(
    ILogger<SecretEncryptedNotificationHandler> logger,
    INotificationPublisher notificationPublisher) 
    : INotificationHandler<SecretEncrypted>
{
    // Send Notification to Validator
    public Task Handle(SecretEncrypted request, CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}