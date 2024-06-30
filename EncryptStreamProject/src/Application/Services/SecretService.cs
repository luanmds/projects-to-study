using System.Security.Cryptography;
using System.Text;
using Domain.Model;
using Domain.Repositories;
using Domain.Services;

namespace Application.Services;

public class SecretService(ISecretRepository secretRepository) : ISecretService
{
    public async Task<string> EncryptSecret(string text, SecretEncryptData secretEncryptData)
    {
        var encrypted = secretEncryptData.EncryptType switch
        {
            EncryptType.Aes => await EncryptAes(text, secretEncryptData.KeyValue),
            _ => throw new ArgumentOutOfRangeException(secretEncryptData.EncryptType.ToString())
        };

        return encrypted;
    }
    
    public async Task<bool> ValidateSecret(string encryptedText, SecretEncryptData secretEncryptData)
    {
        _ = secretEncryptData.EncryptType switch
        {
            EncryptType.Aes => await DecryptAes(encryptedText, secretEncryptData.KeyValue),
            _ => throw new ArgumentOutOfRangeException(secretEncryptData.EncryptType.ToString())
        };

        return true;
    }

    public async Task<string> PersistSecret(string text, string keyValue, EncryptType encryptType)
    {
        var secret = new Secret(text, new SecretEncryptData { KeyValue = keyValue, EncryptType = encryptType});

        await secretRepository.AddAsync(secret);
        await secretRepository.Commit();

        return secret.Id;
    }
    
    private static async Task<string> EncryptAes(string text, string keyValue)
    {
        var aes = Aes.Create();
        var key = await GetCryptorKeyAsBytes(keyValue);
        aes.Key = key;

        using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        await using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        await using var swEncrypt = new StreamWriter(csEncrypt);
        
        await swEncrypt.WriteAsync(text);
        var encrypted = msEncrypt.ToArray();
        return Convert.ToHexString(encrypted);
    }
    
    private static async Task<string> DecryptAes(string encryptedText, string keyValue)
    {
        var aes = Aes.Create();
        aes.Key = await GetCryptorKeyAsBytes(keyValue);
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(Encoding.UTF8.GetBytes(encryptedText));
        await using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);
        
        var plaintext = await srDecrypt.ReadToEndAsync();

        return plaintext;
    }

    private static async Task<byte[]> GetCryptorKeyAsBytes(string text) => 
        await SHA256.HashDataAsync(new MemoryStream(Encoding.UTF8.GetBytes(text)));
}