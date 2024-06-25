using Domain.Model;

namespace Domain.Services;

public interface ISecretService
{
    public Task<string> EncryptSecret(string text, HashCryptor hashCryptor);

    public Task<string> PersistSecret(string text, string hashValue, HashType hashType);
}