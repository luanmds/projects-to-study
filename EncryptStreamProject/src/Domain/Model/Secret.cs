using Domain.Model.Abstractions;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public string TextEncrypted { get; set; }
    public HashCryptor HashCryptor { get; set; }
    public DateTime CreatedAt { get; init; }
    public EncryptStatus Status { get; set; }

    protected Secret(string Id): base(Id) { }

    public Secret(string textEncrypted, HashCryptor hashCryptor) : base(Guid.NewGuid().ToString())
    {
        TextEncrypted = textEncrypted;
        HashCryptor = hashCryptor;
        CreatedAt = DateTime.UtcNow;        
    }
}