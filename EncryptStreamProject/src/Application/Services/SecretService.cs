using System.Security.Cryptography;
using System.Text;
using Domain.Model;
using Domain.Model.Enum;
using Domain.Repositories;
using Domain.Services;
using Microsoft.Extensions.Logging;

namespace Application.Services;

public class SecretService(
    ISecretRepository secretRepository, 
    CryptorBuilder cryptorBuilder, 
    ILogger<SecretService> logger) : ISecretService
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
        var result = secretEncryptData.EncryptType switch
        {
            EncryptType.Aes => await DecryptAes(encryptedText, secretEncryptData.KeyValue),
            _ => throw new ArgumentOutOfRangeException(secretEncryptData.EncryptType.ToString())
        };

        return result is not null;
    }

    public async Task<string> PersistSecret(string text, string keyValue, EncryptType encryptType)
    {
        var secret = new Secret(text, new SecretEncryptData { KeyValue = keyValue, EncryptType = encryptType});

        await secretRepository.AddAsync(secret);
        await secretRepository.Commit();

        return secret.Id;
    }
    
    private async Task<string> EncryptAes(string text, string keyValue)
    {
        var key = await GetCryptorKeyAsBytes(keyValue);
        var aes = cryptorBuilder
            .UseAes()
            .WithKey(key)
            .WithPadding()
            .WithInitializationVector()
            .Build();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var msEncrypt = new MemoryStream();
        await using var csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write);
        await using var swEncrypt = new StreamWriter(csEncrypt);
        
        await swEncrypt.WriteAsync(text);
        await csEncrypt.FlushFinalBlockAsync();
        
        var encrypted = msEncrypt.ToArray();
        return Convert.ToBase64String(encrypted);
    }
    
    private async Task<string?> DecryptAes(string encryptedText, string keyValue)
    {
        var key = await GetCryptorKeyAsBytes(keyValue);
        var aes = cryptorBuilder
            .UseAes()
            .WithKey(key)
            .WithPadding()
            .WithInitializationVector()
            .Build();
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var msDecrypt = new MemoryStream(Convert.FromBase64String(encryptedText));
        await using var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read);
        using var srDecrypt = new StreamReader(csDecrypt);

        try
        {
            var plaintext = await srDecrypt.ReadToEndAsync();
            return plaintext;
        }
        catch (CryptographicException e)
        {
            logger.LogError(e, "Error in Decrypt secret");
            return null;
        }
    }

    private static async Task<byte[]> GetCryptorKeyAsBytes(string text) => 
        await SHA256.HashDataAsync(new MemoryStream(Encoding.UTF8.GetBytes(text)));
}