using Application.Cqrs.Commands;
using Application.Notifications;
using Application.Publishers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.NotificationHandler;

public class SecretEncryptedNotificationHandler(
    ILogger<SecretEncryptedNotificationHandler> logger,
    ICommandPublisher commandPublisher) 
    : INotificationHandler<SecretEncrypted>
{
    public async Task Handle(SecretEncrypted request, CancellationToken cancellationToken)
    {
        var command = new ValidateSecret(request.TraceKey, request.SecretId);

        await commandPublisher.Publish(command, cancellationToken);
        
        logger.LogInformation("Secret encrypted notification has received");
    }
}