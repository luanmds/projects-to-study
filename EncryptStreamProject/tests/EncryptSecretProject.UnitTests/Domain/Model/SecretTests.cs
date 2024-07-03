using AutoFixture.Xunit2;
using Domain.Exceptions;
using Domain.Model;
using Domain.Model.Enum;
using EncryptSecretProject.UnitTests.Mocks;
using Xunit;

namespace EncryptSecretProject.UnitTests.Domain.Model;

[Trait("Layer", "Domain")]
[Trait("Aggregate", "Secret")]
public class SecretTests
{
    [Theory(DisplayName = "When all parameters are valid, should not have errors")]
    [AutoData]
    public void Constructor__WhenAllParametersAreValid__ShouldNotHaveErrors(
        string textEncrypted, string keyValue, EncryptType encryptType)
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData { KeyValue = keyValue, EncryptType = encryptType };
        
        // Act
        var sut = new Secret(textEncrypted, secretEncryptData);

        // Assert
        Assert.NotNull(sut.Id);
        Assert.NotEmpty(sut.Id);
        Assert.Equal(sut.TextEncrypted, textEncrypted);
        Assert.Equal(sut.TextEncrypted, textEncrypted);
        Assert.Equal(sut.SecretEncryptData.KeyValue, keyValue);
        Assert.Equal(sut.SecretEncryptData.EncryptType, encryptType);
        Assert.Equivalent(sut.CreatedAt.Date, DateTime.UtcNow.Date);
    }
    
    [Theory(DisplayName = "When new text encrypted is valid, should update successfully")]
    [AutoData]
    public void UpdateTextEncrypted__WhenNewTextEncryptedIsValid__ShouldUpdateSuccessfully(
        Secret sut, string newTextEncrypted)
    {
        // Act
        sut.UpdateTextEncrypted(newTextEncrypted);
        
        // Assert
        Assert.Equal(sut.TextEncrypted, newTextEncrypted);
        Assert.Equal(EncryptStatus.Encrypted, sut.EncryptStatus);
    }
    
    [Theory(DisplayName = "When new text encrypted is invalid, should throw exception")]
    [AutoData]
    public void UpdateTextEncrypted__WhenNewTextEncryptedIsInvalid__ShouldThrowException(
        Secret sut)
    {
        // Arrange 
        var newTextEncrypted = string.Empty;
        
        // Act and Assert
        Assert.Throws<ArgumentOutOfRangeException>(() => sut.UpdateTextEncrypted(newTextEncrypted));
    }
    
    [Theory(DisplayName = "When secret was encrypted, should not make changes")]
    [AutoData]
    public void UpdateTextEncrypted__WhenSecretWasEncrypted__ShouldNotMakeChanges(string newTextEncrypted)
    {
        // Arrange 
        var sut = SecretMock.GetValidEncryptedSecret();
        
        // Act
        sut.UpdateTextEncrypted(newTextEncrypted);
        
        // Assert
        Assert.Equivalent(EncryptStatus.Encrypted, sut.EncryptStatus);
    }
    
    [Theory(DisplayName = "Given validate result and secret is encrypted, should update status")]
    [InlineData(true, EncryptStatus.Valid)]
    [InlineData(false, EncryptStatus.NotValid)]
    public void UpdateValidStatus__GivenValidateResult_And_SecretIsEncrypted__ShouldUpdateStatus(
        bool isValid, EncryptStatus statusExpected)
    {
        // Arrange
        var secretEncryptData = new SecretEncryptData { KeyValue = "AnyKeyValue", EncryptType = EncryptType.Aes };
        var sut = new Secret("AnyTextEncrypted", secretEncryptData);
        
        // Act
        sut.UpdateTextEncrypted("TextEncrypted");
        sut.UpdateValidStatus(isValid);
        
        // Assert
        Assert.Equal(sut.EncryptStatus, statusExpected);
    }
    
    [Fact(DisplayName = "Given validate result and secret is not encrypted, should throws exception")]
    public void UpdateValidStatus__WhenNewTextEncryptedIsInvalid__ShouldThrowsException()
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData { KeyValue = "AnyKeyValue", EncryptType = EncryptType.Aes };
        var sut = new Secret("AnyTextEncrypted", secretEncryptData);
        
        // Act and Arrange
        Assert.Throws<ChangeSecretStatusException>(() => sut.UpdateValidStatus(true));
    }
}