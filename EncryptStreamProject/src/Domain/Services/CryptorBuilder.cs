using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Services;

[ExcludeFromCodeCoverage]
public class CryptorBuilder
{
    private SymmetricAlgorithm? _algorithm;

    public CryptorBuilder UseAes()
    {
        _algorithm = Aes.Create();
        return this;
    }

    public CryptorBuilder WithPaddingPkcs7()
    {
        _algorithm!.Padding = PaddingMode.PKCS7;
        return this;
    }
    
    public CryptorBuilder WithKey(string key)
    {
        _algorithm!.Key = SHA256.HashData(new MemoryStream(Encoding.UTF8.GetBytes(key)));
        return this;
    }
    
    public CryptorBuilder WithInitializationVector(int byteArraySize = 16)
    {
        _algorithm!.IV = new byte[byteArraySize];
        return this;
    }

    public SymmetricAlgorithm Build() => _algorithm!;
}
