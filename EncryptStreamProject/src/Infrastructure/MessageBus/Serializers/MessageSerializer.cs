using System.Diagnostics.CodeAnalysis;
using System.Text;
using Confluent.Kafka;
using Infrastructure.MessageBus.Model;
using System.Text.Json;

namespace Infrastructure.MessageBus.Serializers;

[ExcludeFromCodeCoverage]
public class MessageSerializer : ISerializer<Message>
{
    public byte[] Serialize(Message data, SerializationContext context)
    {
        var json = JsonSerializer.Serialize(data);
        var bytes = Encoding.UTF8.GetBytes(json);
        return bytes;
    }
}