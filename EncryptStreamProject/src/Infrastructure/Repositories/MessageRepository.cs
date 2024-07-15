using Infrastructure.MessageBus.Model;
using Infrastructure.Repositories.Abstractions;
using Infrastructure.Settings;

namespace Infrastructure.Repositories;

public class MessageRepository(MessageDbContext dbContext) : IMessageRepository
{
    public async Task AddAsync(Message message)
    {
        await dbContext.Messages.AddAsync(message);
    }

    public async Task Commit()
    {
        await dbContext.SaveChangesAsync();
    }
}