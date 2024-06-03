using Application.Cqrs.Abstractions;
using Application.Events.Abstractions;

namespace Infrastructure.MessageBus.Abstractions;

public interface IMessagePublisher
{
    Task Publish(IEvent @event, CancellationToken cancellationToken = default);

    Task Publish(ICommand command, CancellationToken cancellationToken = default);
}