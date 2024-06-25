using Domain.Events.Abstractions;

namespace Application.Publishers;

public interface IEventPublisher
{
    Task Publish<T>(T @event, CancellationToken cancellationToken = default) where T : IEvent;
}