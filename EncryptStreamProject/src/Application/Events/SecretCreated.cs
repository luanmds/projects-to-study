using Application.Events.Abstractions;
using Domain.Events;

namespace Application.Events;

public class SecretCreated(
    string traceKey, string applicationId, string secretId)
    : Event(traceKey, applicationId, "SecretCreated"), ISecretCreated
{
    public string SecretId { get; init; } = secretId;
}