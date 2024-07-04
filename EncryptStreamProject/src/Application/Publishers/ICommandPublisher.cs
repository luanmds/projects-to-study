using Application.Cqrs.Abstractions;

namespace Application.Publishers;

public interface ICommandPublisher
{
    public Task Publish(ICommand command, CancellationToken cancellationToken = default);
}