using Application.Cqrs.Commands;
using Application.Events;
using Application.Publishers;
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

       var @event = new SecretEncrypted(secret.Id, "Encryptor",request.CorrelationId);
       
       await eventPublisher.Publish(@event, cancellationToken);
       
       logger.LogInformation("Secret {Id} text encrypted successfully", secret.Id);
    }
}