
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;
using System.Text;

namespace Domain.Services;

[ExcludeFromCodeCoverage]
public class CryptorBuilder
{
    private SymmetricAlgorithm? _algorithm = null;
    private readonly byte[] _initializationVectorDefault = RandomNumberGenerator.GetBytes(16);

    public CryptorBuilder UseAes()
    {
        _algorithm = Aes.Create();
        return this;
    }

    public CryptorBuilder WithPadding()
    {
        _algorithm!.Padding = PaddingMode.PKCS7;
        return this;
    }
    
    public CryptorBuilder WithKey(byte[] key)
    {
        _algorithm!.Key = key;
        return this;
    }
    
    public CryptorBuilder WithInitializationVector()
    {
        _algorithm!.IV = _initializationVectorDefault;
        return this;
    }

    public SymmetricAlgorithm Build() => _algorithm!;
}
