using System.Text.Json;
using Domain.Events.Abstractions;
using Infrastructure.MessageBus.Model;
using MediatR;

namespace Infrastructure.MessageBus;

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
            Type = MessageType.Event
        };
    }
}