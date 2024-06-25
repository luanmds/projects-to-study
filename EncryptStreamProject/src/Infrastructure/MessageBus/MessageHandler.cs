using System.Text.Json;
using Application.MessageHandlers;
using Domain.Events.Abstractions;

namespace Infrastructure.MessageBus;

public class MessageHandler : IMessageHandler
{
    private readonly List<Type> _messageTypes = [];
    
    public void AddSubscription<T>() where T : IEvent
    {
        _messageTypes.Add(typeof(T));
    }

    public Type? GetMessageTypeByName(string messageName)
    {
        return _messageTypes.Find(t => t.Name == messageName);
    }

    public object? GetDataFromMessageType(object data, Type messageType)
    {
        return JsonSerializer.SerializeToDocument(data).Deserialize(messageType);
    }
}