using Application.Cqrs.Commands;
using Application.Notifications;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;
using INotificationPublisher = Application.Publishers.INotificationPublisher;

namespace Application.Cqrs.CommandHandlers;

public class UpdateTextSecretCommandHandler(
    ISecretRepository secretRepository, 
    INotificationPublisher notificationPublisher,
    ILogger<UpdateTextSecretCommandHandler> logger) 
    : IRequestHandler<UpdateTextSecret>
{
    public async Task Handle(UpdateTextSecret request, CancellationToken cancellationToken)
    {
       var secret = await secretRepository.GetById(request.SecretId);
       secret.UpdateTextEncrypted(request.TextEncrypted);
       
       secretRepository.Update(secret);
       await secretRepository.Commit();

       await SendNotification(secret.Id, request.CorrelationId, cancellationToken);
       
       logger.LogInformation("[Update Text Secret] Secret {Id} text encrypted successfully", secret.Id);
    }

    private async Task SendNotification(string secretId, string correlationId, CancellationToken cancellationToken)
    {
        var @event = new SecretEncrypted(secretId,"Encryptor",correlationId);
       
        await notificationPublisher.Publish(@event, cancellationToken);
    }
}