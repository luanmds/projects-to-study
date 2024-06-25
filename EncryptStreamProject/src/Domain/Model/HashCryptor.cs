using Domain.Abstractions;

namespace Domain.Model;

public class HashCryptor : ValueObject<HashCryptor>
{
    public string HashValue { get; init; }
    public HashType HashType { get; init; }
}