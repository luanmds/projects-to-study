using Infrastructure.MessageBus.Model;

namespace Infrastructure.Repositories.Abstractions;

public interface IMessageRepository
{
    Task AddAsync(Message message);

    Task Commit();
}