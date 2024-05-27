using Domain.Events.Abstractions;

namespace Infrastructure.MessageBus.Abstractions;

public interface IMessagePublisher
{
    Task Subscribe();

    Task Publish(IEvent @event);
}