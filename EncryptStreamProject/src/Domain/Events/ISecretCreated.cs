using Domain.Events.Abstractions;

namespace Domain.Events;

public interface ISecretCreated : IEvent
{
    public string SecretId { get; init; }
}