using Application.Cqrs.Commands;
using Application.Events;
using Application.Publishers;
using Domain.Model;
using Domain.Services;
using MediatR;

namespace Application.Cqrs.CommandHandlers;

public class CreateSecretCommandHandler(
    ISecretService secretService, 
    IEventPublisher eventPublisher) 
    : IRequestHandler<CreateSecret>
{
    public async Task Handle(CreateSecret request, CancellationToken cancellationToken)
    {
        var secretId = await secretService.PersistSecret(
            request.SecretTextRaw, request.CorrelationId, HashType.Sha256);

        var @event = new SecretCreated(request.CorrelationId, "Encryptor", secretId );

        await eventPublisher.Publish(@event, cancellationToken);
    }
}