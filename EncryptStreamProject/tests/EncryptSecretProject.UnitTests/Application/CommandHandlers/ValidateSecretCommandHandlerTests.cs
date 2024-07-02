using Application.Cqrs.CommandHandlers;
using Application.Cqrs.Commands;
using AutoFixture.Xunit2;
using Domain.Model.Enum;
using Domain.Repositories;
using Domain.Services;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EncryptSecretProject.UnitTests.Application.CommandHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Validate Secret")]
public class ValidateSecretCommandHandlerTests
{
    private readonly ISecretRepository _secretRepository = Substitute.For<ISecretRepository>();
    private readonly ISecretService _secretService = Substitute.For<ISecretService>();
    private readonly MockLogger<ValidateSecretCommandHandler> _logger = Substitute.For<MockLogger<ValidateSecretCommandHandler>>();
    private readonly ValidateSecretCommandHandler _handler;

    public ValidateSecretCommandHandlerTests()
    {
        _handler = new ValidateSecretCommandHandler(_secretRepository, _secretService, _logger);
    }

    [Theory(DisplayName="When persists secret successfully, should send event")]
    [InlineData("secretId", "correlationId", true, EncryptStatus.Valid)]
    [InlineData("secretId", "correlationId", false, EncryptStatus.NotValid)]
    public async Task Handle__GivenValidSecretId__ShouldValidateAndUpdateStatus(
        string id, string correlationId, bool isValid, EncryptStatus statusExpected)
    {
        // Arrange
        var command = new ValidateSecret(id, correlationId);
        var secret = SecretMock.GetValidEncryptedSecret();
        _secretRepository.GetById(id).Returns(secret);
        _secretService.ValidateSecret(secret.TextEncrypted, secret.SecretEncryptData).Returns(isValid);

        // Act
        await _handler.Handle(command, default);

        // Assert
        Assert.Equivalent(statusExpected, secret.EncryptStatus);
        await _secretService.Received().ValidateSecret(secret.TextEncrypted, secret.SecretEncryptData);
        await _secretRepository.Received().GetById(id);
        _secretRepository.Received().Update(secret);
        await _secretRepository.Received().Commit();
    }
    
    [Theory(DisplayName="When repository throws exception, should not continue flow")]
    [AutoData]
    public async Task Handle__WhenRepositoryThrowsException__ShouldNotContinueFlow(string id, string correlationId)
    {
        // Arrange
        var command = new ValidateSecret(id, correlationId);
        var secret = SecretMock.GetValidEncryptedSecret();
        _secretService.ValidateSecret(secret.TextEncrypted, secret.SecretEncryptData).Returns(true);
        _secretRepository.GetById(id).Returns(secret);
        _secretRepository
            .When(s => s.Update(secret))
            .Do(_ => throw new Exception());
    
        // Act
        var sut = async () => await _handler.Handle(command, default);
    
        // Assert
        await Assert.ThrowsAsync<Exception>(sut);
        await _secretRepository.Received().GetById(id);
        _secretRepository.Received().Update(secret);
        await _secretRepository.DidNotReceive().Commit();
        await _secretService.Received().ValidateSecret(secret.TextEncrypted, secret.SecretEncryptData);
    }
    
    [Theory(DisplayName="Given all flow success, then log result")]
    [AutoData]
    public async Task Handle__GivenAllFlowSuccess__ThenLogResult(string id, string correlationId)
    {
        // Arrange
        var command = new ValidateSecret(id, correlationId);
        var secret = SecretMock.GetValidEncryptedSecret();
        _secretService.ValidateSecret(secret.TextEncrypted, secret.SecretEncryptData).Returns(true);
        _secretRepository.GetById(id).Returns(secret);
    
        // Act
        await _handler.Handle(command, default);
    
        // Assert
        _logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"[Validate Secret] Secret {secret.Id} has been validated successfully")));
    }
}