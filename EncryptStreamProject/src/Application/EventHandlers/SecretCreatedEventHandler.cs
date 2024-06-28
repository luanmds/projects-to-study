using Application.Cqrs.Commands;
using Application.EventHandlers.Abstractions;
using Application.Events;
using Application.Publishers;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public class SecretCreatedEventHandler(
    ILogger<SecretCreatedEventHandler> logger,
    ICommandPublisher commandPublisher,
    ISecretService secretService,
    ISecretRepository secretRepository) 
    : IEventHandler<SecretCreated>
{
    public async Task Handle(SecretCreated request, CancellationToken cancellationToken)
    {
        var secret = await secretRepository.GetById(request.SecretId);
        
        var textEncrypted = await secretService.EncryptSecret(secret.TextEncrypted, secret.SecretEncryptData);

        var command = new UpdateTextSecret(
            Guid.NewGuid().ToString(), 
            request.TraceKey, 
            request.SecretId,
            textEncrypted);
        
        await commandPublisher.Publish(command, cancellationToken);
        
        logger.LogInformation("Secret created event has been processed. SecretId {Id}", secret.Id);
    }
}