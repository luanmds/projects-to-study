using Domain.Model;
using Domain.Model.Enum;

namespace EncryptSecretProject.UnitTests.Mocks;

public static class SecretMock
{
    public static Secret GetValidToEncryptSecret()
    {
        var secretEncryptData = new SecretEncryptData { KeyValue = "key", EncryptType = EncryptType.Aes };
        var secret = new Secret("textEncrypted", secretEncryptData);

        return secret;
    }
    
    public static Secret GetValidEncryptedSecret()
    {
        var secretEncryptData = new SecretEncryptData { KeyValue = "key", EncryptType = EncryptType.Aes };
        var secret = new Secret("textEncrypted", secretEncryptData);
        secret.UpdateTextEncrypted("newTextEncrypted");

        return secret;
    }
}