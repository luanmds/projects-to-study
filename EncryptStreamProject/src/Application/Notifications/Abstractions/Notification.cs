using System.Diagnostics.CodeAnalysis;
using Domain.Events;
using Domain.Events.Abstractions;
using MediatR;

namespace Application.Notifications.Abstractions;

[ExcludeFromCodeCoverage]
public abstract class Notification(
    string traceKey, 
    string applicationId) 
    : IEvent, INotification
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TraceKey { get; init; } = traceKey;
    public string ApplicationId { get; init; } = applicationId;
    public DateTimeOffset CreatedAt { get; init; }
    public ChannelType ChannelType { get; init; } = ChannelType.NotificationsTopic;
}