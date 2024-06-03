using Domain.Model;
using Domain.Repositories;
using Domain.Services;

namespace Application.Services;

public class SecretService(ISecretRepository secretRepository) : ISecretService
{
    public ISecretRepository _secretRepository = secretRepository;

    public Task<string> EncryptSecret(string text, HashCryptor hashCryptor)
    {
        throw new NotImplementedException();
    }

    // TODO: Encrypt Secret
    public async Task EncryptAndPersistSecret(string text, string hashValue, HashType hashType)
    {
        var secret = new Secret(text, new HashCryptor(hashValue, hashType));

        await _secretRepository.SaveAsync(secret);
    }
}