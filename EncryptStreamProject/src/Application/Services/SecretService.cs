using Domain.Model;
using Domain.Services;

namespace Application.Services;

public class SecretService : ISecretService
{
    public Task<string> EncryptSecret(string text, HashCryptor hashCryptor)
    {
        throw new NotImplementedException();
    }

    public Task CreateSecret(string text, string hashValue, HashType hashType)
    {
        throw new NotImplementedException();
    }
}