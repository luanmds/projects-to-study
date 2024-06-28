using Application.Cqrs.Commands;
using Application.Publishers;
using Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.CommandHandlers;

public class ValidateSecretCommandHandler(
    ISecretRepository secretRepository, 
    IEventPublisher eventPublisher,
    ILogger<ValidateSecretCommandHandler> logger) 
    : IRequestHandler<ValidateSecret>
{
    public async Task Handle(ValidateSecret request, CancellationToken cancellationToken)
    {
        var secret = await secretRepository.GetById(request.SecretId);
        
        // TODO: VALIDATE SECRET AND UPDATE STATUS
    }
}