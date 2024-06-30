using Application.Cqrs.Commands;
using Domain.Repositories;
using Domain.Services;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.Cqrs.CommandHandlers;

public class ValidateSecretCommandHandler(
    ISecretRepository secretRepository, 
    ISecretService secretService,
    ILogger<ValidateSecretCommandHandler> logger) 
    : IRequestHandler<ValidateSecret>
{
    public async Task Handle(ValidateSecret request, CancellationToken cancellationToken)
    {
        var secret = await secretRepository.GetById(request.SecretId);
        var isValid = await secretService.ValidateSecret(secret!.TextEncrypted, secret.SecretEncryptData);

        secret.UpdateValidStatus(isValid);

        secretRepository.Update(secret);
        await secretRepository.Commit();
        
        logger.LogInformation("[Validate Secret] Secret {Id} has been validated successfully", secret.Id);
    }
}