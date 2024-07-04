using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json;
using Confluent.Kafka;
using Infrastructure.MessageBus.Model;

namespace Infrastructure.MessageBus.Serializers;

[ExcludeFromCodeCoverage]
public class MessageDeserializer : IDeserializer<Message>
{
    public Message Deserialize(ReadOnlySpan<byte> data, bool isNull, SerializationContext context)
    {
        string json = Encoding.UTF8.GetString(data.ToArray());

        return JsonSerializer.Deserialize<Message>(json)!;
    }
}