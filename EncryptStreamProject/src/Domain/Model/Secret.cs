using Domain.Model.Abstractions;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public required string TextEncrypted { get; set; }
    public required HashCryptor HashCryptor { get; set; }
    public DateTime CreatedAt { get; init; }

     // Obrigatório para o EF funcionar
    protected Secret() { }

    public Secret(string textEncrypted, HashCryptor hashCryptor)
    {
        Id = Guid.NewGuid().ToString();
        TextEncrypted = textEncrypted;
        HashCryptor = hashCryptor;
        CreatedAt = DateTime.UtcNow;
    }
}
