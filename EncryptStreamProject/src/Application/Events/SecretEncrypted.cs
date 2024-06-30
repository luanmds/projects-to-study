using Application.Events.Abstractions;
using Domain.Events;

namespace Application.Events;

public class SecretEncrypted(
    string secretId, string applicationId, string traceKey) 
    : Event(traceKey, applicationId, "SecretEncrypted"), ISecretEncrypted
{
    public string SecretId { get; init; } = secretId;
}