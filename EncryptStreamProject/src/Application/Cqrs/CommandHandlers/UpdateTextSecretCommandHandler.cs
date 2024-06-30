using Application.Cqrs.Commands;
using Application.Notifications;
using Application.Publishers;
using Domain.Model;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.CommandHandlers;

public class UpdateTextSecretCommandHandler(
    ISecretRepository secretRepository, 
    IEventPublisher eventPublisher,
    ILogger<UpdateTextSecretCommandHandler> logger) 
    : IRequestHandler<UpdateTextSecret>
{
    public async Task Handle(UpdateTextSecret request, CancellationToken cancellationToken)
    {
       var secret = await secretRepository.GetById(request.SecretId);
       secret!.UpdateTextEncrypted(request.TextEncrypted);
       
       secretRepository.Update(secret);
       await secretRepository.Commit();

       await SendEvent(secret.Id, request.CorrelationId, cancellationToken);
       
       logger.LogInformation("Secret {Id} text encrypted successfully", secret.Id);
    }

    private async Task SendEvent(string secretId, string correlationId, CancellationToken cancellationToken)
    {
        var @event = new SecretEncrypted(secretId,"Encryptor",correlationId);
       
        await eventPublisher.Publish(@event, cancellationToken);
    }
}