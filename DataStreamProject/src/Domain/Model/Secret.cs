using Domain.Abstractions;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public string TextEncripted { get; init; }
    public HashCryptor HashCryptor { get; init; }
    public DateTime CreatedAt { get; init; }
}
