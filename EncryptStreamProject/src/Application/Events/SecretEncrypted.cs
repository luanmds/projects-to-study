using System.Diagnostics.CodeAnalysis;
using Application.Events.Abstractions;
using Domain.Events;

namespace Application.Events;

[ExcludeFromCodeCoverage]
public class SecretEncrypted(
    string secretId, string applicationId, string traceKey) 
    : Event(traceKey, applicationId, "SecretEncrypted"), ISecretEncrypted
{
    public string SecretId { get; init; } = secretId;
}