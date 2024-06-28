using Domain.Model.Abstractions;
using Domain.Model.Enum;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public string TextEncrypted { get; set; }
    public SecretEncryptData SecretEncryptData { get; set; }
    public DateTime CreatedAt { get; init; }
    public EncryptStatus EncryptStatus { get; private set; }

    protected Secret(string id) : base(id){ }

    public Secret(string textEncrypted, SecretEncryptData secretEncryptData) : base(Guid.NewGuid().ToString())
    {
        TextEncrypted = textEncrypted;
        SecretEncryptData = secretEncryptData;
        EncryptStatus = EncryptStatus.ToEncrypt;
        CreatedAt = DateTime.UtcNow;        
    }

    public void UpdateTextEncrypted(string textEncrypted)
    {
        TextEncrypted = textEncrypted;
        EncryptStatus = EncryptStatus.Encrypted;
    }
}
