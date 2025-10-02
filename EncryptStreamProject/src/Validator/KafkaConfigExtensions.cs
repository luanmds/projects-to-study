using Confluent.Kafka;
using Infrastructure.MessageBus;
using Infrastructure.MessageBus.Model;
using Infrastructure.MessageBus.Serializers;

namespace Validator;

public static class KafkaConfigExtensions
{
    public const string ConsumerGroupId = "ValidateSecretNotifications";

    public static IHostApplicationBuilder AddConsumerKafka(
        this IHostApplicationBuilder builder,
        MessageBusSettings messageBusSettings,
        string name)
    {

        var channel = messageBusSettings
            .MessageBusChannels
            .Find(x => x is { IsPublisher: false, ConsumerGroupId: ConsumerGroupId });

        builder.AddKafkaConsumer<string, Message>(
            name,
            config =>
            {
                config.Config.BootstrapServers = messageBusSettings.BrokerServer;
                config.Config.GroupId = channel!.ConsumerGroupId;
                config.Config.EnableAutoCommit = true;
                config.Config.SessionTimeoutMs = messageBusSettings.SessionTimeout;
                config.Config.EnableAutoOffsetStore = true;
                config.Config.AutoOffsetReset = AutoOffsetReset.Latest;
            },
            static consumerBuilder =>
            {
                consumerBuilder.SetKeyDeserializer(Deserializers.Utf8);
                consumerBuilder.SetValueDeserializer(new MessageDeserializer());
            }
        );

        return builder;
    }
}
