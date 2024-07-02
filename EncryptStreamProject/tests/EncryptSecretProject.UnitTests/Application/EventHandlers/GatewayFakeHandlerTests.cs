using Application.EventHandlers;
using Application.Events;
using AutoFixture.Xunit2;
using EncryptSecretProject.UnitTests.Mocks;
using NSubstitute;
using Xunit;
using LogLevel = Microsoft.Extensions.Logging.LogLevel;

namespace EncryptSecretProject.UnitTests.Application.EventHandlers;

[Trait("Layer", "Application")]
[Trait("Feature", "Update Text Secret")]
public class GatewayFakeHandlerTests
{
    [Theory(DisplayName = "When secret was encrypted, should log gateway")]
    [AutoData]
    public async Task Handle__WhenSecretWasEncrypted_ShouldLogGateway(string secretId, string applicationId, string traceKey)
    {
        // Arrange
        var logger = Substitute.For<MockLogger<GatewayFakeHandler>>();
        var handler = new GatewayFakeHandler(logger);
        var @event = new SecretEncrypted(secretId, applicationId, traceKey);
        
        // Act
        await handler.Handle(@event, CancellationToken.None);
        
        // Assert
        logger.Received().Log(
            LogLevel.Information, 
            Arg.Is<string>(s => s.Contains($"Secret {secretId} encrypted notified to Gateway API")));
    }
}