using Application.Services;
using AutoFixture.Xunit2;
using Domain.Model;
using Domain.Model.Enum;
using Domain.Repositories;
using Domain.Services;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EncryptSecretProject.UnitTests.Application.Services;

[Trait("Layer", "Application")]
[Trait("Component", "SecretService")]
public class SecretServiceTests
{
    private readonly ISecretRepository _secretRepository = Substitute.For<ISecretRepository>();
    private readonly MockLogger<SecretService> _logger = Substitute.For<MockLogger<SecretService>>();
    private readonly SecretService _service;
    private const string KeyValueMock = "encryptKeyMock";
    private const string TextRawMock = "textRawMock";
    private const string TextEncryptedMock = "809A52067669B4A738165B7B54A997FC";

    public SecretServiceTests()
    {
        _service = new SecretService(_secretRepository, new CryptorBuilder(), _logger);
    }

    [Trait("Feature", "Encrypt Secret")]
    [Fact(DisplayName = "Given valid encrypt type, should returns encrypted text")]
    public async Task EncryptSecret__GivenValidEncryptType__ShouldReturnsEncryptedText()
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData
        {
            KeyValue = KeyValueMock, 
            EncryptType = EncryptType.Aes
        };
        
        // Act
        var sut = await _service.EncryptSecret(TextRawMock, secretEncryptData);
        
        // Assert 
        Assert.NotEmpty(sut);
    }
    
    [Trait("Feature", "Encrypt Secret")]
    [Theory(DisplayName = "Given encrypt type none, should throws ArgumentOutOfRangeException")]
    [AutoData]
    public async Task EncryptSecret__GivenEncryptTypeNone__ShouldThrowsArgumentOutOfRangeException(string text)
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData { KeyValue = "keyValue", EncryptType = EncryptType.None};
        
        // Act
        var sut = async () => await _service.EncryptSecret(text, secretEncryptData);
        
        // Assert 
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(sut);
    }
    
    [Trait("Feature", "Validate Secret")]
    [Fact(DisplayName = "Given valid encrypt type and correct secret key, then returns true")]
    public async Task ValidateSecret__GivenValidEncryptTypeAndCorrectSecretKey__ThenReturnsTrue()
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData
        {
            KeyValue = KeyValueMock, 
            EncryptType = EncryptType.Aes
        };
        var encrypted = await _service.EncryptSecret(TextRawMock, secretEncryptData);
        
        // Act
        var sut = await _service.ValidateSecret(encrypted, secretEncryptData);
        
        // Assert 
        Assert.True(sut);
    }
    
    [Trait("Feature", "Validate Secret")]
    [Fact(DisplayName = "Given wrong secret key, then returns false")]
    public async Task ValidateSecret__GivenWrongSecretKey__ThenReturnsFalse()
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData
        {
            KeyValue = KeyValueMock, 
            EncryptType = EncryptType.Aes
        };
        
        // Act
        var sut = await _service.ValidateSecret(TextEncryptedMock, secretEncryptData);
        
        // Assert 
        Assert.False(sut);
        _logger.Received().Log(LogLevel.Error, Arg.Any<string>());
    }
    
    [Trait("Feature", "Validate Secret")]
    [Theory(DisplayName = "Given encrypt type none, should throws ArgumentOutOfRangeException")]
    [AutoData]
    public async Task ValidateSecret__GivenEncryptTypeNone__ShouldThrowsArgumentOutOfRangeException(string text)
    {
        // Arrange 
        var secretEncryptData = new SecretEncryptData { KeyValue = "keyValue", EncryptType = EncryptType.None};
        
        // Act
        var sut = async () => await _service.ValidateSecret(text, secretEncryptData);
        
        // Assert 
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(sut);
    }
    
    [Trait("Feature", "Create Secret")]
    [Theory(DisplayName = "Given valid data, should persists successfully")]
    [AutoData]
    public async Task PersistSecret__GivenValidData__ShouldPersistsSuccessfully(
        string text, string keyValue, EncryptType encryptType)
    {
        // Act
        var sut = await _service.PersistSecret(text, keyValue, encryptType);
        
        // Assert 
        Assert.NotEmpty(sut);
        await _secretRepository.Received().AddAsync(Arg.Is<Secret>(s => 
            s.TextEncrypted == text && 
            s.SecretEncryptData.KeyValue == keyValue &&
            s.SecretEncryptData.EncryptType == encryptType));
        await _secretRepository.Received().Commit();
    }
}