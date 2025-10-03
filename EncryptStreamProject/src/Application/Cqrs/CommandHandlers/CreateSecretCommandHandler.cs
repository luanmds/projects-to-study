using Application.Cqrs.Commands;
using Application.Events;
using Application.Publishers;
using Domain.Model.Enum;
using Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.CommandHandlers;

public class CreateSecretCommandHandler(
    ISecretService secretService, 
    IEventPublisher eventPublisher,
    ILogger<CreateSecretCommandHandler> logger) 
    : IRequestHandler<CreateSecret>
{
    public async Task Handle(CreateSecret request, CancellationToken cancellationToken)
    {
        var secretId = await secretService.PersistSecret(
            request.SecretTextRaw, request.CorrelationId, EncryptType.Aes);
        
        var @event = new SecretCreated(secretId, request.CorrelationId, "Encryptor" );

        await eventPublisher.Publish(@event, cancellationToken);
        
        logger.LogInformation("[Create Secret] Secret {SecretId} as been created", secretId);
    }
}