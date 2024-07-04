using Application.Cqrs.Commands;
using Application.Notifications;
using Application.Publishers;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.NotificationHandlers;

public class SecretEncryptedNotificationHandler(
    ICommandPublisher commandPublisher,
    ILogger<SecretEncryptedNotificationHandler> logger) 
    : INotificationHandler<SecretEncrypted>
{
    public async Task Handle(SecretEncrypted request, CancellationToken cancellationToken)
    {
        var command = new ValidateSecret(request.SecretId, request.TraceKey);

        await commandPublisher.Publish(command, cancellationToken);
        
        logger.LogInformation("[Validate Secret] Secret {Id} encrypted notification has received", request.SecretId);
    }
}