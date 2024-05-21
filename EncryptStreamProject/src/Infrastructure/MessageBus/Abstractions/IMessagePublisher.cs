using Domain.Events;

namespace Infrastructure.MessageBus.Abstractions;

public interface IMessagePublisher
{
    Task Subscribe();

    Task Publish(Event @event);
}