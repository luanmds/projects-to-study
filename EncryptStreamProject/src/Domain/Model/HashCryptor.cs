using Domain.Abstractions;

namespace Domain.Model;

public class HashCryptor : ValueObject<HashCryptor>
{
    string HashValue { get; init; }
    HashType HashType { get; init; }

    public HashCryptor(string hashValue, HashType hashType)
    {
        HashValue = hashValue;
        HashType = hashType;
    }
}