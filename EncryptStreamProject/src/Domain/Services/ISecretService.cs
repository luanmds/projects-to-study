using Domain.Model;

namespace Domain.Services;

public interface ISecretService
{
    public Task<string> EncryptSecret(string text, SecretEncryptData secretEncryptData);
    
    public Task<bool> ValidateSecret(string encryptedText, SecretEncryptData secretEncryptData);

    public Task<string> PersistSecret(string text, string keyValue, EncryptType encryptType);
}