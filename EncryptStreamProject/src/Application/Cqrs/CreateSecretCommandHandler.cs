using Domain.Model;
using Domain.Services;
using MediatR;

namespace Application.Cqrs;

public class CreateSecretCommandHandler(ISecretService secretService) : IRequestHandler<CreateSecret>
{
    public readonly ISecretService _secretService = secretService;

    public async Task Handle(CreateSecret request, CancellationToken cancellationToken)
    {
        await _secretService.EncryptAndPersistSecret(request.SecretTextRaw, request.CorrelationId, HashType.SHA156);
    }
}