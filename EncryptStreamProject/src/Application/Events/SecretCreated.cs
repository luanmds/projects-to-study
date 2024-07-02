using Application.Events.Abstractions;
using Domain.Events;

namespace Application.Events;

public class SecretCreated(
    string secretId, string traceKey, string applicationId)
    : Event(traceKey, applicationId, "SecretCreated"), ISecretCreated
{
    public string SecretId { get; init; } = secretId;
}