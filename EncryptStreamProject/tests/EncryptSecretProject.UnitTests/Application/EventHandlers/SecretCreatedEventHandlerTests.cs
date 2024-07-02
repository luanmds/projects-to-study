using Application.Cqrs.Commands;
using Application.EventHandlers;
using Application.Events;
using Application.Publishers;
using AutoFixture.Xunit2;
using Domain.Model;
using Domain.Repositories;
using Domain.Services;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EncryptSecretProject.UnitTests.Application.EventHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Create Secret")]
public class SecretCreatedEventHandlerTests
{
    private readonly ICommandPublisher _commandPublisher = Substitute.For<ICommandPublisher>();
    private readonly ISecretService _secretService = Substitute.For<ISecretService>();
    private readonly ISecretRepository _secretRepository = Substitute.For<ISecretRepository>();
    private readonly MockLogger<SecretCreatedEventHandler> _logger = Substitute.For<MockLogger<SecretCreatedEventHandler>>();
    private readonly SecretCreatedEventHandler _handler;

    public SecretCreatedEventHandlerTests()
    {
        _handler = new SecretCreatedEventHandler(_commandPublisher, _secretService, _secretRepository, _logger);
    }
    
    
    [Theory(DisplayName = "When secret was created, should encrypt secret and call command")]
    [AutoData]
    public async Task Handle__WhenSecretWasCreated__ShouldEncryptSecretAndCallCommand(string applicationId, string traceKey)
    {
        // Arrange
        var secret = SecretMock.GetValidToEncryptSecret();
        var @event = new SecretCreated(secret.Id, traceKey, applicationId);
        _secretRepository.GetById(secret.Id).Returns(secret);
        _secretService.EncryptSecret(secret.TextEncrypted, secret.SecretEncryptData).Returns(secret.TextEncrypted);
        
        // Act
        await _handler.Handle(@event, CancellationToken.None);
        
        // Assert
        await _secretRepository.Received().GetById(secret.Id);
        await _secretService.Received().EncryptSecret(secret.TextEncrypted, secret.SecretEncryptData);
        await _commandPublisher.Received().Publish(Arg.Any<UpdateTextSecret>(), Arg.Any<CancellationToken>());
    }
    
    [Theory(DisplayName = "When repository throws exception, should not continue flow")]
    [AutoData]
    public async Task Handle__WhenRepositoryThrowsException__ShouldNotContinueFlow(string applicationId, string traceKey)
    {
        // Arrange
        var secret = SecretMock.GetValidToEncryptSecret();
        var @event = new SecretCreated(secret.Id, traceKey, applicationId);
        _secretRepository
            .When(s => s.GetById(secret.Id))
            .Do(_ => throw new Exception());
        
        // Act
        var sut = async () => await _handler.Handle(@event, CancellationToken.None);
        
        // Assert
        await Assert.ThrowsAsync<Exception>(sut);
        await _secretRepository.Received().GetById(secret.Id);
        await _secretService.DidNotReceive().EncryptSecret(Arg.Any<string>(), Arg.Any<SecretEncryptData>());
        await _commandPublisher.DidNotReceive().Publish(Arg.Any<UpdateTextSecret>(), Arg.Any<CancellationToken>());
    }
    
    [Theory(DisplayName = "Given all flow success, then log result")]
    [AutoData]
    public async Task Handle__GivenAllFlowSuccess__ThenLogResult(string applicationId, string traceKey)
    {
        // Arrange
        var secret = SecretMock.GetValidToEncryptSecret();
        var @event = new SecretCreated(secret.Id, traceKey, applicationId);
        _secretRepository.GetById(secret.Id).Returns(secret);
        _secretService.EncryptSecret(secret.TextEncrypted, secret.SecretEncryptData).Returns(secret.TextEncrypted);
        
        // Act
        await _handler.Handle(@event, CancellationToken.None);
        
        // Assert
        _logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"Secret {secret.Id} created event has been processed.")));
    }
}