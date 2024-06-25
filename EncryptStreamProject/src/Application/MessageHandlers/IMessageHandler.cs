using Domain.Events.Abstractions;

namespace Application.MessageHandlers;

public interface IMessageHandler
{
    void AddSubscription<T>() where T : IEvent;

    Type? GetMessageTypeByName(string messageName);

    object? GetDataFromMessageType(object data, Type messageType);
}