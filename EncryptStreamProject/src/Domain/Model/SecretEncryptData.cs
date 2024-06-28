using Domain.Abstractions;

namespace Domain.Model;

public class SecretEncryptData : ValueObject<SecretEncryptData>
{
    public required string KeyValue { get; init; }
    public EncryptType EncryptType { get; init; }
}