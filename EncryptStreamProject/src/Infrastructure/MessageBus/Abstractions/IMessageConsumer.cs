using Microsoft.Extensions.Hosting;

namespace Infrastructure.MessageBus.Abstractions;

public interface IMessageConsumer : IHostedService
{
    public Task Subscribe();
}