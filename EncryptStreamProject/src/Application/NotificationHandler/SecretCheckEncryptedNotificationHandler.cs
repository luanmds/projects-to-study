using Application.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.NotificationHandler;

public class SecretCheckEncryptedNotificationHandler(ILogger<SecretCheckEncryptedNotificationHandler> logger) 
    : INotificationHandler<SecretEncrypted>
{
    public Task Handle(SecretEncrypted notification, CancellationToken cancellationToken)
    {
        logger.LogInformation("[Check Encrypted] Check if secret {SecretId} was encrypted...", notification.SecretId);
        Thread.Sleep(1000);
        
        logger.LogInformation("[Check Encrypted] Secret was encrypted");
        Thread.Sleep(1000);
        
        logger.LogDebug("[Check Encrypted] Sent secret encrypted notification for e-mail");
        return Task.CompletedTask;
    }
}