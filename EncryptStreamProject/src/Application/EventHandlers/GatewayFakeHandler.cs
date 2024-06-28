using Application.Events;
using Application.Notifications;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public class GatewayFakeHandler(ILogger<GatewayFakeHandler> logger) 
    : INotificationHandler<SecretEncrypted>
{
    public Task Handle(SecretEncrypted request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Secret {Id} encrypted notified to Gateway API", request.Id);
        return Task.CompletedTask;
    }
}