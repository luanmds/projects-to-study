using System.Diagnostics.CodeAnalysis;
using Domain.Events.Abstractions;
using Infrastructure.MessageBus.Model;

namespace Infrastructure.MessageBus;

[ExcludeFromCodeCoverage]
public static class MessageSubscriptionManager
{
    public static Message ParseEventToMessage<T>(T @event) where T : IEvent
    {
        return new Message
        {
            Id = @event.Id,
            Data = @event,
            CreatedAt = DateTimeOffset.UtcNow,
            Label = @event.GetType().Name,
            Type = MessageType.Event,
            RetryCount = 0
        };
    }
}