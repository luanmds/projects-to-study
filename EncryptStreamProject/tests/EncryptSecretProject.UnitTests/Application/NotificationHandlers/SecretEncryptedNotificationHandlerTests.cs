using Application.Cqrs.Commands;
using Application.Notifications;
using Application.NotificationHandlers;
using Application.Publishers;
using AutoFixture.Xunit2;
using EncryptSecretProject.UnitTests.Mocks;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace EncryptSecretProject.UnitTests.Application.NotificationHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Validate Secret")]
public class SecretEncryptedNotificationHandlerTests
{
    private readonly ICommandPublisher _commandPublisher = Substitute.For<ICommandPublisher>();
    private readonly MockLogger<SecretEncryptedNotificationHandler> _logger = Substitute.For<MockLogger<SecretEncryptedNotificationHandler>>();
    private readonly SecretEncryptedNotificationHandler _handler;

    public SecretEncryptedNotificationHandlerTests()
    {
        _handler = new SecretEncryptedNotificationHandler(_commandPublisher, _logger);
    }
    
    
    [Theory(DisplayName = "When secret was encrypted, should call validate command")]
    [AutoData]
    public async Task Handle__WhenSecretWasEncrypted__ShouldCallValidateCommand(string applicationId, string traceKey)
    {
        // Arrange
        var secret = SecretMock.GetValidEncryptedSecret();
        var notification = new SecretEncrypted(secret.Id, applicationId, traceKey);
        
        // Act
        await _handler.Handle(notification, CancellationToken.None);
        
        // Assert
        await _commandPublisher.Received().Publish(
            Arg.Is<ValidateSecret>(c => c.SecretId == notification.SecretId), 
            Arg.Any<CancellationToken>());
    }
    
    [Theory(DisplayName = "Given all flow success, then log result")]
    [AutoData]
    public async Task Handle__GivenAllFlowSuccess__ThenLogResult(string applicationId, string traceKey)
    {
        // Arrange
        var secret = SecretMock.GetValidEncryptedSecret();
        var notification = new SecretEncrypted(secret.Id, applicationId, traceKey);
        
        // Act
        await _handler.Handle(notification, CancellationToken.None);
        
        // Assert
        _logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"[Validate Secret] Secret {secret.Id} encrypted notification has received")));
    }
}