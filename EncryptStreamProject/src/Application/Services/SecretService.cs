using System.Security.Cryptography;
using System.Text;
using Domain.Model;
using Domain.Repositories;
using Domain.Services;

namespace Application.Services;

public class SecretService(ISecretRepository secretRepository) : ISecretService
{
    // TODO: Encrypt Secret
    public async Task<string> EncryptSecret(string text, HashCryptor hashCryptor)
    {
        string encrypted;
        switch (hashCryptor.HashType)
        {
            case HashType.Sha256:
                encrypted = await EncryptSha256(text);
                break;
            default:
                throw new ArgumentOutOfRangeException(hashCryptor.HashType.ToString());
        }

        return encrypted;
    }

    public async Task<string> PersistSecret(string text, string hashValue, HashType hashType)
    {
        var secret = new Secret(text, new HashCryptor { HashValue = hashValue, HashType = hashType});

        await secretRepository.SaveAsync(secret);

        return secret.Id;
    }
    
    private async Task<string> EncryptSha256(string text)
    {
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(text));
        var inputHash = await SHA256.HashDataAsync(stream);
        return Convert.ToHexString(inputHash);
    }
}