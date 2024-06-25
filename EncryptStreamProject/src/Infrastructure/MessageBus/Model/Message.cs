using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.MessageBus.Model;

[ExcludeFromCodeCoverage]
public class Message
{
    public string Id { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public object Data { get; init; }
    
    public MessageType Type { get; init; }
    
    public string Label { get; init; }
}

public enum MessageType
{
    Command = 0,
    Event = 1
}