using System.Diagnostics.CodeAnalysis;
using Domain.Events;
using Domain.Events.Abstractions;
using MediatR;

namespace Application.Events.Abstractions;

[ExcludeFromCodeCoverage]
public abstract class Event(
    string traceKey,
    string applicationId,
    string eventName)
    : IEvent, IRequest
{
    public string Id { get; init; } = Guid.NewGuid().ToString();
    public string TraceKey { get; init; } = traceKey;
    public string ApplicationId { get; init; } = applicationId;
    public DateTimeOffset CreatedAt { get; init; } = DateTimeOffset.UtcNow;
    public ChannelType ChannelType { get; init; } = ChannelType.EventsTopic;
    public string EventName { get; init; } = eventName;
}