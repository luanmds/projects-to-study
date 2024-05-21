using Domain.Model;

namespace Domain.Services;

public interface ISecretService
{
    protected Task<string> EncryptSecret(string text, HashCryptor hashCryptor);

    public Task CreateSecret(string text, string hashValue, HashType hashType);
}