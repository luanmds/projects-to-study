using Application.Cqrs.CommandHandlers;
using Application.Cqrs.Commands;
using Application.Publishers;
using AutoFixture.Xunit2;
using Domain.Repositories;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;
using SecretEncrypted = Application.Notifications.SecretEncrypted;

namespace EncryptSecretProject.UnitTests.Application.CommandHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Update Text Secret")]
public class UpdateTextSecretCommandHandlerTests
{
    private readonly ISecretRepository _secretRepository = Substitute.For<ISecretRepository>();
    private readonly INotificationPublisher _notificationPublisher = Substitute.For<INotificationPublisher>();
    private readonly MockLogger<UpdateTextSecretCommandHandler> _logger = Substitute.For<MockLogger<UpdateTextSecretCommandHandler>>();
    private readonly UpdateTextSecretCommandHandler _handler;

    public UpdateTextSecretCommandHandlerTests()
    {
        _handler = new UpdateTextSecretCommandHandler(_secretRepository, _notificationPublisher, _logger);
    }

    [Theory(DisplayName="When persists secret successfully, should send event")]
    [AutoData]
    public async Task Handle__GivenValidSecretId__ShouldUpdateSecretAndSendEvent(
        string id, string correlationId, string textEncrypted)
    {
        // Arrange
        var command = new UpdateTextSecret(id,correlationId, id, textEncrypted);
        var secret = SecretMock.GetValidToEncryptSecret();
        _secretRepository.GetById(id).Returns(secret);

        // Act
        await _handler.Handle(command, default);

        // Assert
        await _notificationPublisher.Received().Publish(Arg.Any<SecretEncrypted>(), Arg.Any<CancellationToken>());
        await _secretRepository.Received().GetById(id);
        _secretRepository.Received().Update(secret);
        await _secretRepository.Received().Commit();
    }
    
    [Theory(DisplayName="When update secret throws exception, should not send event")]
    [AutoData]
    public async Task Handle__WhenUpdateSecretThrowsException__ShouldNotSendEvent(
        string id, string correlationId, string textEncrypted)
    {
        // Arrange
        var command = new UpdateTextSecret(id,correlationId, id, textEncrypted);
        var secret = SecretMock.GetValidToEncryptSecret();
        _secretRepository.GetById(id).Returns(secret);
        _secretRepository
            .When(s => s.Update(secret))
            .Do(_ => throw new Exception());
    
        // Act
        var sut = async () => await _handler.Handle(command, default);
    
        // Assert
        await Assert.ThrowsAsync<Exception>(sut);
        await _secretRepository.Received().GetById(id);
        await _notificationPublisher.DidNotReceive().Publish(Arg.Any<SecretEncrypted>(), Arg.Any<CancellationToken>());
    }
    
    [Theory(DisplayName="Given all flow success, then log result")]
    [AutoData]
    public async Task Handle__GivenAllFlowSuccess__ThenLogResult(string id, string correlationId, string textEncrypted)
    {
        // Arrange
        var command = new UpdateTextSecret(id,correlationId, id, textEncrypted);
        var secret = SecretMock.GetValidToEncryptSecret();
        _secretRepository.GetById(id).Returns(secret);
    
        // Act
        await _handler.Handle(command, default);
    
        // Assert
        _logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"[Update Text Secret] Secret {secret.Id} text encrypted successfully")));
    }
}