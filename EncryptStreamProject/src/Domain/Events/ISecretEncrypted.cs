using Domain.Events.Abstractions;

namespace Domain.Events;

public interface ISecretEncrypted : IEvent
{
    public string SecretId { get; init; }
}