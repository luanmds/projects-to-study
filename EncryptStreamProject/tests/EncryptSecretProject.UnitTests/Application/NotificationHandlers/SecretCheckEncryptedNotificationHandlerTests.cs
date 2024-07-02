using Application.Notifications;
using Application.NotificationHandlers;
using AutoFixture.Xunit2;
using EncryptSecretProject.UnitTests.Mocks;
using NSubstitute;
using Xunit;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EncryptSecretProject.UnitTests.Application.NotificationHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Validate Secret")]
public class SecretCheckEncryptedNotificationHandlerTests
{
    [Theory(DisplayName = "When secret was encrypted, should log checks")]
    [AutoData]
    public async Task Handle__WhenSecretWasEncrypted_ShouldLogChecks(string secretId, string applicationId, string traceKey)
    {
        // Arrange
        var logger = Substitute.For<MockLogger<SecretCheckEncryptedNotificationHandler>>();
        var handler = new SecretCheckEncryptedNotificationHandler(logger);
        var notification = new SecretEncrypted(secretId, applicationId, traceKey);
        
        // Act
        await handler.Handle(notification, CancellationToken.None);
        
        // Assert
        logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"[Check Encrypted] Check if secret {notification.SecretId} was encrypted...")));
        logger.Received().Log(
            LogLevel.Information, Arg.Is<string>(s => s.Contains("[Check Encrypted] Secret was encrypted")));
        logger.Received().Log(
            LogLevel.Debug, 
            Arg.Is<string>(s => s.Contains("[Check Encrypted] Sent secret encrypted notification for e-mail")));
    }
}