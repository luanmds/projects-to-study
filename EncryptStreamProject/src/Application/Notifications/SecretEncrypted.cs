using System.Diagnostics.CodeAnalysis;
using Application.Notifications.Abstractions;
using Domain.Events;

namespace Application.Notifications;

[ExcludeFromCodeCoverage]
public class SecretEncrypted(
    string secretId, string applicationId, string traceKey) 
    : Notification(traceKey, applicationId), ISecretEncrypted
{
    public string SecretId { get; init; } = secretId;
}