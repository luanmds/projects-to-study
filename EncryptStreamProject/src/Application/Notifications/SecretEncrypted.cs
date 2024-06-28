using Application.Notifications.Abstractions;
using Domain.Events;

namespace Application.Notifications;

public class SecretEncrypted(string secretId, string applicationId, string traceKey) 
    : Notification(traceKey, applicationId), ISecretEncrypted
{
    public string SecretId { get; init; } = secretId;
}