using Domain.Abstractions;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public required string TextEncrypted { get; set; }
    public required HashCryptor HashCryptor { get; set; }
    public DateTime CreatedAt { get; init; }
}
