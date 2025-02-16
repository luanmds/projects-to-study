﻿using Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Application.EventHandlers;

public class GatewayFakeHandler(ILogger<GatewayFakeHandler> logger) : IRequestHandler<SecretEncrypted>
{
    public Task Handle(SecretEncrypted request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Secret {Id} encrypted notified to Gateway API", request.SecretId);
        return Task.CompletedTask;
    }
}