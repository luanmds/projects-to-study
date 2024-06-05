using MediatR;

namespace Application.Events.Abstractions;

public interface IEvent : IRequest
{
    public string Id { get; init; }
    public string TraceKey { get; init; }
    public string ApplicationId { get; init; }
    public DateTimeOffset CreatedAt { get; init; }
    public ChannelType ChannelType { get; init; }
}