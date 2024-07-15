using System.Security.Cryptography;
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
        var aes = cryptorBuilder
            .UseAes()
            .WithKey(keyValue)
            .WithPaddingPkcs7()
            .WithInitializationVector()
            .Build();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        await using var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
        await using var streamWriter = new StreamWriter(cryptoStream);
        
        await streamWriter.WriteAsync(text);
        await cryptoStream.FlushFinalBlockAsync();
        
        var encrypted = memoryStream.ToArray();
        return Convert.ToBase64String(encrypted);
    }
    
    private async Task<string?> DecryptAes(string encryptedText, string keyValue)
    {
        using var aes = cryptorBuilder
            .UseAes()
            .WithKey(keyValue)
            .WithPaddingPkcs7()
            .WithInitializationVector()
            .Build();
        
        using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(Convert.FromBase64String(encryptedText));
        await using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);

        try
        {
            var plaintext = await streamReader.ReadToEndAsync();
            return plaintext;
        }
        catch (CryptographicException e)
        {
            logger.LogError(e, "Error in Decrypt secret");
            return null;
        }
    }
}