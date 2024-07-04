using Application.Cqrs.CommandHandlers;
using Application.Cqrs.Commands;
using Application.Events;
using Application.Publishers;
using AutoFixture.Xunit2;
using Domain.Model;
using Domain.Model.Enum;
using Domain.Services;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using Xunit;

namespace EncryptSecretProject.UnitTests.Application.CommandHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Create Secret")]
public class CreateSecretCommandHandlerTests
{
    private readonly ISecretService _secretService = Substitute.For<ISecretService>();
    private readonly IEventPublisher _eventPublisher = Substitute.For<IEventPublisher>();
    private readonly MockLogger<CreateSecretCommandHandler> _logger = Substitute.For<MockLogger<CreateSecretCommandHandler>>();
    private readonly CreateSecretCommandHandler _handler;

    public CreateSecretCommandHandlerTests()
    {
        _handler = new CreateSecretCommandHandler(_secretService, _eventPublisher, _logger);
    }

    [Theory(DisplayName="When persists secret successfully, should send event")]
    [AutoData]
    public async Task Handle__WhenPersistsSecretSuccessfully__ShouldSendEvent(string id, string correlationId, string secretTextRaw)
    {
        // Arrange
        var command = new CreateSecret(id,correlationId, secretTextRaw);
        _secretService.PersistSecret(secretTextRaw, command.CorrelationId, EncryptType.Aes).Returns(id);

        // Act
        await _handler.Handle(command, default);

        // Assert
        await _eventPublisher.Received().Publish(Arg.Any<SecretCreated>(), Arg.Any<CancellationToken>());
        await _secretService.Received().PersistSecret(secretTextRaw, command.CorrelationId, EncryptType.Aes);
    }
    
    [Theory(DisplayName="When secret service throws exception, should not send event")]
    [AutoData]
    public async Task Handle__WhenPersistenceSecretThrowsException__ShouldNotSendEvent(string id, string correlationId, string secretTextRaw)
    {
        // Arrange
        var command = new CreateSecret(id,correlationId, secretTextRaw);
        _secretService.PersistSecret(secretTextRaw, command.CorrelationId, EncryptType.Aes)
            .ThrowsAsync<Exception>();

        // Act
        var sut = async () => await _handler.Handle(command, default);

        // Assert
        await Assert.ThrowsAsync<Exception>(sut);
        await _eventPublisher.DidNotReceive().Publish(Arg.Any<SecretCreated>(), Arg.Any<CancellationToken>());
        await _secretService.Received().PersistSecret(secretTextRaw, command.CorrelationId, EncryptType.Aes);
    }
    
    [Theory(DisplayName="Given all flow success, then log result")]
    [AutoData]
    public async Task Handle__GivenAllFlowSuccess__ThenLogResult(string id, string correlationId, string secretTextRaw)
    {
        // Arrange
        var command = new CreateSecret(id,correlationId, secretTextRaw);
        _secretService.PersistSecret(secretTextRaw, command.CorrelationId, EncryptType.Aes).Returns(id);

        // Act
        await _handler.Handle(command, default);

        // Assert
        _logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"[Create Secret] Secret {id} as been created")));
    }
}