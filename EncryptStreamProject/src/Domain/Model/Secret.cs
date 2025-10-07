using System.Diagnostics.CodeAnalysis;
using Domain.Exceptions;
using Domain.Model.Abstractions;
using Domain.Model.Enum;

namespace Domain.Model;

public class Secret : AggregateRoot<Secret>
{
    public string TextEncrypted { get; private set; } = string.Empty;
    public SecretEncryptData SecretEncryptData { get; private set; }
    public DateTime CreatedAt { get; init; }
    public EncryptStatus EncryptStatus { get; private set; }

    [ExcludeFromCodeCoverage]
    protected Secret(string id) : base(id){ }

    public Secret(string textEncrypted, SecretEncryptData secretEncryptData) : base(Guid.NewGuid().ToString())
    {
        if (textEncrypted.Length == 0)
            throw new ArgumentOutOfRangeException(textEncrypted, "Text encrypted should not be null or empty");
        
        TextEncrypted = textEncrypted;
        SecretEncryptData = secretEncryptData;
        EncryptStatus = EncryptStatus.ToEncrypt;
        CreatedAt = DateTime.UtcNow;        
    }

    public void UpdateTextEncrypted(string newTextEncrypted)
    {
        if (newTextEncrypted.Length == 0)
            throw new ArgumentOutOfRangeException(newTextEncrypted, "Text encrypted should not be null or empty");
        
        if(EncryptStatus == EncryptStatus.Encrypted) return;
        
        TextEncrypted = newTextEncrypted;
        EncryptStatus = EncryptStatus.Encrypted;
    }

    public void UpdateValidStatus(bool isValid)
    {
        var newStatus = isValid ? EncryptStatus.Valid : EncryptStatus.NotValid;
        
        if (EncryptStatus != EncryptStatus.Encrypted) 
            throw new ChangeSecretStatusException(EncryptStatus, newStatus);
        
        EncryptStatus = newStatus;
    }
    
}
