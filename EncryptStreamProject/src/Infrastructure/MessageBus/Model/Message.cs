using System.Diagnostics.CodeAnalysis;

namespace Infrastructure.MessageBus.Model;

[ExcludeFromCodeCoverage]
public class Message
{
    public required string Id { get; init; }
    
    public DateTimeOffset CreatedAt { get; init; }
    
    public required object Data { get; init; }
    
    public MessageType Type { get; init; }
    
    public required string Label { get; init; }
}

public enum MessageType
{
    Command = 0,
    Event = 1
}